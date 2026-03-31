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

namespace Biobrain.Application.LearningMaterialAssignments.UnassignLearningMaterialToClass
{
    [PublicAPI]
    public sealed class UnassignLearningMaterialToClass : ICommand<UnassignLearningMaterialToClass.Result>
    {
            public Guid LearningMaterialAssignmentId { get; init; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<UnassignLearningMaterialToClass>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.LearningMaterialAssignmentId).ExistsInTable(Db.LearningMaterialAssignments);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<UnassignLearningMaterialToClass>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db) 
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(UnassignLearningMaterialToClass request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;
                
                var assignment = _db.LearningMaterialAssignments.Include(_ => _.SchoolClass).Where(LearningMaterialAssignmentSpec.ById(request.LearningMaterialAssignmentId)).GetSingle();

                return user.IsSchoolTeacher(assignment.SchoolClass.SchoolId) || user.IsSchoolAdmin(assignment.SchoolClass.SchoolId);
            }
        }


        internal sealed class Handler : CommandHandlerBase<UnassignLearningMaterialToClass, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UnassignLearningMaterialToClass request, CancellationToken cancellationToken)
            {
                var assignment = Db.LearningMaterialAssignments.Include(_ => _.SchoolClass).Where(LearningMaterialAssignmentSpec.ById(request.LearningMaterialAssignmentId)).GetSingle();
                Db.LearningMaterialAssignments.Remove(assignment);
                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }

        }
    }
}
