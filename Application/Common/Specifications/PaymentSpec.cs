using System;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;

namespace Biobrain.Application.Common.Specifications
{
	public class PaymentSpec
	{
		public static Spec<PaymentEntity> ForDates(DateTime from, DateTime to) => new(_ => _.CreatedAt >= from && _.CreatedAt <= to);
		public static Spec<PaymentEntity> Succeeded() => new(_ => _.Status == PaymentStatusEnum.ChargeSuccess);
	}
}