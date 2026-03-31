using System.Threading.Tasks;
using Biobrain.Application.Common.Models;

namespace Biobrain.Application.Services.Domain.Voucher
{
    public interface IVoucherService
    {
        Task<GenerateCodeResult> TryGetNewVoucher();
    }
}