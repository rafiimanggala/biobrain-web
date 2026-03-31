using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Transfers
{
    public interface IPinTransferService
    {
        Task<Transfer> PostTransferAsync(Transfer transfer);
    }
}
