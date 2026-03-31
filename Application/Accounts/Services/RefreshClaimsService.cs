using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.SiteIdentity;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Accounts.Services
{
    internal class RefreshClaimsService : IRefreshClaimsService
    {
        private readonly IDb _db;

        public RefreshClaimsService(IDb db) => _db = db;

        public async Task RefreshClaims(Guid userId)
        {
            var exists = await _db.UserClaims.Where(x => x.UserId == userId).ToListAsync();
            _db.UserClaims.RemoveRange(exists);
            await _db.SaveChangesAsync();

            var teacher = await _db.Teachers.Include(x => x.Schools).ThenInclude(x => x.School).SingleOrDefaultAsync(TeacherSpec.ById(userId));
            if (teacher != null)
            {
	            var admins = await _db.SchoolAdmins.Where(SchoolAdminSpec.ForTeacher(teacher.TeacherId)).ToListAsync();
	            await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.SchoolId, ClaimValue = string.Join(",", teacher.Schools?.Select(x => x.School.SchoolId.ToString()) ?? new List<string>()) });
	            await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.AdminSchoolId, ClaimValue = string.Join(",", admins.Select(x => x.SchoolId.ToString()) ?? new List<string>()) });
	            await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.SchoolName, ClaimValue = string.Join(", ",teacher.Schools?.Select(x => x.School.Name) ?? new List<string>()) });
                await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.GivenName, ClaimValue = teacher.FirstName });
                await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.FamilyName, ClaimValue = teacher.LastName });
            }

            var student = await _db.Students.Include(x => x.Schools).ThenInclude(x => x.School).SingleOrDefaultAsync(StudentSpec.ById(userId));
            var subscriptions = await _db.ScheduledPayment.AsNoTracking()
	            .Where(x => x.UserId == userId && x.DeletedAt == null
	                                           && (x.Status == ScheduledPaymentStatus.Success || x.Status == ScheduledPaymentStatus.StoppedByUser))
	            .ToListAsync();
            if (student != null)
            {
	            if (student.Schools.Any())
	            {
		            await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.SchoolName, ClaimValue = string.Join(", ", student.Schools?.Select(x => x.School.Name) ?? new List<string>()) });
                }

	            await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.GivenName, ClaimValue = student.FirstName });
                await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.FamilyName, ClaimValue = student.LastName });

                if (subscriptions.Any())
                {
                    await _db.UserClaims.AddAsync(new UserClaimEntity
                    {
                        UserId = userId, ClaimType = Constant.ClaimTypes.SubscriptionStatus,
                        ClaimValue = $"{(int)subscriptions.First().Status}"
                    });

                    await _db.UserClaims.AddAsync(new UserClaimEntity
                    {
                        UserId = userId,
                        ClaimType = Constant.ClaimTypes.SubscriptionType,
                        ClaimValue = subscriptions.All(_ => _.Type == ScheduledPaymentType.FreeTrial)
                            ? $"{(int)ScheduledPaymentType.FreeTrial}"
                            : subscriptions.Any(_ => _.Type == ScheduledPaymentType.AccessCode)
                                ? $"{(int)ScheduledPaymentType.AccessCode}"
                                : subscriptions.Any(_ => _.Type == ScheduledPaymentType.Voucher)
                                    ? $"{(int)ScheduledPaymentType.Voucher}"
                                    : $"{(int)subscriptions.First(_ => _.Type != ScheduledPaymentType.FreeTrial).Type}"
                    });
                }
                else if(!student.Schools.Any())
	                await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.SubscriptionStatus, ClaimValue = $"{(int)ScheduledPaymentStatus.Inactive}" });
            }

            await _db.UserClaims.AddAsync(new UserClaimEntity { UserId = userId, ClaimType = Constant.ClaimTypes.UserId, ClaimValue = userId.ToString("N") });
            await _db.SaveChangesAsync();
        }
    }
}
