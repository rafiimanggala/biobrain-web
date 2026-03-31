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
using Biobrain.Domain.Entities.SchoolClass;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.SchoolClasses.AddTeacherToSchoolClass
{
    [PublicAPI]
    public class AddTeacherToSchoolClassCommand : ICommand<AddTeacherToSchoolClassCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public Guid TeacherId { get; init; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<AddTeacherToSchoolClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<AddTeacherToSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(AddTeacherToSchoolClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId) || user.IsSchoolTeacher(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<AddTeacherToSchoolClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(AddTeacherToSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses
                                          .Include(_ => _.Teachers)
                                          .GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);

                var teacher = await Db.Teachers.Include(x => x.Schools).Where(TeacherSpec.ById(request.TeacherId)).FirstOrDefaultAsync(cancellationToken);

                if (teacher.Schools.All(x => x.SchoolId != schoolClass.SchoolId))
                    throw new TeacherIsAssignedToAnotherSchoolException();

                if(schoolClass.Teachers.Any(x => x.TeacherId == teacher.TeacherId))
	                return new Result();

                await Db.SchoolClassTeachers.AddAsync(new SchoolClassTeacherEntity
                {
	                TeacherId = teacher.TeacherId,
	                SchoolClassId = schoolClass.SchoolClassId
                }, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }


        public class TeacherIsAssignedToAnotherSchoolException : BusinessLogicException
        {
            public override Guid ErrorCode => new("21337E21-EFDE-468D-9D93-84BBEE57F223");
        }
    }
}
