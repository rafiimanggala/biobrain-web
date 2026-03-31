using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.SchoolClasses.EmailSchoolClass
{
    [PublicAPI]
    public class EmailSchoolClassCommand : ICommand<EmailSchoolClassCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public string EmailText { get; init; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<EmailSchoolClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.EmailText).NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<EmailSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(EmailSchoolClassCommand request, IUserSecurityInfo user)
            {
                //if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId) || user.IsSchoolTeacher(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<EmailSchoolClassCommand, Result>
        {
            private readonly INotificationService _notificationService;
            private readonly ISessionContext _sessionContext;
            public Handler(IDb db, INotificationService notificationService, ISessionContext sessionContext) : base(db)
            {
	            _notificationService = notificationService;
	            _sessionContext = sessionContext;
            }

            public override async Task<Result> Handle(EmailSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses.AsNoTracking()
	                .Include(_ => _.Students).ThenInclude(_ => _.Student).ThenInclude(_ => _.User)
                    .Include(_ => _.Course).ThenInclude(_ => _.Subject)
                    .GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);
                var teacher = await Db.Users.AsNoTracking()
                    .Include(_ => _.Teacher)
	                .Where(UserSpec.ById(_sessionContext.GetUserId()))
	                .FirstOrDefaultAsync(cancellationToken);

                foreach (var student in schoolClass.Students.Select(_ => _.Student))
                {
	                await _notificationService.Send(new EmailClassNotification(student.User.Email, student.User.GetFirstName(), request.EmailText, teacher.GetFullName(), schoolClass.Name, schoolClass.Course.Subject.Symbol));
                }

                return new Result();
            }
        }


        public class TeacherIsAssignedToAnotherSchoolException : BusinessLogicException
        {
            public override Guid ErrorCode => new("C114B241-D5FC-4EFF-87B9-3274988A0F48");
        }
    }
}
