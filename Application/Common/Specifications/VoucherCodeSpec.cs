using Biobrain.Domain.Entities.Vouchers;
using System;

namespace Biobrain.Application.Specifications
{
    public static class VoucherCodeSpec
    {

        public static Spec<VoucherEntity> ByVoucher(string voucher)
        {
            var accessCodeParam = voucher.ToUpper();
            return new(_ => _.Code == accessCodeParam);
        }
        public static Spec<VoucherEntity> ForDates(DateTime from, DateTime to) => new(_ => _.UpdatedAt >= from && _.UpdatedAt <= to);
    }
}
