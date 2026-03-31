using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.LearningMaterialAssignments.Services;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;


namespace Biobrain.Application.LearningMaterialAssignments.ReassignLearningMaterialsToStudents
{
    [PublicAPI]
    public class ReassignLearningMaterialsToStudentsCommand : ICommand<ReassignLearningMaterialsToStudentsCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public ImmutableList<Guid> StudentIds { get; init; }
        public ImmutableList<Guid> ContentTreeNodeIds { get; init; }
        public DateTime DueDateUtc { get; init; }
        public DateTime DueDateLocal { get; init; }
        public DateTime AssignedDateUtc { get; init; }
        public DateTime AssignedDateLocal { get; init; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<ReassignLearningMaterialsToStudentsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleForEach(_ => _.ContentTreeNodeIds).ExistsInTable(db.ContentTree);
                RuleForEach(_ => _.StudentIds).ExistsInTable(db.Students);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<ReassignLearningMaterialsToStudentsCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(ReassignLearningMaterialsToStudentsCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;


                var schoolId = _db.SchoolClasses.Where(SchoolClassSpec.ById(request.SchoolClassId)).Select(_ => _.SchoolId).FirstOrDefault();

                return user.IsSchoolTeacher(schoolId) || user.IsSchoolAdmin(schoolId);
            }
        }


        internal class Handler : CommandHandlerBase<ReassignLearningMaterialsToStudentsCommand, Result>
        {
            private readonly IAssignLearningMaterialService _assignLearningMaterialService;
            private readonly IAssignLearningMaterialsNotificationService _assignLearningMaterialsNotificationService;

            public Handler(IDb db,
                           IAssignLearningMaterialService assignLearningMaterialService,
                           IAssignLearningMaterialsNotificationService assignLearningMaterialsNotificationService)
                : base(db)
            {
                _assignLearningMaterialService = assignLearningMaterialService;
                _assignLearningMaterialsNotificationService = assignLearningMaterialsNotificationService;
            }

            public override async Task<Result> Handle(ReassignLearningMaterialsToStudentsCommand request, CancellationToken cancellationToken)
            {
                var assignParams = new IAssignLearningMaterialService.Params
                                   {
                                       StudentIds = request.StudentIds ?? ImmutableList.Create<Guid>(),
                                       SchoolClassId = request.SchoolClassId,
                                       ContentTreeNodeIds = request.ContentTreeNodeIds,
                                       DueDateUtc = request.DueDateUtc,
                                       DueDateLocal = request.DueDateLocal,
                                       AssignedDateUtc = request.AssignedDateUtc,
                                       AssignedDateLocal = request.AssignedDateLocal
                                   };
                var assignResult = await _assignLearningMaterialService.Assign(assignParams, true, cancellationToken);
                await _assignLearningMaterialsNotificationService.Send(assignResult, request.SchoolClassId, request.DueDateLocal, cancellationToken);
                
                return new Result();
            }
        }
    }
}
