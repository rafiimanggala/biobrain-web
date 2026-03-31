using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Charges
{
    public interface IDynamicPinChargeService
    {
        Task<Charge> PostChargeAsync(Charge charge, string apiKey);
    }
}
