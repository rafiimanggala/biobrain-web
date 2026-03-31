using System;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Common.Models;
using Biobrain.Application.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services.Domain.Voucher
{
    public class VoucherService: IVoucherService
    {
        private readonly IDb _db;
        public VoucherService(IDb db) => _db = db;

        public async Task<GenerateCodeResult> TryGetNewVoucher()
        {
            
            var generatedCode = $"{Guid.NewGuid():N}"[..10].ToUpperInvariant();
            var existing = await _db.Vouchers.Where(_ => _.Code == generatedCode).FirstOrDefaultAsync();
            if (existing == null)
            {
                return new GenerateCodeResult{Code = generatedCode, Success = true};
            }
            
            return new GenerateCodeResult{Code = string.Empty, Success = false};
        }
    }
}