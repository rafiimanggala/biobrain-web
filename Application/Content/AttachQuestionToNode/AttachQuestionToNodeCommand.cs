using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.AttachQuestionToNode
{
    [PublicAPI]
    public sealed class AttachQuestionToNode : IQuery<AttachQuestionToNode.Result>
    {
	    public Guid NodeId { get; set; }
	    public List<KeyValue> QuestionIds { get; set; }
	    public bool IsReplaceMode { get; set; }

		public record KeyValue
	    {
		    public int Key { get; set; }
		    public Guid Value { get; set; }
	    };


        [PublicAPI]
        public record Result;

        internal class Validator : ValidatorBase<AttachQuestionToNode>
        {
	        public Validator(IDb db) : base(db)
	        {
		        RuleFor(_ => _.NodeId).ExistsInTable(Db.ContentTree);
		        //RuleForEach(_ => _.QuestionIds).ExistsInTable(Db.Questions);
		        RuleFor(_ => _.NodeId).MustAsync(async (command, _, _) =>
			        (await Db.ContentTree.Where(x => x.NodeId == command.NodeId).Include(x => x.ContentTreeMeta)
				        .FirstAsync()).ContentTreeMeta.CouldAddContent).WithMessage("Can't attach questions to this node.");
	        }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<AttachQuestionToNode>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(AttachQuestionToNode request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<AttachQuestionToNode, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(AttachQuestionToNode request, CancellationToken cancellationToken)
            {
                // Get existing page or create new
	            var quiz =
		            (await Db.Quizzes.AsNoTracking()
			            .Where(x => x.ContentTreeId == request.NodeId)
			            .FirstOrDefaultAsync(cancellationToken))

		            ?? (await Db.Quizzes.AddAsync(new QuizEntity
		            {
			            ContentTreeId = request.NodeId,
		            }, cancellationToken)).Entity;

	            var links = await Db.QuizQuestions
		            .Where(x => x.QuizId == quiz.QuizId)
		            .ToListAsync(cancellationToken);
	            
                // Delete no needed
	            Db.QuizQuestions.RemoveRange(links.Where(x => request.QuestionIds.All(y => y.Value != x.QuestionId)));

                // Update order for existing
                links.Where(x => request.QuestionIds.Any(y => y.Value == x.QuestionId)).ToList().ForEach(x =>
                {
	                var order = request.QuestionIds.First(m => m.Value == x.QuestionId).Key;
	                x.Order = order;
                });

                // Add new
                await Db.QuizQuestions.AddRangeAsync(request.QuestionIds.Where(x => links.All(y => y.QuestionId != x.Value))
	                .Select(x => new QuizQuestionEntity
	                {
		                QuizId = quiz.QuizId,
		                QuestionId = x.Value,
                        Order = x.Key
	                }), cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}