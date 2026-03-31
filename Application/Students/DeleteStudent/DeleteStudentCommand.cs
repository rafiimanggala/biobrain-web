using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.DeleteStudent
{
    [PublicAPI]
    public class DeleteStudentFromSchoolCommand : ICommand<DeleteStudentFromSchoolCommand.Result>
    {
        public Guid StudentId { get; set; }
        public Guid SchoolId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteStudentFromSchoolCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteStudentFromSchoolCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(DeleteStudentFromSchoolCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                //var student = _db.Students.GetSingle(StudentSpec.ById(request.StudentId));
                //if (student.SchoolId.HasValue && user.IsSchoolAdmin(student.SchoolId.Value)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<DeleteStudentFromSchoolCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteStudentFromSchoolCommand request, CancellationToken cancellationToken)
            {
                var studentSchool = await Db.SchoolStudents.Where(_ => _.SchoolId == request.SchoolId && _.StudentId == request.StudentId).GetSingleAsync(cancellationToken);
                if(studentSchool == null) return new Result();
                Db.SchoolStudents.Remove(studentSchool);
                var classes = await Db.SchoolClassStudents.Include(_ => _.SchoolClass)
	                .Where(_ => _.StudentId == request.StudentId && _.SchoolClass.SchoolId == request.SchoolId)
	                .ToListAsync(cancellationToken);
                Db.SchoolClassStudents.RemoveRange(classes);

                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }
    }
}