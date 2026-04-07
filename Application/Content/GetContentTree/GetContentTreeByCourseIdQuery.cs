using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Material;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.GetContentTree
{
    [PublicAPI]
    public sealed class GetContentTreeByCourseIdQuery : IQuery<List<GetContentTreeByCourseIdQuery.Result>>
    {
        public Guid CourseId { get; init; }
        public TreeMode Mode { get; init; }


        public enum TreeMode
        {
	        All = 0,
	        Materials = 1,
	        Questions = 2,
			Topics = 3
        }

        [PublicAPI]
        public record Result
        {
            public Guid EntityId { get; set; }
            public Guid? ParentId { get; set; }
            public long Depth { get; set; }
            public string Header { get; set; }
            public long Order { get; set; }
            public List<Result> Children { get; set; } = new List<Result>();
            public bool IsCanAddChildren { get; set; }
            public bool IsCanAttachContent { get; set; }
            public bool IsCanCopyIn { get; set; }
			public bool IsMaterialsFolder { get; set; }
            public bool IsQuestionsFolder { get; set; }
			public bool IsAutoMapped { get; set; }
			public bool IsExcluded { get; set; }
            public bool IsAvailableInDemo { get; set; }
		}
        
        internal sealed class PermissionCheck : PermissionCheckBase<GetContentTreeByCourseIdQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetContentTreeByCourseIdQuery request, IUserSecurityInfo user)
                => user.IsApplicationAdmin() || user.IsTeacher() || user.IsStudent();
        }


        internal sealed class Handler : QueryHandlerBase<GetContentTreeByCourseIdQuery, List<Result>>
        {
	        private const int NumberOfPreviewSymbols = 15;
            public Handler(IDb db) : base(db) { }

            public override async Task<List<Result>> Handle(GetContentTreeByCourseIdQuery request, CancellationToken cancellationToken)
            {
	            var models = await Db.ContentTree
		            .Where(x => x.CourseId == request.CourseId)
		            .Include(x => x.ContentTreeMeta)
		            .Include(x => x.Icon)
		            .Where(DeletedSpec<ContentTreeEntity>.NotDeleted())
		            .AsNoTracking()
		            .ToListAsync(cancellationToken);

	            var pages = await Db.Pages.AsNoTracking()
		            .Include(x => x.ContentTreeNode)
		            .Include(x => x.PageMaterials)
		            .ThenInclude(x => x.Material)
		            .Where(x => x.ContentTreeNode.CourseId == request.CourseId)
		            .ToListAsync(cancellationToken);

	            var quizzes = await Db.Quizzes.AsNoTracking()
                    .Include(_ => _.AutoMapQuiz).ThenInclude(_ => _.QuizQuestions).ThenInclude(_ => _.Question)
                    .Include(_ => _.QuizExcludedQuestions).ThenInclude(_ => _.Question)
		            .Include(x => x.ContentTreeNode)
		            .Include(x => x.QuizQuestions)
		            .ThenInclude(x => x.Question)
		            .Where(x => x.ContentTreeNode.CourseId == request.CourseId)
		            .ToListAsync(cancellationToken);

	            var tree = models.Where(x => x.ParentId == null).Select(ResultByContentTreeEntity).OrderBy(x => x.Order).ToList();
	            tree.ForEach(x => SetChildrenRecursively(x, models, pages, quizzes, request.Mode));
                return tree;
            }

            private void SetChildrenRecursively(Result parent, List<ContentTreeEntity> models, List<PageEntity> pages, List<QuizEntity> quizzes, TreeMode mode)
            {
				if(mode == TreeMode.Topics && (parent.IsCanAddChildren == false && parent.IsCanAttachContent == false)) return;

	            parent.Children = models
		            .Where(x => x.ParentId == parent.EntityId && x.ContentTreeMeta.Depth == parent.Depth + 1)
		            .Select(ResultByContentTreeEntity)
		            .OrderBy(x => x.Order)
		            .ToList();

	            if (!parent.IsCanAddChildren && parent.IsCanAttachContent)
	            {
		            if (mode == TreeMode.All || mode == TreeMode.Materials)
		            {
			            var materialsNode = new Result
			            {
				            EntityId = Guid.NewGuid(),
							Header = "Materials",
				            Order = 0,
				            ParentId = parent.EntityId,
				            Depth = parent.Depth + 1,
				            IsCanAddChildren = false,
				            IsCanAttachContent = true,
				            Children = new List<Result>(),
				            IsMaterialsFolder = true,
				            IsCanCopyIn = false,
						};
			            parent.Children.Add(materialsNode);
			            var page = pages.FirstOrDefault(x => x.ContentTreeId == parent.EntityId);
			            if (page != null)
			            {
				            materialsNode.Children.AddRange(page.PageMaterials.OrderBy(x => x.Order).Select(x =>
					            new Result
					            {
						            EntityId = x.MaterialId,
						            ParentId = materialsNode.EntityId,
						            Header = x.Material.Header,
						            Depth = materialsNode.Depth + 1,
						            Order = x.Order,
						            IsCanAddChildren = false,
						            IsCanAttachContent = false,
						            IsCanCopyIn = false
								}));
			            }
		            }

		            if (mode == TreeMode.All || mode == TreeMode.Questions)
                    {
                        var quiz = quizzes.FirstOrDefault(x => x.ContentTreeId == parent.EntityId);
                        var questionsNode = new Result
			            {
				            EntityId = quiz?.QuizId ?? Guid.NewGuid(),
				            Header = "Questions",
				            Order = 1,
				            ParentId = parent.EntityId,
				            Depth = parent.Depth + 1,
				            IsCanAddChildren = false,
				            IsCanAttachContent = true,
				            Children = new List<Result>(),
				            IsQuestionsFolder = true,
				            IsCanCopyIn = false,
							IsAutoMapped = quiz?.AutoMapQuiz != null
						};
			            parent.Children.Add(questionsNode);
			            if (quiz != null)
			            {
				            questionsNode.Children.AddRange(quiz.QuizQuestions.OrderBy(x => x.Order).Select(x =>
					            new Result
					            {
						            EntityId = x.QuestionId,
						            ParentId = questionsNode.EntityId,
						            Header = GetQuestionHeader(x),
						            Depth = questionsNode.Depth + 1,
						            Order = x.Order,
						            IsCanAddChildren = false,
						            IsCanAttachContent = false,
						            IsCanCopyIn = false,
									IsAutoMapped = quiz.AutoMapQuiz != null
                                }));
                            if (quiz.AutoMapQuiz != null && quiz.QuizExcludedQuestions.Any())
                            {
								//Handle not included questions
                                var excludedQuestions = quiz.QuizExcludedQuestions;
                                questionsNode.Children.AddRange(excludedQuestions.OrderBy(x => x.Order).Select(x =>
                                    new Result
                                    {
                                        EntityId = x.QuestionId,
                                        ParentId = questionsNode.EntityId,
                                        Header = GetExcludedQuestionHeader(x),
                                        Depth = questionsNode.Depth + 1,
                                        Order = x.Order,
                                        IsCanAddChildren = false,
                                        IsCanAttachContent = false,
                                        IsCanCopyIn = false,
										IsExcluded = true,
                                        IsAutoMapped = quiz.AutoMapQuiz != null
                                    }));
                            }
			            }
		            }

		            return;
                }

	            parent.Children.ForEach(x => SetChildrenRecursively(x, models, pages, quizzes, mode));
            }

            private string GetQuestionHeader(QuizQuestionEntity question)
            {
	            var raw = RemoveTags(question.Question.Text);
	            return $"Q{question.Order} - {raw.Substring(0, raw.Length > NumberOfPreviewSymbols ? NumberOfPreviewSymbols : raw.Length)}";
            }

            private string GetExcludedQuestionHeader(QuizExcludedQuestionEntity question)
            {
                var raw = RemoveTags(question.Question.Text);
                return $"Q{question.Order} - {raw.Substring(0, raw.Length > NumberOfPreviewSymbols ? NumberOfPreviewSymbols : raw.Length)}";
            }

            private Result ResultByContentTreeEntity(ContentTreeEntity model) => new Result
            {
                EntityId = model.NodeId,
                ParentId = model.ParentId,
                Depth = model.ContentTreeMeta.Depth,
                Header = model.Name,
                IsCanAddChildren = model.ContentTreeMeta.CouldAddEntry,
                IsCanAttachContent = model.ContentTreeMeta.CouldAddContent,
				IsCanCopyIn = model.ContentTreeMeta.CouldCopyIn,
				Order = model.Order,
                IsAvailableInDemo = model.IsAvailableInDemo
            };

            public string RemoveTags(string text)
            {
                var htmlTagsRegEx = new Regex("<[^>]*>");
                var spacesRegEx = new Regex(@"\s+");
                var raw = htmlTagsRegEx.Replace(text, string.Empty);
	            raw = raw.Replace("&nbsp;", " ");
	            raw = spacesRegEx.Replace(raw, " ");
	            return raw;
            }
        }
    }
}