using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Schools.UpdateSchoolDetails
{
    [PublicAPI]
    public class UpdateSchoolDetailsCommand : ICommand<UpdateSchoolDetailsCommand.Result>
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; }
        public bool UseAccessCodes { get; set; }
        public Constant.SchoolStatus Status { get; init; }
        public DateTime? EndDateUtc { get; set; }
        public bool AiDisabled { get; set; }
        public List<Guid> Courses { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid SchoolId { get; set; }
            public string Name { get; set; }
            public Constant.SchoolStatus Status { get; init; }
        }


        internal class Validator : ValidatorBase<UpdateSchoolDetailsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.Name).NotEmpty().Must((r, name) => !Db.Schools.Where(SchoolSpec.OtherSchools(r.SchoolId)).Any(SchoolSpec.WithName(name))).WithMessage("'Name' must be unique.");
                RuleForEach(_ => _.Courses).ExistsInTable(Db.Courses);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateSchoolDetailsCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(UpdateSchoolDetailsCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                if (user.IsSchoolAdmin(request.SchoolId)) return true;
                return false;
            }
        }


        internal class Handler: CommandHandlerBase<UpdateSchoolDetailsCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UpdateSchoolDetailsCommand request, CancellationToken cancellationToken)
            {
                var school = await Db.Schools.GetSingleAsync(SchoolSpec.ById(request.SchoolId), cancellationToken);
                school.Name = request.Name;
                school.Status = request.Status;
                school.UseAccessCodes = request.UseAccessCodes;
                school.AiDisabled = request.AiDisabled;
                school.EndDateUtc = request.EndDateUtc;

                var courses = await Db.SchoolCourses.Where(_ => _.SchoolId == request.SchoolId)
                    .ToListAsync(cancellationToken);
                Db.SchoolCourses.RemoveRange(courses);
                request.Courses.ForEach(_ => Db.SchoolCourses.Add(new SchoolCourseEntity
                {
                    CourseId = _,
                    SchoolId = request.SchoolId,
                }));

                await Db.SaveChangesAsync(cancellationToken);
                return new Result {SchoolId = school.SchoolId, Name = school.Name, Status = school.Status};
            }
        }
    }
}
