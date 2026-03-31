using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Teachers.AddTeacherByEmail
{
    [PublicAPI]
    public partial class AddTeacherByEmailCommand : ICommand<AddTeacherByEmailCommand.Result>
    {
        public Guid SchoolId { get; set; }
        public string Email { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<AddTeacherByEmailCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.Email).NotEmpty().EmailAddress().ExistsInTable(Db.Users, UserSpec.WithLoginNameOrEmail);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<AddTeacherByEmailCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(AddTeacherByEmailCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                if (user.IsSchoolAdmin(request.SchoolId)) return true;
                return false;
            }
        }


        internal class Handler : CommandHandlerBase<AddTeacherByEmailCommand, Result>
        {
            private readonly UserManager<UserEntity> _userManager;
            private readonly INotificationService _notificationService;
            private readonly ISiteUrls _siteUrls;
            public Handler(IDb db,  UserManager<UserEntity> userManager, INotificationService notificationService, ISiteUrls siteUrls) : base(db)
            {
                _userManager = userManager;
                _notificationService = notificationService;
                _siteUrls = siteUrls;
            }

            public override async Task<Result> Handle(AddTeacherByEmailCommand request, CancellationToken cancellationToken)
            {
                var school = await Db.Schools.GetSingleAsync(SchoolSpec.ById(request.SchoolId), cancellationToken);
                if (Db.Teachers.Include(x => x.Schools).Count(TeacherSpec.ForSchool(request.SchoolId)) + 1 > school.TeachersLicensesNumber)
                    throw new NotEnoughTeachersLicensesException();

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) throw new ValidationException("User with such email not exist");

                var teacher = await Db.Teachers.GetSingleAsync(TeacherSpec.ById(user.Id), cancellationToken);
                if (teacher == null) throw new ValidationException("User with such email not teacher");

                await Db.SchoolTeachers.AddAsync(new SchoolTeacherEntity{SchoolId = request.SchoolId, TeacherId = teacher.TeacherId}, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
