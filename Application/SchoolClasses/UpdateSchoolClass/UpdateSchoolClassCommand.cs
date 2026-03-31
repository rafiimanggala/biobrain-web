using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.SchoolClass;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.SchoolClasses.UpdateSchoolClass
{
    [PublicAPI]
    public class UpdateSchoolClassCommand : ICommand<UpdateSchoolClassCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public int Year { get; init; }
        public string Name { get; init; }
        public ImmutableList<Guid> TeacherIds { get; init; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<UpdateSchoolClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.Year).InclusiveBetween(9, 12);
                RuleFor(_ => _.Name).NotEmpty().Must(BeUniqueForSchoolAndYear);
                RuleFor(_ => _.TeacherIds).Must(TeachersBeEmptyOrExistsInTable).WithMessage("One or more teachers don't exist");
            }

            private bool TeachersBeEmptyOrExistsInTable(ImmutableList<Guid> teacherIds)
            {
                if (teacherIds is null)
                    return true;

                if (teacherIds.IsEmpty)
                    return true;

                var teachersInDbCount = Db.Teachers
                                          .Where(TeacherSpec.ByIds(teacherIds))
                                          .Count();

                return teachersInDbCount == teacherIds.Count;
            }
            
            private bool BeUniqueForSchoolAndYear(UpdateSchoolClassCommand request, string name)
            {
                var schoolClass = Db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));

                return !Db.SchoolClasses
                          .Where(SchoolClassSpec.OtherSchoolClasses(request.SchoolClassId))
                          .Where(SchoolClassSpec.ForSchool(schoolClass.SchoolId))
                          .Where(SchoolClassSpec.ForYear(request.Year))
                          .Any(SchoolClassSpec.WithName(name));
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(UpdateSchoolClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<UpdateSchoolClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UpdateSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses
                                          .Include(_ => _.Teachers)
                                          .GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);

                schoolClass.Year = request.Year;
                schoolClass.Name = request.Name;

                var teachers = await Db.Teachers
	                .Include(x => x.Schools)
	                .Where(TeacherSpec.ByIds(request.TeacherIds))
	                .ToListAsync(cancellationToken);

                if (teachers.Any(_ => !_.IsInSchool(schoolClass.SchoolId)))
                    throw new TeacherIsAssignedToAnotherSchoolException();

                var newTeachers = request.TeacherIds
                                         .Select(_ => new SchoolClassTeacherEntity
                                                      {
                                                          TeacherId = _,
                                                          SchoolClassId = schoolClass.SchoolClassId
                                                      });

                Db.SchoolClassTeachers.RemoveRange(schoolClass.Teachers);
                await Db.SchoolClassTeachers.AddRangeAsync(newTeachers, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }


        public class TeacherIsAssignedToAnotherSchoolException : BusinessLogicException
        {
            public override Guid ErrorCode => new("C114B241-D5FC-4EFF-87B9-3274988A0F48");
        }
    }
}
