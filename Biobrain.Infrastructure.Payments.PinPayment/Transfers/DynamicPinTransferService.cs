using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Extension;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Transfers
{
    public class DynamicPinTransferService : PinServiceBase, IDynamicPinTransferService
    {
        public DynamicPinTransferService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Transfer> PostTransferAsync(Transfer transfer, string apiKey)
        {
            var response = await HttpClient
                .PostWithApiKey("1/transfers", CreateBody(transfer), apiKey);

            return await ResponseProcessingWithBodyAsync<Transfer>(response);
        }
    }
}
