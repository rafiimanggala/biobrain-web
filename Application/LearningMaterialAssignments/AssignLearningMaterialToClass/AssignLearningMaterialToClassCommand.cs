using System;
using System.Collections.Generic;
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
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.LearningMaterialAssignments.AssignLearningMaterialToClass
{
    [PublicAPI]
    public class AssignLearningMaterialToClassCommand : ICommand<AssignLearningMaterialToClassCommand.Result>
    {
        public ImmutableDictionary<Guid, ImmutableList<Guid>> StudentIdsBySchoolClassIdMap { get; init; }
        public ImmutableList<Guid> ContentTreeNodeIds { get; init; }
        public DateTime DueDateUtc { get; init; }
        public DateTime DueDateLocal { get; init; }
        public DateTime AssignedDateUtc { get; init; }
        public DateTime AssignedDateLocal { get; init; }
        public bool ForceCreateNew { get; init; } = false;


        [PublicAPI]
        public class Result
        {
            public ImmutableList<Guid> NotAssignedNodeIds { get; init; }
        }


        internal class Validator : ValidatorBase<AssignLearningMaterialToClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleForEach(_ => _.StudentIdsBySchoolClassIdMap.Keys).ExistsInTable(Db.SchoolClasses).OverridePropertyName("SchoolClasses");
                RuleForEach(_ => _.StudentIdsBySchoolClassIdMap.Values.SelectMany(x => x)).ExistsInTable(db.Students).OverridePropertyName("Students");
                RuleForEach(_ => _.ContentTreeNodeIds).ExistsInTable(Db.ContentTree);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<AssignLearningMaterialToClassCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(AssignLearningMaterialToClassCommand request, IUserSecurityInfo user)
            {
	            if (user.IsApplicationAdmin())
		            return true;

	            var schoolId = _db.SchoolClasses.Where(SchoolClassSpec.ByIds(request.StudentIdsBySchoolClassIdMap.Keys))
                                  .Select(_ => _.SchoolId)
                                  .FirstOrDefault();

	            return user.IsSchoolTeacher(schoolId) || user.IsSchoolAdmin(schoolId);
            }
        }


        internal class Handler : CommandHandlerBase<AssignLearningMaterialToClassCommand, Result>
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

            public override async Task<Result> Handle(AssignLearningMaterialToClassCommand request, CancellationToken cancellationToken)
            {
                HashSet<Guid> notAssignedNodeIds = new();

                foreach ((Guid schoolClassId, ImmutableList<Guid> studentIds) in request.StudentIdsBySchoolClassIdMap)
                {
                    var assignParams = new IAssignLearningMaterialService.Params
                                       {
                                           StudentIds = studentIds,
                                           SchoolClassId = schoolClassId,
                                           ContentTreeNodeIds = request.ContentTreeNodeIds,
                                           DueDateUtc = request.DueDateUtc,
                                           DueDateLocal = request.DueDateLocal,
                                           AssignedDateUtc = request.AssignedDateUtc,
                                           AssignedDateLocal = request.AssignedDateLocal
                                       };
                    var assignResult = await _assignLearningMaterialService.Assign(assignParams, request.ForceCreateNew, cancellationToken);
                    await _assignLearningMaterialsNotificationService.Send(assignResult, schoolClassId, request.DueDateLocal, cancellationToken);
                    notAssignedNodeIds.UnionWith(assignResult.NotAssignedNodeIds);
                }

                return new Result { NotAssignedNodeIds = notAssignedNodeIds.ToImmutableList() };
            }
        }
    }
}
