using System;
using Biobrain.Domain.Entities.Payment;

namespace Biobrain.Application.Specifications
{
    public static class PromoCodeSpec
    {
        public static Spec<PromoCodeEntity> ById(Guid id) => new(_ => _.PromoCodeId == id);
        public static Spec<PromoCodeEntity> ByCode(string code) => new(_ => _.Code == code);
    }
}