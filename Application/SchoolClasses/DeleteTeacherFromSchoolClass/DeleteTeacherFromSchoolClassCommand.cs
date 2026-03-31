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
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.SchoolClasses.DeleteTeacherFromSchoolClass
{
    [PublicAPI]
    public class DeleteTeacherFromSchoolClassCommand : ICommand<DeleteTeacherFromSchoolClassCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public Guid TeacherId { get; init; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteTeacherFromSchoolClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteTeacherFromSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(DeleteTeacherFromSchoolClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId) || user.IsSchoolTeacher(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<DeleteTeacherFromSchoolClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteTeacherFromSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses
                                          .Include(_ => _.Teachers)
                                          .GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);

                var teacherModel = await Db.Teachers.Include(x => x.Schools).Where(TeacherSpec.ById(request.TeacherId)).FirstOrDefaultAsync(cancellationToken);

                if (teacherModel.Schools.All(x => x.SchoolId != schoolClass.SchoolId))
                    throw new TeacherIsAssignedToAnotherSchoolException();

                var classTeacher = schoolClass.Teachers.FirstOrDefault(x => x.TeacherId == teacherModel.TeacherId);
                if (classTeacher == null)
	                return new Result();

                Db.SchoolClassTeachers.Remove(classTeacher);

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
