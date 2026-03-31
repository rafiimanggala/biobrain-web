using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using BiobrainWebAPI.Values;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Content.FixNodesAttachedToDeleted
{
    [PublicAPI]
    public class DeleteNodesAttachedForChemistryCommand : ICommand<DeleteNodesAttachedForChemistryCommand.Result>
    {
	    [PublicAPI]
        public class Result
        {
	        public int DeletedMaterials { get; set; }
	        public int DeletedQuestions { get; set; }
        }


        internal class Validator : ValidatorBase<DeleteNodesAttachedForChemistryCommand>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck: PermissionCheckBase<DeleteNodesAttachedForChemistryCommand>
        {
            private readonly IDb db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => this.db = db;

            protected override bool CanExecute(DeleteNodesAttachedForChemistryCommand request, IUserSecurityInfo user) =>
                //ToDo Uncomment
                //if (user.IsApplicationAdmin()) return true;
                //return false;
                true;
        }


        internal class Handler : CommandHandlerBase<DeleteNodesAttachedForChemistryCommand, Result>
		{
			private readonly ILogger logger;

			public Handler(IDb db, ILogger<DeleteNodesAttachedForChemistryCommand> logger) : base(db) => this.logger = logger;

            public override async Task<Result> Handle(DeleteNodesAttachedForChemistryCommand request,
                                                      CancellationToken cancellationToken)
			{
				var subject = Constant.Subject.Chemistry;
                var materialsToDelete = await Db.PageMaterials
					.Include(x => x.Material)
					.Include(x => x.Page)
					.ThenInclude(x => x.ContentTreeNode)
					.ThenInclude(x => x.Course)
					.Where(x => x.Page.ContentTreeNode.Course.SubjectCode == subject && !GenericCourses.Ids.Contains(x.Page.ContentTreeNode.CourseId))
					.ToListAsync(cancellationToken);
				
				var questionsToDelete = await Db.QuizQuestions.Include(x => x.Question)
					.Include(x => x.Quiz)
					.ThenInclude(x => x.ContentTreeNode)
					.ThenInclude(x => x.Course)
                    .Where(x => x.Quiz.ContentTreeNode.Course.SubjectCode == subject && !GenericCourses.Ids.Contains(x.Quiz.ContentTreeNode.CourseId))
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
