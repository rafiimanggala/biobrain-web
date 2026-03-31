using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Content.FixNodesAttachedToDeleted
{
    [PublicAPI]
    public class DeleteDeletedCommand : ICommand<DeleteDeletedCommand.Result>
    {
	    [PublicAPI]
        public class Result
        {
	        public int DeletedMaterials { get; set; }
	        public int DeletedQuestions { get; set; }
        }


        internal class Validator : ValidatorBase<DeleteDeletedCommand>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck: PermissionCheckBase<DeleteDeletedCommand>
        {
            private readonly IDb db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => this.db = db;

            protected override bool CanExecute(DeleteDeletedCommand request, IUserSecurityInfo user) =>
                //ToDo Uncomment
                //if (user.IsApplicationAdmin()) return true;
                //return false;
                true;
        }


        internal class Handler : CommandHandlerBase<DeleteDeletedCommand, Result>
		{
			private readonly ILogger logger;

			public Handler(IDb db, ILogger<DeleteDeletedCommand> logger) : base(db) => this.logger = logger;

            public override async Task<Result> Handle(DeleteDeletedCommand request,
                                                      CancellationToken cancellationToken)
			{
				var subject = Constant.Subject.Physics;
				var materialsToDelete = await Db.PageMaterials
					.Include(x => x.Material)
					.Include(x => x.Page)
					.ThenInclude(x => x.ContentTreeNode)
					.ThenInclude(x => x.Course)
					.Where(x => x.Page.ContentTreeNode.Course.SubjectCode == subject && x.Material.DeletedAt != null)
					.ToListAsync(cancellationToken);
				
				var questionsToDelete = await Db.QuizQuestions.Include(x => x.Question)
					.Include(x => x.Quiz)
					.ThenInclude(x => x.ContentTreeNode)
					.ThenInclude(x => x.Course)
                    .Where(x => x.Quiz.ContentTreeNode.Course.SubjectCode == subject && x.Question.DeletedAt != null)
					.ToListAsync(cancellationToken);

				var data = new Result
					{DeletedMaterials = materialsToDelete.Count, DeletedQuestions = questionsToDelete.Count};
                Db.PageMaterials.RemoveRange(materialsToDelete);
                Db.QuizQuestions.RemoveRange(questionsToDelete);

                await Db.SaveChangesAsync(cancellationToken);

				return data;
			}

		}
    }
}
