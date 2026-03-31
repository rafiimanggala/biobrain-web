using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.RemoveStudentClasses
{
    [PublicAPI]
    public class RemoveStudentClassCommand : ICommand<RemoveStudentClassCommand.Result>
    {
        public Guid StudentId { get; set; }
        public Guid SchoolClassId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<RemoveStudentClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => new {_.SchoolClassId, _.StudentId}).Custom((data, context) =>
	                Db.SchoolClassStudents.Any(sc =>
		                sc.StudentId == data.StudentId && sc.SchoolClassId == data.SchoolClassId));
            }
        }


        internal class PermissionCheck : PermissionCheckBase<RemoveStudentClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(RemoveStudentClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId) || user.IsSchoolTeacher(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<RemoveStudentClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(RemoveStudentClassCommand request, CancellationToken cancellationToken)
            {
	            var entity = await Db.SchoolClassStudents.Where(_ =>
			            _.SchoolClassId == request.SchoolClassId && _.StudentId == request.StudentId)
		            .FirstOrDefaultAsync(cancellationToken);
	            Db.Remove(entity);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }

        public class SchoolClassIsAssignedToAnotherSchool : BusinessLogicException
        {
            public override Guid ErrorCode => new("95EEB5EB-D0FC-441D-AFA1-92120AA25FBF");
        }
    }
}