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

namespace Biobrain.Application.Vouchers
{
    [PublicAPI]
    public class DeleteVoucherCommand : ICommand<DeleteVoucherCommand.Result>
    {
        public Guid VoucherId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteVoucherCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.VoucherId).ExistsInTable(db.Vouchers);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteVoucherCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(DeleteVoucherCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<DeleteVoucherCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteVoucherCommand request, CancellationToken cancellationToken)
            {
                var voucher = await Db.Vouchers.Where(_ => _.VoucherId == request.VoucherId).SingleAsync(cancellationToken);
                Db.Vouchers.Remove(voucher);
                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }
    }
}