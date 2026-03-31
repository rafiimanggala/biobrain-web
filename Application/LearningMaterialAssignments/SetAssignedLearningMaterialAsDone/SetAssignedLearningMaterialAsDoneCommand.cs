using System;
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


namespace Biobrain.Application.LearningMaterialAssignments.SetAssignedLearningMaterialAsDone
{
    [PublicAPI]
    public sealed class SetAssignedLearningMaterialAsDoneCommand : ICommand<SetAssignedLearningMaterialAsDoneCommand.Result>
    {
        public Guid LearningMaterialUserAssignmentId { get; init; }

        [PublicAPI]
        public record Result
        {
            public Guid LearningMaterialUserAssignmentId { get; init; }
            public DateTime CompletedAtUtc { get; init; }
        }


        internal sealed class Validator : ValidatorBase<SetAssignedLearningMaterialAsDoneCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.LearningMaterialUserAssignmentId).ExistsInTable(Db.LearningMaterialUserAssignments);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<SetAssignedLearningMaterialAsDoneCommand>
        {
            private readonly IDb _db;
            private readonly ISessionContext _sessionContext;

            public PermissionCheck(ISecurityService securityService, IDb db, ISessionContext sessionContext) : base(securityService)
            {
                _db = db;
                _sessionContext = sessionContext;
            }

            protected override bool CanExecute(SetAssignedLearningMaterialAsDoneCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var assignment = _db.LearningMaterialUserAssignments
                                    .GetSingle(LearningMaterialUserAssignmentSpec.ById(request.LearningMaterialUserAssignmentId));

                return assignment.AssignedToUserId == _sessionContext.GetUserId();
            }
        }


        internal sealed class Handler : CommandHandlerBase<SetAssignedLearningMaterialAsDoneCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(SetAssignedLearningMaterialAsDoneCommand request, CancellationToken cancellationToken)
            {
                var assignment = await Db.LearningMaterialUserAssignments
                                         .GetSingleAsync(LearningMaterialUserAssignmentSpec.ById(request.LearningMaterialUserAssignmentId),
                                                         cancellationToken);

                if (!assignment.CompletedAtUtc.HasValue)
                {
                    assignment.CompletedAtUtc = DateTime.UtcNow;
                    await Db.SaveChangesAsync(cancellationToken);
                }

                return new Result
                       {
                           LearningMaterialUserAssignmentId = assignment.LearningMaterialUserAssignmentId,
                           CompletedAtUtc = assignment.CompletedAtUtc.Value
                       };
            }
        }
    }
}
