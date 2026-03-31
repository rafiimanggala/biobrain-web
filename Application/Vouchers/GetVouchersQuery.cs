using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Vouchers
{
    [PublicAPI]
    public sealed class GetVouchersQuery : IQuery<List<GetVouchersQuery.Result>>
    {

        [PublicAPI]
        public class Result
        {
            public Guid VoucherId { get; set; }
            public string Code { get; set; }
            public string Note { get; set; }
            public double TotalAmount { get; set; }
            public double AmountUsed { get; set; }
            public string Country { get; set; }
            public DateTime ExpiryDateUtc { get; set; }
            public DateTime RedeemExpiryDateUtc { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        internal sealed class Validator : ValidatorBase<GetVouchersQuery>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetVouchersQuery>
        {
            public PermissionCheck(ISecurityService securityService) 
                : base(securityService)
            {
            }

            protected override bool CanExecute(GetVouchersQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetVouchersQuery, List<GetVouchersQuery.Result>>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<List<GetVouchersQuery.Result>> Handle(GetVouchersQuery request, CancellationToken cancellationToken)
            {
                var vouchers = await Db.Vouchers.AsNoTracking()
                    .OrderByDescending(_ => _.CreatedAt).ThenBy(_ => _.Note)
                    .ToListAsync(cancellationToken);

                return vouchers.Select(_ => new Result
                {
                    VoucherId = _.VoucherId,
                    Note = _.Note,
                    Country = _.Country,
                    TotalAmount = _.TotalAmount,
                    AmountUsed = _.AmountUsed,
                    Code = _.Code,
                    CreatedAt = _.CreatedAt,
                    ExpiryDateUtc = _.ExpiryDateUtc,
                    RedeemExpiryDateUtc = _.RedeemExpiryDateUtc,
                }).ToList();
            }
        }
    }
}
