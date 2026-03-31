using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.Voucher;
using Biobrain.Domain.Entities.Vouchers;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.Vouchers
{
    [PublicAPI]
    public class CreateVoucherCommand : ICommand<CreateVoucherCommand.Result>
    {
        public string Note { get; set; }
        public double TotalAmount { get; set; }
        public string Country { get; set; }
        public DateTime ExpiryDateUtc { get; set; }
        public DateTime RedeemExpiryDateUtc { get; set; }
        public int NumberOfVouchers { get; set; }


        [PublicAPI]
        public class Result
        {
        }

        [PublicAPI]
        public class AccessCode
        {
            public Guid AccessCodeId { get; init; }
            public string Code { get; init; }
        }


        internal class Validator : ValidatorBase<CreateVoucherCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.NumberOfVouchers).GreaterThan(0);
                RuleFor(_ => _.Country).NotNull().NotEmpty();
                RuleFor(_ => _.Note).NotNull().NotEmpty();
            }
        }


        internal class PermissionCheck : PermissionCheckBase<CreateVoucherCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(CreateVoucherCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<CreateVoucherCommand, Result>
        {
            private readonly IVoucherService _vouchersService;
            public Handler(IDb db, IVoucherService vouchersService) : base(db) => _vouchersService = vouchersService;

            public override async Task<Result> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
            {
                var codes = new List<string>();
                for (var i = 0; i < request.NumberOfVouchers; i++)
                {
                    var result = await _vouchersService.TryGetNewVoucher();
                    while (!result.Success)
                    {
                        result = await _vouchersService.TryGetNewVoucher();
                    }
                    codes.Add(result.Code);
                }

                await Db.Vouchers.AddRangeAsync(
                    codes.Select(_ => new VoucherEntity
                    {
                        Code = _, AmountUsed = 0, Country = request.Country, TotalAmount = request.TotalAmount,
                        ExpiryDateUtc = request.ExpiryDateUtc, Note = request.Note,
                        RedeemExpiryDateUtc = request.RedeemExpiryDateUtc, RedeemedDateUtc = null
                    }), cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}