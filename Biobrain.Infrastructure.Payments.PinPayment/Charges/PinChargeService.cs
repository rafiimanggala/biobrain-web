using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Charges
{
    public class PinChargeService : PinServiceBase, IPinChargeService
    {
        public PinChargeService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Charge> PostChargeAsync(Charge charge)
        {
            var response = await HttpClient
                .PostAsync("1/charges", CreateBody(charge));
            return await ResponseProcessingWithBodyAsync<Charge>(response);
        }
    }
}
