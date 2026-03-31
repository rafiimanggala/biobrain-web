using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Teachers.DeleteTeacher
{
    [PublicAPI]
    public class DeleteTeacherFromSchoolCommand : ICommand<DeleteTeacherFromSchoolCommand.Result>
    {
        public Guid TeacherId { get; set; }
        public Guid SchoolId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteTeacherFromSchoolCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteTeacherFromSchoolCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(DeleteTeacherFromSchoolCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                
                if (user.IsSchoolAdmin(request.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<DeleteTeacherFromSchoolCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteTeacherFromSchoolCommand request, CancellationToken cancellationToken)
            {
	            var teacherSchool = await Db.SchoolTeachers
		            .Where(_ => _.SchoolId == request.SchoolId && _.TeacherId == request.TeacherId)
		            .GetSingleAsync(cancellationToken);
	            if (teacherSchool == null) throw new ValidationException("Teacher not assigned to this school");
	            var classes = await Db.SchoolClassTeachers.Include(_ => _.SchoolClass)
		            .Where(_ => _.TeacherId == request.TeacherId && _.SchoolClass.SchoolId == request.SchoolId)
		            .ToListAsync(cancellationToken);
	            Db.SchoolTeachers.Remove(teacherSchool);
                Db.SchoolClassTeachers.RemoveRange(classes);
                //var teacher = await Db.Teachers.Include(x => x.User).GetSingleAsync(TeacherSpec.ById(request.TeacherId), cancellationToken);
                //Db.Teachers.Remove(teacher);
                //Db.Users.Remove(teacher.User);
                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
