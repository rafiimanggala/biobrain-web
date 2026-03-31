using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.SchoolClasses.MoveStudentToSchoolClass
{
    [PublicAPI]
    public class MoveStudentToSchoolClassCommand : ICommand<MoveStudentToSchoolClassCommand.Result>
    {
        public Guid OldSchoolClassId { get; init; }
        public Guid NewSchoolClassId { get; init; }
        public Guid StudentId { get; set; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<MoveStudentToSchoolClassCommand>
        {
            public Validator(IDb db, UserManager<UserEntity> userManager) : base(db)
            {
                RuleFor(_ => _.OldSchoolClassId).ExistsInTable(Db.SchoolClasses, SchoolClassSpec.ById);
                RuleFor(_ => _.NewSchoolClassId).ExistsInTable(Db.SchoolClasses, SchoolClassSpec.ById);
                RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<MoveStudentToSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(MoveStudentToSchoolClassCommand request, IUserSecurityInfo user)
            {
                //if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.OldSchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId) || user.IsSchoolTeacher(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<MoveStudentToSchoolClassCommand, Result>
        {
            private readonly INotificationService _notificationService;
            private readonly ISessionContext _sessionContext;
            private readonly ISiteUrls _siteUrls;
            public Handler(IDb db, INotificationService notificationService, ISessionContext sessionContext, ISiteUrls siteUrls) : base(db)
            {
	            _notificationService = notificationService;
	            _sessionContext = sessionContext;
	            _siteUrls = siteUrls;
            }

            public override async Task<Result> Handle(MoveStudentToSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses.AsNoTracking()
                    .GetSingleAsync(SchoolClassSpec.ById(request.NewSchoolClassId), cancellationToken);

                var student = await Db.Students.AsNoTracking()
	                .GetSingleAsync(StudentSpec.ById(request.StudentId), cancellationToken);

                var oldLink = await Db.SchoolClassStudents
	                .Where(x => x.SchoolClassId == request.OldSchoolClassId && x.StudentId == request.StudentId)
	                .FirstOrDefaultAsync(cancellationToken);
                if (oldLink == null) throw new ValidationException("Student not exist in source class");

                Db.SchoolClassStudents.Remove(oldLink);
                if (await Db.SchoolClassStudents.AnyAsync(
	                x => x.StudentId == request.StudentId && x.SchoolClassId == request.NewSchoolClassId,
	                cancellationToken))
	                throw new ValidationException("Student already in class");

                await Db.SchoolClassStudents.AddAsync(new SchoolClassStudentEntity{SchoolClassId = request.NewSchoolClassId, StudentId = request.StudentId}, cancellationToken);

                // ToDo: Update assignments and results
                //var assignments = await Db.QuizAssignments
	               // .Include(_ => _.QuizStudentAssignments)
	               // .Where(QuizAssignmentSpec.ForClass(request.OldSchoolClassId))
	               // .Where(x => x.)
	               // //.Where(QuizAssignmentSpec.ForCourse(request.CourseId))
	               // .ToListAsync(cancellationToken);

                // ToDO: Add notification?
                //await _notificationService.Send(new InviteByEmailNotification(request.Email, teacher.GetFullName(), $"{schoolClass.Year} {schoolClass.Name}", schoolClass.AutoJoinClassCode, _siteUrls.Login()));

                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }


        public class TeacherIsAssignedToAnotherSchoolException : BusinessLogicException
        {
            public override Guid ErrorCode => new("BED4396C-65C2-4C7E-91E2-18EAB8F4C2D6");
        }
    }
}
