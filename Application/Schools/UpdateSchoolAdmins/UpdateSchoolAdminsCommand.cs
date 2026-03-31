using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Models;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Schools.UpdateSchoolAdmins
{
    [PublicAPI]
    public class UpdateSchoolAdminsCommand : ICommand<UpdateSchoolAdminsCommand.Result>
    {
        public Guid SchoolId { get; set; }
        public CollectionUpdateModel<Guid> Teachers { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<UpdateSchoolAdminsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.Teachers).Must(BeTeacherOfSchool);
            }

            private bool BeTeacherOfSchool(UpdateSchoolAdminsCommand command, CollectionUpdateModel<Guid> collectionUpdate)
            {
                var schoolTeachers = Db.Teachers
	                .Include(x => x.Schools)
                                       .Where(TeacherSpec.ForSchool(command.SchoolId))
                                       .Where(TeacherSpec.ByIds(collectionUpdate.All))
                                       .Select(_ => _.TeacherId).ToHashSet();

                return collectionUpdate.All.All(_ => schoolTeachers.Contains(_));
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateSchoolAdminsCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(UpdateSchoolAdminsCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<UpdateSchoolAdminsCommand, Result>
        {
	        private readonly UserManager<UserEntity> userManager;
            public Handler(IDb db, UserManager<UserEntity> userManager) : base(db) => this.userManager = userManager;

            public override async Task<Result> Handle(UpdateSchoolAdminsCommand request, CancellationToken cancellationToken)
            {
                var schoolAdmins = await Db.SchoolAdmins
                                           .Where(SchoolAdminSpec.ForSchool(request.SchoolId))
                                           .Where(SchoolAdminSpec.ForTeachers(request.Teachers.All))
                                           .ToListAsync(cancellationToken);

                var addSchoolAdmins = request.Teachers.ToAdd
                                             .Where(_ => schoolAdmins.All(a => a.TeacherId != _))
                                             .Select(_ => new SchoolAdminEntity {SchoolId = request.SchoolId, TeacherId = _})
                                             .ToList();
                await Db.SchoolAdmins.AddRangeAsync(addSchoolAdmins, cancellationToken);
                foreach (var schoolAdmin in addSchoolAdmins)
                {
	                var user = await userManager.GetUserById(schoolAdmin.TeacherId);
	                await userManager.AddToRoleAsync(user, Constant.Roles.SchoolAdministrator);
                }
                
                var removeSchoolAdmins = schoolAdmins.Where(_ => request.Teachers.ToRemove.Contains(_.TeacherId));
                Db.SchoolAdmins.RemoveRange(removeSchoolAdmins);
                foreach (var schoolAdmin in removeSchoolAdmins)
                {
	                var user = await userManager.GetUserById(schoolAdmin.TeacherId);
	                await userManager.RemoveFromRoleAsync(user, Constant.Roles.SchoolAdministrator);
                }

                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }
    }
}