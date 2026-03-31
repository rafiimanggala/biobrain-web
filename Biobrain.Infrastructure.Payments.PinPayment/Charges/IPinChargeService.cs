using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Charges
{
    public interface IPinChargeService
    {
        Task<Charge> PostChargeAsync(Charge charge);
    }
}
