using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.SchoolClasses.RenameSchoolClass
{
    [PublicAPI]
    public class RenameSchoolClassCommand : ICommand<RenameSchoolClassCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public string Name { get; init; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<RenameSchoolClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
                RuleFor(_ => _.Name).NotEmpty().Must(BeUniqueForSchoolAndYear).WithMessage("Class name must be uniq for class year");
            }

            private bool BeUniqueForSchoolAndYear(RenameSchoolClassCommand request, string name)
            {
                var schoolClass = Db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));

                return !Db.SchoolClasses
                          .Where(SchoolClassSpec.OtherSchoolClasses(request.SchoolClassId))
                          .Where(SchoolClassSpec.ForSchool(schoolClass.SchoolId))
                          .Where(SchoolClassSpec.ForYear(schoolClass.Year))
                          .Any(SchoolClassSpec.WithName(name));
            }
        }


        internal class PermissionCheck : PermissionCheckBase<RenameSchoolClassCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(RenameSchoolClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId) || user.IsSchoolTeacher(schoolClass.SchoolId)) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<RenameSchoolClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(RenameSchoolClassCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses
                                          .GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);
                
                schoolClass.Name = request.Name;
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
