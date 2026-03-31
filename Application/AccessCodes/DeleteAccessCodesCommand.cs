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

namespace Biobrain.Application.AccessCodes
{
    [PublicAPI]
    public class DeleteAccessCodesCommand : ICommand<DeleteAccessCodesCommand.Result>
    {
        public Guid AccessCodeBatchId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteAccessCodesCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.AccessCodeBatchId).ExistsInTable(db.AccessCodeBatches);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteAccessCodesCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(DeleteAccessCodesCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<DeleteAccessCodesCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteAccessCodesCommand request, CancellationToken cancellationToken)
            {
                var batch = await Db.AccessCodeBatches.Where(_ => _.AccessCodeBatchId == request.AccessCodeBatchId).SingleAsync(cancellationToken);
                Db.AccessCodeBatches.Remove(batch);
                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }
    }
}