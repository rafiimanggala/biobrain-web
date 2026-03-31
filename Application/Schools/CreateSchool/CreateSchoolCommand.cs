using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.Schools.CreateSchool
{
    [PublicAPI]
    public class CreateSchoolCommand : ICommand<CreateSchoolCommand.Result>
    {
        public string Name { get; set; }
        public bool UseAccessCodes { get; set; }
        public Constant.SchoolStatus Status { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public List<Guid> Courses { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid SchoolId { get; set; }
        }


        internal class Validator : ValidatorBase<CreateSchoolCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Name).NotEmpty().Unique(Db.Schools, SchoolSpec.WithName);
                RuleForEach(_ => _.Courses).ExistsInTable(db.Courses);
            }
        }


        internal class PermissionCheck: PermissionCheckBase<CreateSchoolCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(CreateSchoolCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<CreateSchoolCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
            {
                var school = new SchoolEntity
                             {
                                 Name = request.Name,
                                 TeachersLicensesNumber = 50,
                                 StudentsLicensesNumber = 5000,
                                 Status = request.Status,
                                 UseAccessCodes = request.UseAccessCodes,
                                 EndDateUtc = request.EndDateUtc
                };

                var entry = await Db.Schools.AddAsync(school, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);
               
                request.Courses.ForEach(_ => Db.SchoolCourses.Add(new SchoolCourseEntity
                {
                    CourseId = _,
                    SchoolId = entry.Entity.SchoolId,
                }));
                await Db.SaveChangesAsync(cancellationToken);

                return new Result {SchoolId = entry.Entity.SchoolId};
            }
        }
    }
}