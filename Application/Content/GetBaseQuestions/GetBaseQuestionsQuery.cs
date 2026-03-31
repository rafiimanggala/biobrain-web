using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Quiz;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.GetBaseQuestions
{
    [PublicAPI]
    public sealed class GetBaseQuestionsQuery : IQuery<List<GetBaseQuestionsQuery.Result>>
    {
	    public Guid BaseCourseId { get; set; }


        [PublicAPI]
        public record Result
        {
	        public Guid EntityId { get; init; }
	        public List<string> Path { get; init; }
        }

        internal record TreeNode
        {
	        public Guid EntityId { get; init; }
	        public string Name { get; init; }
	        public long Order { get; init; }
	        public List<TreeNode> Children { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetBaseQuestionsQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetBaseQuestionsQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetBaseQuestionsQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<List<Result>> Handle(GetBaseQuestionsQuery request, CancellationToken cancellationToken)
            {
	            var nodes = await Db.ContentTree
		            .Where(x => x.CourseId == request.BaseCourseId)
		            .AsNoTracking()
		            .ToListAsync(cancellationToken);

                var quizzes = await Db.Quizzes.AsNoTracking()
		            .Include(x => x.ContentTreeNode)
		            .Include(x => x.QuizQuestions)
		            .ThenInclude(x => x.Question)
		            .Where(x => x.ContentTreeNode.CourseId == request.BaseCourseId)

                    .ToListAsync(cancellationToken);

                var tree = nodes.Where(x => x.ParentId == null).Select(EntityToTreeNode).OrderBy(x => x.Order).ToList();
                tree.ForEach(x => SetChildrenRecursively(x, nodes));

                var results = FillResults(tree, new List<string>(), quizzes, new List<Result>());

                return results;
            }

            public List<Result> FillResults(List<TreeNode> treeLevel, List<string> existingPath, List<QuizEntity> pages, List<Result> result)
            {
	            foreach (var node in treeLevel)
	            {
		            if (node.Children.Any())
		            {
			            var path = new List<string>(existingPath);
                        path.Add(node.Name);
                        FillResults(node.Children, path, pages, result);
		            }
		            else
		            {
			            var nodePage = pages.FirstOrDefault(x => x.ContentTreeId == node.EntityId);
			            if (nodePage == null) continue;

			            foreach (var material in nodePage.QuizQuestions.OrderBy(x => x.Order))
			            {
                            var entry = new Result
                            {
	                            EntityId = material.QuestionId,
	                            Path = existingPath
                            };
                            entry.Path.Add(material.Question.Header);
                            result.Add(entry);
                        }
		            }
	            }

	            return result;
            }

            private void SetChildrenRecursively(TreeNode parent, List<ContentTreeEntity> models)
            {
	            parent.Children.AddRange(models
		            .Where(x => x.ParentId == parent.EntityId)
		            .Select(EntityToTreeNode)
		            .OrderBy(x => x.Order)
		            .ToList());

	            parent.Children.ForEach(x => SetChildrenRecursively(x, models));
            }

            private TreeNode EntityToTreeNode(ContentTreeEntity entity) => new()
                                                                           {
                                                                               Name = entity.Name,
                                                                               Children = new List<TreeNode>(),
                                                                               EntityId = entity.NodeId,
                                                                               Order = entity.Order
                                                                           };
        }
    }
}