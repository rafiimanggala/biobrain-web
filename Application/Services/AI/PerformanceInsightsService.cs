using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.AI
{
    public class PerformanceInsightsService : IPerformanceInsightsService
    {
        private readonly IDb _db;
        private readonly IAiService _aiService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PerformanceInsightsService> _logger;

        public PerformanceInsightsService(
            IDb db,
            IAiService aiService,
            INotificationService notificationService,
            ILogger<PerformanceInsightsService> logger)
        {
            _db = db;
            _aiService = aiService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<string> GenerateInsightsForClassAsync(
            Guid schoolClassId, Guid courseId, DateTime fromDate, DateTime toDate)
        {
            var classInfo = await _db.SchoolClasses
                .AsNoTracking()
                .Where(c => c.SchoolClassId == schoolClassId)
                .Select(c => new { c.SchoolClassId, c.Name, c.Year })
                .FirstOrDefaultAsync();

            if (classInfo == null)
            {
                return "<p>Class not found.</p>";
            }

            var courseInfo = await _db.Courses
                .AsNoTracking()
                .Include(c => c.Subject)
                .Where(c => c.CourseId == courseId)
                .Select(c => new { c.CourseId, SubjectName = c.Subject.Name })
                .FirstOrDefaultAsync();

            var students = await _db.SchoolClassStudents
                .AsNoTracking()
                .Where(scs => scs.SchoolClassId == schoolClassId)
                .Join(
                    _db.Students.AsNoTracking(),
                    scs => scs.StudentId,
                    s => s.StudentId,
                    (scs, s) => new { s.StudentId, s.FirstName, s.LastName })
                .ToListAsync();

            if (!students.Any())
            {
                return "<p>No students enrolled in this class.</p>";
            }

            var studentIds = students.Select(s => s.StudentId).ToList();
            var studentUserIds = await _db.Users
                .AsNoTracking()
                .Where(u => u.Student != null && studentIds.Contains(u.Student.StudentId))
                .Select(u => new { UserId = u.Id, u.Student.StudentId })
                .ToListAsync();

            var userIdToStudentId = studentUserIds.ToDictionary(x => x.UserId, x => x.StudentId);
            var userIds = studentUserIds.Select(x => x.UserId).ToList();

            var quizAssignmentIds = await _db.QuizAssignments
                .AsNoTracking()
                .Where(qa => qa.SchoolClassId == schoolClassId
                    && qa.AssignedAtUtc >= fromDate
                    && qa.AssignedAtUtc <= toDate)
                .Include(qa => qa.Quiz)
                    .ThenInclude(q => q.ContentTreeNode)
                .Where(qa => qa.Quiz.ContentTreeNode.CourseId == courseId)
                .Select(qa => new
                {
                    qa.QuizAssignmentId,
                    QuizName = qa.Quiz.Name ?? qa.Quiz.ContentTreeNode.Name,
                    TopicName = qa.Quiz.ContentTreeNode.Name
                })
                .ToListAsync();

            if (!quizAssignmentIds.Any())
            {
                return "<p>No quiz activity in this period.</p>";
            }

            var assignmentIds = quizAssignmentIds.Select(qa => qa.QuizAssignmentId).ToList();

            var completedResults = await _db.QuizStudentAssignments
                .AsNoTracking()
                .Include(qsa => qsa.Result)
                .Where(qsa => assignmentIds.Contains(qsa.QuizAssignmentId)
                    && userIds.Contains(qsa.AssignedToUserId)
                    && qsa.Result != null
                    && qsa.Result.CompletedAt.HasValue)
                .Select(qsa => new
                {
                    qsa.QuizAssignmentId,
                    qsa.AssignedToUserId,
                    qsa.Result.Score,
                    qsa.Result.CompletedAt
                })
                .ToListAsync();

            var bestResults = completedResults
                .GroupBy(r => new { r.QuizAssignmentId, r.AssignedToUserId })
                .Select(g => g.OrderByDescending(x => x.CompletedAt).First())
                .ToList();

            var studentSummaries = students.Select(s =>
            {
                var userId = userIdToStudentId
                    .Where(kv => kv.Value == s.StudentId)
                    .Select(kv => kv.Key)
                    .FirstOrDefault();

                var studentResults = bestResults
                    .Where(r => r.AssignedToUserId == userId)
                    .ToList();

                return new
                {
                    Name = $"{s.FirstName} {s.LastName}",
                    QuizzesCompleted = studentResults.Count,
                    QuizzesAssigned = assignmentIds.Count,
                    AverageScore = studentResults.Any()
                        ? Math.Round(studentResults.Average(r => r.Score), 1)
                        : 0.0
                };
            }).ToList();

            var topicBreakdown = bestResults
                .Join(
                    quizAssignmentIds,
                    r => r.QuizAssignmentId,
                    qa => qa.QuizAssignmentId,
                    (r, qa) => new { qa.TopicName, r.Score })
                .GroupBy(x => x.TopicName)
                .Select(g => new
                {
                    Topic = g.Key,
                    AverageScore = Math.Round(g.Average(x => x.Score), 1),
                    AttemptCount = g.Count()
                })
                .OrderBy(x => x.AverageScore)
                .ToList();

            var dataSummary = new
            {
                ClassName = classInfo.Name,
                ClassYear = classInfo.Year,
                Subject = courseInfo?.SubjectName ?? "Unknown",
                Period = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalStudents = students.Count,
                TotalQuizzes = assignmentIds.Count,
                OverallAverageScore = bestResults.Any()
                    ? Math.Round(bestResults.Average(r => r.Score), 1)
                    : 0.0,
                CompletionRate = students.Count > 0 && assignmentIds.Count > 0
                    ? Math.Round(
                        (double)bestResults.Select(r => r.AssignedToUserId).Distinct().Count()
                        / students.Count * 100, 1)
                    : 0.0,
                Students = studentSummaries,
                TopicBreakdown = topicBreakdown
            };

            var dataJson = JsonSerializer.Serialize(dataSummary, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var systemPrompt = @"You are an educational analytics assistant for BioBrain,
a science education platform. Analyze student performance data and provide actionable
insights for teachers. Format your response in clean HTML suitable for email
(use <h3>, <p>, <ul>, <li>, <strong> tags — no <html>/<body> wrapper).
Keep insights concise and practical. Use encouraging, professional language.";

            var userPrompt = $@"Analyze this student performance data and provide actionable insights for the teacher.

Data:
{dataJson}

Provide these sections:
1) Overall class performance summary (2-3 sentences)
2) Students who need attention (low scores or incomplete quizzes)
3) Topics that need reinforcement (lowest scoring topics)
4) Positive trends to celebrate (high performers, improvement areas)

Be specific with names and numbers. If a section has nothing noteworthy, say so briefly.";

            try
            {
                var insights = await _aiService.GenerateAsync(systemPrompt, userPrompt, 2048);
                return insights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate AI insights for class {ClassId}", schoolClassId);
                return BuildFallbackInsightsHtml(dataSummary.ClassName, dataSummary.OverallAverageScore,
                    dataSummary.CompletionRate, dataSummary.TotalStudents, dataSummary.TotalQuizzes);
            }
        }

        public async Task SendWeeklyInsightsAsync()
        {
            var toDate = DateTime.UtcNow;
            var fromDate = toDate.AddDays(-7);

            _logger.LogInformation("Starting weekly insights generation for period {From} to {To}", fromDate, toDate);

            var teacherClasses = await _db.SchoolClassTeachers
                .AsNoTracking()
                .Include(sct => sct.Teacher)
                    .ThenInclude(t => t.User)
                .Include(sct => sct.SchoolClass)
                .Select(sct => new
                {
                    sct.TeacherId,
                    TeacherEmail = sct.Teacher.User.Email,
                    TeacherName = sct.Teacher.FirstName,
                    sct.SchoolClass.SchoolClassId,
                    ClassName = sct.SchoolClass.Name,
                    sct.SchoolClass.CourseId
                })
                .ToListAsync();

            if (!teacherClasses.Any())
            {
                _logger.LogInformation("No teacher-class assignments found, skipping weekly insights");
                return;
            }

            var groupedByTeacher = teacherClasses
                .GroupBy(tc => new { tc.TeacherId, tc.TeacherEmail, tc.TeacherName });

            var sentCount = 0;

            foreach (var teacherGroup in groupedByTeacher)
            {
                if (string.IsNullOrWhiteSpace(teacherGroup.Key.TeacherEmail))
                {
                    continue;
                }

                var allInsightsHtml = new List<string>();

                foreach (var classInfo in teacherGroup)
                {
                    var hasActivity = await _db.QuizAssignments
                        .AsNoTracking()
                        .AnyAsync(qa => qa.SchoolClassId == classInfo.SchoolClassId
                            && qa.AssignedAtUtc >= fromDate
                            && qa.AssignedAtUtc <= toDate);

                    if (!hasActivity)
                    {
                        continue;
                    }

                    try
                    {
                        var insights = await GenerateInsightsForClassAsync(
                            classInfo.SchoolClassId, classInfo.CourseId, fromDate, toDate);

                        allInsightsHtml.Add($"<h2>{classInfo.ClassName}</h2>{insights}<hr/>");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to generate insights for teacher {TeacherId}, class {ClassId}",
                            classInfo.TeacherId, classInfo.SchoolClassId);
                    }
                }

                if (!allInsightsHtml.Any())
                {
                    continue;
                }

                var emailBody = BuildWeeklyEmailHtml(
                    teacherGroup.Key.TeacherName,
                    fromDate,
                    toDate,
                    string.Join("\n", allInsightsHtml));

                var notification = new WeeklyInsightsNotification(
                    teacherGroup.Key.TeacherEmail,
                    $"BioBrain Weekly Performance Insights - {toDate:d MMM yyyy}",
                    emailBody);

                try
                {
                    await _notificationService.Send(notification);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send weekly insights to {Email}",
                        teacherGroup.Key.TeacherEmail);
                }
            }

            _logger.LogInformation("Weekly insights sent to {Count} teachers", sentCount);
        }

        private static string BuildWeeklyEmailHtml(
            string teacherName, DateTime fromDate, DateTime toDate, string classInsightsHtml)
        {
            return $@"
<div style=""font-family: Arial, sans-serif; max-width: 700px; margin: 0 auto;"">
    <div style=""background-color: #2563eb; color: white; padding: 20px; border-radius: 8px 8px 0 0;"">
        <h1 style=""margin: 0; font-size: 22px;"">Weekly Performance Insights</h1>
        <p style=""margin: 5px 0 0; opacity: 0.9;"">{fromDate:d MMM} - {toDate:d MMM yyyy}</p>
    </div>
    <div style=""padding: 20px; background-color: #f9fafb; border: 1px solid #e5e7eb; border-top: none; border-radius: 0 0 8px 8px;"">
        <p>Hi {teacherName},</p>
        <p>Here is your weekly performance summary for your classes:</p>
        {classInsightsHtml}
        <p style=""color: #6b7280; font-size: 13px; margin-top: 20px;"">
            This report was generated automatically by BioBrain AI.
            For detailed analytics, please visit the Class Results page in BioBrain.
        </p>
    </div>
</div>";
        }

        private static string BuildFallbackInsightsHtml(
            string className, double avgScore, double completionRate, int totalStudents, int totalQuizzes)
        {
            return $@"
<h3>Class Summary</h3>
<ul>
    <li><strong>Average Score:</strong> {avgScore}%</li>
    <li><strong>Completion Rate:</strong> {completionRate}%</li>
    <li><strong>Students:</strong> {totalStudents}</li>
    <li><strong>Quizzes Assigned:</strong> {totalQuizzes}</li>
</ul>
<p><em>Detailed AI analysis was temporarily unavailable. Please check the Class Results page for full analytics.</em></p>";
        }
    }

    internal class WeeklyInsightsNotification : IEmailNotification
    {
        public WeeklyInsightsNotification(string to, string subject, string htmlBody)
        {
            To = to;
            Subject = subject;
            HtmlBody = htmlBody;
        }

        public string To { get; }
        public string Subject { get; }
        public string HtmlBody { get; }
    }
}
