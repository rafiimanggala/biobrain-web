using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.SchoolClass;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.SchoolClasses.CreateSchoolClass
{
    [PublicAPI]
    public class CreateSchoolClassCommand : ICommand<CreateSchoolClassCommand.Result>
    {
        public Guid SchoolId { get; init; }
        public Guid CourseId { get; init; }
        public int Year { get; init; }
        public string Name { get; init; }
        public ImmutableList<Guid> TeacherIds { get; init; }


        [PublicAPI]
        public class Result
        {
            public Guid SchoolClassId { get; set; }
        }


        internal class Validator : ValidatorBase<CreateSchoolClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
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

            private bool BeUniqueForSchoolAndYear(CreateSchoolClassCommand command, string name) => !Db.SchoolClasses
                                                                                                       .Where(SchoolClassSpec.ForSchool(command.SchoolId))
                                                                                                       .Where(SchoolClassSpec.ForYear(command.Year))
                                                                                                       .Any(SchoolClassSpec.WithName(name));
        }


        internal class PermissionCheck : PermissionCheckBase<CreateSchoolClassCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(CreateSchoolClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                if (user.IsSchoolAdmin(request.SchoolId)) return true;
                return false;
            }
        }


        internal class Handler : CommandHandlerBase<CreateSchoolClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(CreateSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = new SchoolClassEntity
                                  {
                                      SchoolId = request.SchoolId,
                                      Year = request.Year,
                                      Name = request.Name,
                                      CourseId = request.CourseId,
                                      AutoJoinClassCode = GetNewAutoJoinCode()
                                  };
                await Db.SchoolClasses.AddAsync(schoolClass, cancellationToken);
                await Db.SchoolClassTeachers
                        .AddRangeAsync(request.TeacherIds
                                              .Select(_ => new SchoolClassTeacherEntity
                                                           {
                                                               TeacherId = _,
                                                               SchoolClassId = schoolClass.SchoolClassId
                                                           }),
                                       cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);
                return new Result {SchoolClassId = schoolClass.SchoolClassId};
            }

            private static string GetNewAutoJoinCode() => $"{Guid.NewGuid():N}"[..6].ToUpperInvariant();
        }
    }
}
