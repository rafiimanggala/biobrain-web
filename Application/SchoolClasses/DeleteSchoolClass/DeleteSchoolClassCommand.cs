using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.SchoolClasses.DeleteSchoolClass
{
    [PublicAPI]
    public class DeleteSchoolClassCommand : ICommand<DeleteSchoolClassCommand.Result>
    {
        public Guid SchoolClassId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteSchoolClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(DeleteSchoolClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<DeleteSchoolClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses.GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);
                var classAssignments = await Db.LearningMaterialAssignments.Where(LearningMaterialAssignmentSpec.ByClassId(request.SchoolClassId)).ToListAsync(cancellationToken);
                Db.LearningMaterialAssignments.RemoveRange(classAssignments);
                Db.SchoolClasses.Remove(schoolClass);

                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }
    }
}