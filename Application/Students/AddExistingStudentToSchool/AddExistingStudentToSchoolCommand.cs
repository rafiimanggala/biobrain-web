using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.AddExistingStudentToSchool
{
    [PublicAPI]
    public class AddExistingStudentToSchoolCommand : ICommand<AddExistingStudentToSchoolCommand.Result>
    {
        public Guid SchoolId { get; init; }
        public string Email { get; init; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<AddExistingStudentToSchoolCommand>
        {
            public Validator(IDb db, UserManager<UserEntity> userManager) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                //RuleFor(_ => _.Email).MustAsync(async (command, _, _) => (await userManager.FindUserByLoginName(command.Email)) == null).WithErrorCode("User with this email already exist");
            }
        }


        internal class PermissionCheck : PermissionCheckBase<AddExistingStudentToSchoolCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(AddExistingStudentToSchoolCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                if (user.IsSchoolAdmin(request.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<AddExistingStudentToSchoolCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(AddExistingStudentToSchoolCommand request, CancellationToken cancellationToken)
            {
                var user = await Db.Users.AsNoTracking()
	                .Include(x => x.Student)
                    .Include(x => x.Teacher)
                    .Where(UserSpec.WithLoginNameOrEmail(request.Email))
	                .FirstOrDefaultAsync(cancellationToken);

                if(user == null)
                    throw new ValidationException($"User {request.Email} is not registered.");

                // Add user to school if not in yet
                if (user.Student != null)
                {
	                var userSchool = await Db.SchoolStudents.AsNoTracking()
		                .Where(x => x.SchoolId == request.SchoolId && x.StudentId == user.Id)
		                .FirstOrDefaultAsync(cancellationToken);
	                if (userSchool == null)
	                {
		                await Db.SchoolStudents.AddAsync(
			                new SchoolStudentEntity {SchoolId = request.SchoolId, StudentId = user.Id},
			                cancellationToken);
		                await Db.SaveChangesAsync(cancellationToken);
	                }

                    return new Result();
                }

                if (user.Teacher != null)
                {
                    throw new ValidationException("This user already registered as teacher.");
                }

                return new Result();
            }
        }


        public class TeacherIsAssignedToAnotherSchoolException : BusinessLogicException
        {
            public override Guid ErrorCode => new("BED4396C-65C2-4C7E-91E2-18EAB8F4C2D6");
        }
    }
}
