using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.DbRequests;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.Student;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.JoinStudentToClass
{
    [PublicAPI]
    public class JoinStudentToClassCommand : ICommand<JoinStudentToClassCommand.Result>
    {
        public Guid StudentId { get; init; }
        public string ClassCode { get; init; }


        [PublicAPI]
        public class Result
        {
            public string ClassName { get; init; }
        }


        internal class Validator : ValidatorBase<JoinStudentToClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
	            RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
                RuleFor(_ => _.ClassCode).NotEmpty().ExistsInTable(Db.SchoolClasses, SchoolClassSpec.ByClassCode);
                RuleFor(_ => _).Must(_ =>
	                Db.SchoolClasses.Include(x => x.Students).Single(SchoolClassSpec.ByClassCode(_.ClassCode))
		                .Students.All(x => x.StudentId != _.StudentId)).WithMessage("You have already joined the class");
            }
        }


        internal class PermissionCheck : PermissionCheckBase<JoinStudentToClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(JoinStudentToClassCommand request, IUserSecurityInfo user)
            {
                if (!user.IsAccountOwner(request.StudentId)) return false;

                //var student = _db.Students.Include(_ => _.Schools).Include(x => x.SchoolClasses).GetSingle(StudentSpec.ById(request.StudentId));
                //var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ByClassCode(request.ClassCode));

                //return student.IsInSchool(schoolClass.SchoolId);
                return true;
            }
        }


        internal class Handler : CommandHandlerBase<JoinStudentToClassCommand, Result>
        {
            private readonly IJoinStudentToSchoolClassWithAccessCodeService _joinStudentToSchoolClassWithAccessCodeService;

            public Handler(IDb db, IJoinStudentToSchoolClassWithAccessCodeService joinStudentToSchoolClassWithAccessCodeService)
                : base(db)
                => _joinStudentToSchoolClassWithAccessCodeService = joinStudentToSchoolClassWithAccessCodeService;

            public override async Task<Result> Handle(JoinStudentToClassCommand request, CancellationToken cancellationToken)
            {
                var student = await Db.Students
                                      .Include(_ => _.SchoolClasses)
                                      .Include(_ => _.Schools).Include(x => x.SchoolClasses)
                                      .GetSingleAsync(StudentSpec.ById(request.StudentId), cancellationToken);

                var schoolClass = await Db.SchoolClasses
	                .Include(x => x.School)
	                .Where(SchoolClassSpec.ByClassCode(request.ClassCode))
	                .GetSingleAsync(cancellationToken);

                if (schoolClass.School.UseAccessCodes)
                {
                    await _joinStudentToSchoolClassWithAccessCodeService.Perform(schoolClass, student, cancellationToken);
                    return new Result { ClassName = schoolClass.Name };
                }

                await JoinStudentToSchoolClass(schoolClass, student, cancellationToken);

                return new Result{ ClassName = schoolClass.Name };
            }

            private async Task JoinStudentToSchoolClass(SchoolClassEntity schoolClass, StudentEntity student, CancellationToken cancellationToken)
            {
                //if(!student.IsInSchool(schoolClass.SchoolId))
                //    throw new ValidationException(Errors.StudentNotInSchool);

                if (Db.Students.CheckLicenseOverflowForSchool(schoolClass.School))
                    throw new NotEnoughStudentsLicensesException();

                var existingClass = student.SchoolClasses.FirstOrDefault(_ => _.SchoolClassId == schoolClass.SchoolClassId);
                if (existingClass != null)
                    return;

                var schoolClassStudents = new SchoolClassStudentEntity
                {
                    StudentId = student.StudentId,
                    SchoolClassId = schoolClass.SchoolClassId
                };

                if (!await Db.SchoolStudents.AnyAsync(_ => _.StudentId == student.StudentId && _.SchoolId == schoolClass.SchoolId, cancellationToken))
                    await Db.SchoolStudents.AddAsync(new SchoolStudentEntity { SchoolId = schoolClass.SchoolId, StudentId = student.StudentId }, cancellationToken);
                await Db.SchoolClassStudents.AddAsync(schoolClassStudents, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
