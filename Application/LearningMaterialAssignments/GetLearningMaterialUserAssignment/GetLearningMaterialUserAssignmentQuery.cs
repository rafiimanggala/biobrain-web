using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.LearningMaterialAssignments.GetLearningMaterialUserAssignment
{
    [PublicAPI]
    public sealed class GetLearningMaterialUserAssignmentQuery : IQuery<GetLearningMaterialUserAssignmentQuery.Result>
    {
        public Guid LearningMaterialUserAssignmentId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid LearningMaterialUserAssignmentId { get; init; }
            public Guid LearningMaterialAssignmentId { get; init; }
            public Guid ContentTreeNodeId { get; set; }
            public Guid AssignedToUserId { get; init; }
            public Guid? SchoolClassId { get; init; }
            public Guid? SchoolId { get; init; }
            public string SchoolName { get; init; }
            public DateTime? CompletedAtUtc { get; init; }
            public DateTime DueAtUtc { get; init; }
            public DateTime DueAtLocal { get; init; }
            public DateTime AssignedAtUtc { get; init; }
            public DateTime AssignedAtLocal { get; init; }
        }


        internal sealed class Validator : ValidatorBase<GetLearningMaterialUserAssignmentQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.LearningMaterialUserAssignmentId).ExistsInTable(Db.LearningMaterialUserAssignments);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetLearningMaterialUserAssignmentQuery>
        {
            private readonly IDb _db;
            private readonly ISessionContext _sessionContext;

            public PermissionCheck(ISecurityService securityService, IDb db, ISessionContext sessionContext)
                : base(securityService)
            {
                _db = db;
                _sessionContext = sessionContext;
            }

            protected override bool CanExecute(GetLearningMaterialUserAssignmentQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var assignedToUserId = _db.LearningMaterialUserAssignments
                                         .Where(LearningMaterialUserAssignmentSpec.ById(request.LearningMaterialUserAssignmentId))
                                         .Select(_ => _.AssignedToUserId)
                                         .GetSingle();
                return assignedToUserId == _sessionContext.GetUserId();
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetLearningMaterialUserAssignmentQuery, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<Result> Handle(GetLearningMaterialUserAssignmentQuery request, CancellationToken cancellationToken)
            {
                return Db.LearningMaterialUserAssignments
                    .Include(_ => _.LearningMaterialAssignment).ThenInclude(_ => _.SchoolClass).ThenInclude(_ => _.School)
                         .Where(LearningMaterialUserAssignmentSpec.ById(request.LearningMaterialUserAssignmentId))
                         .Select(_ => new Result
                                      {
                                          LearningMaterialUserAssignmentId = _.LearningMaterialUserAssignmentId,
                                          LearningMaterialAssignmentId = _.LearningMaterialAssignmentId,
                                          AssignedToUserId = _.AssignedToUserId,
                                          SchoolClassId = _.LearningMaterialAssignment.SchoolClassId,
                                          SchoolId = _.LearningMaterialAssignment.SchoolClass == null ? null : _.LearningMaterialAssignment.SchoolClass.SchoolId,
                                          SchoolName = _.LearningMaterialAssignment.SchoolClass == null ? null : _.LearningMaterialAssignment.SchoolClass.School.Name,
                                          CompletedAtUtc = _.CompletedAtUtc,
                                          DueAtUtc = _.DueAtUtc,
                                          DueAtLocal = _.DueAtLocal,
                                          AssignedAtUtc = _.AssignedAtUtc,
                                          AssignedAtLocal = _.AssignedAtLocal,
                                          ContentTreeNodeId = _.LearningMaterialAssignment.ContentTreeNodeId
                                      })
                         .GetSingleAsync(cancellationToken);
            }
        }
    }
}
