using System;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services
{
    public class QuizStreak
    {
        public int Streak { get; set; }
        public double DaysCount { get; set; }
    }
    public interface IQuizStreakService
    {
        Task<QuizStreak> GetStreak(Guid userId, Guid courseId, DateTime localDateTime);
    }
    public class QuizStreakService:IQuizStreakService
    {
        private readonly IDb _db;

        public QuizStreakService(IDb db) => _db = db;

        public async Task<QuizStreak> GetStreak(Guid userId, Guid courseId, DateTime localDateTime)
        {
            var streak = await _db.QuizStreak.Where(_ => _.CourseId == courseId && _.StudentId == userId)
                .FirstOrDefaultAsync();

            var localDate = localDateTime.ToLocalTime().Date;
            return streak == null
                ? new QuizStreak { DaysCount = 0, Streak = 0 }
                : new QuizStreak
                {
                    Streak = localDate == streak.UpdatedAtLocal.Date || localDate.AddDays(-1).Date == streak.UpdatedAtLocal.Date
                        ? streak.WeeksStreak
                        : 0,
                    DaysCount = localDate == streak.UpdatedAtLocal.Date || localDate.AddDays(-1).Date == streak.UpdatedAtLocal.Date
                        ? streak.DaysCount
                        : 0
                };
        }
    }
}