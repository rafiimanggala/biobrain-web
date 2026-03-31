using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Extension;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Charges
{
    public class DynamicPinChargeService : PinServiceBase, IDynamicPinChargeService
    {
        public DynamicPinChargeService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Charge> PostChargeAsync(Charge charge, string apiKey)
        {
            var response = await HttpClient
                .PostWithApiKey("1/charges", CreateBody(charge), apiKey);
            return await ResponseProcessingWithBodyAsync<Charge>(response);
        }
    }
}
