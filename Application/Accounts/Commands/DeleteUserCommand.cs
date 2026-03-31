using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Accounts.Commands
{
    [PublicAPI]
    public class DeleteUserCommand : ICommand<DeleteUserCommand.Result>
    {
        public Guid UserId { get; set; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteUserCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteUserCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(DeleteUserCommand request, IUserSecurityInfo user) => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<DeleteUserCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                var user = await Db.Users.FindAsync(request.UserId);
                user.DeletedAt = DateTime.UtcNow;
                // ToDo: Delete personal info

                var subscriptions = await Db.ScheduledPayment
	                .Where(x => x.UserId == request.UserId && x.DeletedAt == null).ToListAsync(cancellationToken);
                subscriptions.ForEach(x => x.DeletedAt = DateTime.UtcNow);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
