using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.User;
using JetBrains.Annotations;

namespace Biobrain.Application.Accounts.Commands
{
    [PublicAPI]
    public class SaveUserLogCommand : ICommand<SaveUserLogCommand.Result>
    {
        public string Log { get; set; }
        

        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<SaveUserLogCommand>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SaveUserLogCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SaveUserLogCommand request, IUserSecurityInfo user) =>
                //return user.IsAccountOwner(request.UserId);
                true;
        }


        internal class Handler : CommandHandlerBase<SaveUserLogCommand, Result>
        {
            private readonly ISessionContext _sessionContext;

            public Handler(IDb db, ISessionContext sessionContext) : base(db) => _sessionContext = sessionContext;

            public override async Task<Result> Handle(SaveUserLogCommand request, CancellationToken cancellationToken)
            {
                var userId = _sessionContext.GetUserId();

                await Db.UserLogs.AddAsync(new UserLogEntity { Log = request.Log, UserId = userId }, cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
