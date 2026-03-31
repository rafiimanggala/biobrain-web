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

namespace Biobrain.Application.LearningMaterialAssignments.UpdateDueDateForLearningMaterialAssignment
{
    [PublicAPI]
    public sealed class UpdateDueDateForLearningMaterialAssignmentCommand : ICommand<UpdateDueDateForLearningMaterialAssignmentCommand.Result>
    {
            public Guid LearningMaterialAssignmentId { get; set; }
            public DateTime DueDateUtc { get; init; }
            public DateTime DueDateLocal { get; init; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<UpdateDueDateForLearningMaterialAssignmentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.LearningMaterialAssignmentId).ExistsInTable(Db.LearningMaterialAssignments);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<UpdateDueDateForLearningMaterialAssignmentCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db) 
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(UpdateDueDateForLearningMaterialAssignmentCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;
                
                var assignment = _db.LearningMaterialAssignments.Include(_ => _.SchoolClass).Where(LearningMaterialAssignmentSpec.ById(request.LearningMaterialAssignmentId)).GetSingle();

                return user.IsSchoolTeacher(assignment.SchoolClass.SchoolId) || user.IsSchoolAdmin(assignment.SchoolClass.SchoolId);
            }
        }


        internal sealed class Handler : CommandHandlerBase<UpdateDueDateForLearningMaterialAssignmentCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UpdateDueDateForLearningMaterialAssignmentCommand request, CancellationToken cancellationToken)
            {
                var assignment = Db.LearningMaterialAssignments.Include(_ => _.UserAssignments).Where(LearningMaterialAssignmentSpec.ById(request.LearningMaterialAssignmentId)).GetSingle();
                assignment.DueAtUtc = request.DueDateUtc;
                assignment.DueAtLocal = request.DueDateLocal;
                foreach (var studentAssignment in assignment.UserAssignments)
                {
                    studentAssignment.DueAtUtc = request.DueDateUtc;
                    studentAssignment.DueAtLocal = request.DueDateLocal;
                }
                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }

        }
    }
}
