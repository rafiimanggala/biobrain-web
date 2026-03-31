using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;

namespace Biobrain.Application.Schools.DeleteSchool
{
    [PublicAPI]
    public class DeleteSchoolCommand : ICommand<DeleteSchoolCommand.Result>
    {
        public Guid SchoolId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteSchoolCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteSchoolCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(DeleteSchoolCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<DeleteSchoolCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
            {
                var school = await Db.Schools.GetSingleAsync(SchoolSpec.ById(request.SchoolId), cancellationToken);
                Db.Schools.Remove(school);
                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}