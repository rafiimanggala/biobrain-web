using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Transfers
{
    public class PinTransferService : PinServiceBase, IPinTransferService
    {
        public PinTransferService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Transfer> PostTransferAsync(Transfer transfer)
        {
            var response = await HttpClient
                .PostAsync("1/transfers", CreateBody(transfer));

            return await ResponseProcessingWithBodyAsync<Transfer>(response);
        }
    }
}
