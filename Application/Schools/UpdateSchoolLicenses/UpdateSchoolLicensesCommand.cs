using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.Schools.UpdateSchoolLicenses
{
    [PublicAPI]
    public class UpdateSchoolLicensesCommand : ICommand<UpdateSchoolLicensesCommand.Result>
    {
        public Guid SchoolId { get; set; }
        public int TeachersLicensesNumber { get; set; }
        public int StudentsLicensesNumber { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<UpdateSchoolLicensesCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
                RuleFor(_ => _.TeachersLicensesNumber).GreaterThanOrEqualTo(1).LessThan(int.MaxValue);
                RuleFor(_ => _.StudentsLicensesNumber).GreaterThanOrEqualTo(1).LessThan(int.MaxValue);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateSchoolLicensesCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(UpdateSchoolLicensesCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<UpdateSchoolLicensesCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UpdateSchoolLicensesCommand request, CancellationToken cancellationToken)
            {
                var school = await Db.Schools.GetSingleAsync(SchoolSpec.ById(request.SchoolId), cancellationToken);
                school.StudentsLicensesNumber = request.StudentsLicensesNumber;
                school.TeachersLicensesNumber = request.TeachersLicensesNumber;

                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }
    }
}
