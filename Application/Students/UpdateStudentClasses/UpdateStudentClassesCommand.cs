using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.DbRequests;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.SchoolClass;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.UpdateStudentClasses
{
    [PublicAPI]
    public class UpdateStudentClassesCommand : ICommand<UpdateStudentClassesCommand.Result>
    {
        public Guid StudentId { get; set; }
        public Guid SchoolId { get; set; }
        public List<Guid> SchoolClassIds { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<UpdateStudentClassesCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleForEach(_ => _.SchoolClassIds).ExistsInTable(Db.SchoolClasses);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateStudentClassesCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(UpdateStudentClassesCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var student = _db.Students.Include(x => x.Schools).GetSingle(StudentSpec.ById(request.StudentId));
                if (student.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId))) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<UpdateStudentClassesCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UpdateStudentClassesCommand request, CancellationToken cancellationToken)
            {
                var student = await Db.Students.Include(_ => _.Schools).Include(_ => _.SchoolClasses).ThenInclude(x => x.SchoolClass).GetSingleAsync(StudentSpec.ById(request.StudentId), cancellationToken);
                var schoolClasses = await Db.SchoolClasses.Include(_ => _.School).Where(SchoolClassSpec.ByIds(request.SchoolClassIds)).ToListAsync(cancellationToken);
                if (schoolClasses.Select(_ => _.School).Any(_ => _.UseAccessCodes)) throw new ValidationException("This function is unavailable for schools that use Access Codes license type");

                if (schoolClasses.Any(_ => !student.IsInSchool(_.SchoolId)))
                    throw new SchoolClassIsAssignedToAnotherSchool();
                if(schoolClasses.Any(_ => _.SchoolId != schoolClasses[0].SchoolId))
	                throw new SchoolClassIsAssignedToAnotherSchool();

                var school = await Db.Schools.FirstAsync(_ => _.SchoolId == request.SchoolId);
                var newClasses = request.SchoolClassIds.Select(schoolClassId => new SchoolClassStudentEntity { StudentId = request.StudentId, SchoolClassId = schoolClassId });
                var classesToDelete = student.SchoolClasses.Where(_ => _.SchoolClass.SchoolId == school.SchoolId);

                //Check license
                if(newClasses.Count() - classesToDelete.Count() > 0 && Db.Students.CheckLicenseOverflowForSchool(school, newClasses.Count() - classesToDelete.Count()))
                    throw new NotEnoughStudentsLicensesException();

                Db.SchoolClassStudents.RemoveRange(classesToDelete);
                await Db.SchoolClassStudents.AddRangeAsync(newClasses, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }

        public class SchoolClassIsAssignedToAnotherSchool : BusinessLogicException
        {
            public override Guid ErrorCode => new("95EEB5EB-D0FC-441D-AFA1-92120AA25FBF");
        }
    }
}