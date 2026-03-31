using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Material;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Content.CopyTopic
{
    [PublicAPI]
    public class CopyNodeCommand : ICommand<List<CopyNodeCommand.Result>>
	{
		//public Guid CourseId { get; set; }
		public Guid ParentId { get; set; }
		public List<Guid> NodeIds { get; set; }

		[PublicAPI]
	    public class Result
		{
			public Guid EntityId { get; set; }
			public Guid? ParentId { get; set; }
			public long Depth { get; set; }
			public string Header { get; set; }
			public long Order { get; set; }
			public bool IsCanAddChildren { get; set; }
			public bool IsCanAttachContent { get; set; }
		}


		internal class Validator : ValidatorBase<CopyNodeCommand>
        {
            public Validator(IDb db) : base(db)
            {
				//RuleFor(_ => _.CourseId).ExistsInTable(db.Courses).Must(_ => db.Courses.Where(x => x.CourseId == _).Include(x => x.Curriculum).All(x => !x.Curriculum.IsGeneric)).WithMessage("Can't do this for Generic course");
			}
        }


        internal class PermissionCheck: PermissionCheckBase<CopyNodeCommand>
        {
            private readonly IDb db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => this.db = db;

            protected override bool CanExecute(CopyNodeCommand request, IUserSecurityInfo user)
            {
				if (user.IsApplicationAdmin()) return true;
				return false;
            }
        }


        internal class Handler : CommandHandlerBase<CopyNodeCommand, List<Result>>
        {
	        private readonly ILogger _logger;


			public Handler(IDb db, ILogger<CopyNodeCommand> logger)
				: base(db)
                => _logger = logger;

            public override async Task<List<Result>> Handle(CopyNodeCommand request,
                                                            CancellationToken cancellationToken)
            {
	            var result = new List<Result>();
				var parentNode = await Db.ContentTree
					.Include(x => x.ContentTreeMeta)
					.Where(x => x.NodeId == request.ParentId)
					.FirstOrDefaultAsync(cancellationToken);
				if (parentNode == null) throw new ValidationException("Node to copy was not found");
				if (!parentNode.ContentTreeMeta.CouldCopyIn)
					throw new ValidationException("Can paste only in Key Knowledge");

                var siblings = await Db.ContentTree.Where(x => x.ParentId == request.ParentId)
                    .ToListAsync(cancellationToken);


                var order = siblings.Any() 
					? siblings.Max(x => x.Order)
						: 0;

				foreach (var nodeId in request.NodeIds)
				{
					order++;
					var node = await CopyNode(nodeId, parentNode, order, cancellationToken);
					result.Add(node);
				}

				return result;
            }

            private async Task<Result> CopyNode(Guid nodeId, ContentTreeEntity parentNode, long order, CancellationToken cancellationToken)
            {
				var nodeToCopy = await Db.ContentTree
					.Include(x => x.ContentTreeMeta)
					.Where(x => x.NodeId == nodeId)
					.FirstOrDefaultAsync(cancellationToken);
				if (nodeToCopy == null) throw new ValidationException("Node to copy was not found");
				if (nodeToCopy.ContentTreeMeta.CouldAddContent || nodeToCopy.ContentTreeMeta.CouldAddEntry)
					throw new ValidationException("Can only copy topics");

				var meta = await Db.ContentTreeMeta
					.Where(x => x.CourseId == parentNode.CourseId)
					.ToListAsync(cancellationToken);
				var topicMeta = meta.First(x => x.Depth == parentNode.ContentTreeMeta.Depth + 1);
				var topic = Db.ContentTree
					.Add(new ContentTreeEntity
					{
						ContentTreeMetaId = topicMeta.ContentTreeMetaId,
						CourseId = parentNode.CourseId,
						Name = nodeToCopy.Name,
						Order = order,
						ParentId = parentNode.NodeId
					});

				var levels = await Db.ContentTree.Where(x => x.ParentId == nodeToCopy.NodeId)
					.ToListAsync(cancellationToken);
				foreach (var level in levels)
				{
					// Copy level
					var addedLevel = Db.ContentTree
						.Add(new ContentTreeEntity
						{
							ContentTreeMetaId = meta.First(x => x.Depth == parentNode.ContentTreeMeta.Depth + 2).ContentTreeMetaId,
							CourseId = parentNode.CourseId,
							Name = level.Name,
							Order = level.Order,
							ParentId = topic.Entity.NodeId
						});

					// Add quiz
					var addedQuiz = await Db.Quizzes.AddAsync(new QuizEntity
					{
						ContentTreeId = addedLevel.Entity.NodeId,

					}, cancellationToken);

					// Copy questions
					var questions = await Db.QuizQuestions
						.Include(x => x.Quiz)
						.ThenInclude(x => x.ContentTreeNode)
						.Where(x => x.Quiz.ContentTreeNode.NodeId == level.NodeId)
						.ToListAsync(cancellationToken);
					foreach (var question in questions)
					{
						await Db.QuizQuestions.AddAsync(new QuizQuestionEntity
						{
							Order = question.Order,
							QuestionId = question.QuestionId,
							QuizId = addedQuiz.Entity.QuizId
						}, cancellationToken);
					}

                    addedQuiz.Entity.AutoMapQuizId = questions.FirstOrDefault()?.QuizId;

                    // Add page
                    var addedPage = await Db.Pages.AddAsync(new PageEntity
					{
						ContentTreeId = addedLevel.Entity.NodeId,
					}, cancellationToken);

					// Copy materials
					var headings = await Db.PageMaterials
						.Include(x => x.Page)
						.ThenInclude(x => x.ContentTreeNode)
						.Where(x => x.Page.ContentTreeNode.NodeId == level.NodeId)
						.ToListAsync(cancellationToken);
					foreach (var heading in headings)
					{
						await Db.PageMaterials.AddAsync(new PageMaterialEntity
						{
							Order = heading.Order,
							MaterialId = heading.MaterialId,
							PageId = addedPage.Entity.PageId
						}, cancellationToken);
					}

				}

				await Db.SaveChangesAsync(cancellationToken);

				return new Result
				{
					ParentId = topic.Entity.ParentId,
					Depth = parentNode.ContentTreeMeta.Depth++,
					Order = topic.Entity.Order,
					IsCanAddChildren = topicMeta.CouldAddEntry,
					IsCanAttachContent = topicMeta.CouldAddContent,
					EntityId = topic.Entity.NodeId,
					Header = topic.Entity.Name,
				};
			}
		}
    }
}
