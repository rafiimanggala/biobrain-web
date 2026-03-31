using System;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Common.Models;
using Biobrain.Application.Courses;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Entities.AccessCodes;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services.Domain.AccessCode
{
    public class AccessCodeService: IAccessCodeService
    {
        private readonly IDb _db;
        public AccessCodeService(IDb db) => _db = db;

        public async Task<GenerateCodeResult> TryGetNewAccessCode()
        {
            
            var generatedCode = $"{Guid.NewGuid():N}"[..10].ToUpperInvariant();
            var existing = await _db.AccessCodes.Where(_ => _.Code == generatedCode).FirstOrDefaultAsync();
            if (existing == null)
            {
                return new GenerateCodeResult{Code = generatedCode, Success = true};
            }
            
            return new GenerateCodeResult{Code = string.Empty, Success = false};
        }

        public string GetBatchHeader(AccessCodeBatchEntity batch) => $"{batch.NumberOfCodes} codes {string.Join(", ", batch.Courses.Select(_ => CourseHelper.GetCourseName(_.Course)))} - {batch.Note}";
    }
}