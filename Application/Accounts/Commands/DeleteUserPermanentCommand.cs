using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;

namespace Biobrain.Application.Accounts.Commands
{
    [PublicAPI]
    public class DeleteUserPermanentCommand : ICommand<DeleteUserPermanentCommand.Result>
    {
        public Guid UserId { get; set; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteUserPermanentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteUserPermanentCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(DeleteUserPermanentCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<DeleteUserPermanentCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteUserPermanentCommand request, CancellationToken cancellationToken)
            {
                var user = await Db.Users.FindAsync(request.UserId);
                Db.Users.Remove(user);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
