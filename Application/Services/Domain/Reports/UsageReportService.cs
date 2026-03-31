using Biobrain.Application.Common.Helpers;
using Biobrain.Application.Common.Models;
using Biobrain.Application.Reports.UsageReport;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.User;
using BiobrainWebAPI.Values;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services.Domain.Reports
{
    public class UsageReportService(IDb db,
                                    IUsageReportChartService usageReportChartService,
                                    IUsageReportPdfService usageReportPdfService,
                                    ILogger<UsageReportService> logger)
        : IUsageReportService
    {
        private readonly IUsageReportChartService _usageReportChartService = usageReportChartService;
        private readonly IUsageReportPdfService _usageReportPdfService = usageReportPdfService;
        private readonly ILogger<UsageReportService> _logger = logger;
        private readonly IDb Db = db;

        public async Task<string> GetSchoolReport(Guid schoolId, string timeZoneId, DateTime from, DateTime to, string tempReportFolder, CancellationToken cancellationToken)
        {
            //Get Data
            //var tempReportFolder = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink);

            var school = await Db.Schools.Where(SchoolSpec.ById(schoolId)).Include(_ => _.Courses).FirstAsync(cancellationToken);
            var fileName = school.Name.Replace('/', '|') + ".pdf";
            var availableCourses = school.Courses.Select(_ => _.CourseId).ToList();
            var timePeriod = GetTimePeriod(from, to);
            var reportSubjects = await GetUsageReportSubjects(schoolId);
            var report = new UsageReportModel
            {
                ReportPath = Path.Combine(tempReportFolder, fileName),
                SchoolName = school.Name,
                FromDate = TimeZoneInfoHelper.ConvertTimeFromUtc(from, timeZoneId).ToString("dd/MM/yyyy"),
                ToDate = TimeZoneInfoHelper.ConvertTimeFromUtc(to, timeZoneId).ToString("dd/MM/yyyy"),
            };

            var userSessions = await Db.UserSessions.AsNoTracking()
                .Include(_ => _.Courses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
                .Include(_ => _.Schools)
                .Include(_ => _.User).ThenInclude(_ => _.Student)
                .Include(_ => _.User).ThenInclude(_ => _.Teacher)
                .Where(UserSessionViewSpec.ForSchool(schoolId))
                .Where(_ => _.StartAt >= from)
                .Where(_ => _.StartAt < to)
                .OrderBy(_usageReportService => _usageReportService.StartAt)
                .ToListAsync(cancellationToken);
            var sessionsByInterval = GetSessionsByInterval(userSessions, timePeriod, from, to).OrderBy(_ => _.Key);
            report.TotalNumberOfSessions = userSessions.Count;
            report.AverageSessionDuration = userSessions.Count == 0 ? 0 : userSessions.Sum(_ => (_.LastTrack - _.StartAt).TotalMinutes) / userSessions.Count;

            _logger.LogWarning($"Report {school.Name}");
            _logger.LogWarning($"Total sessions: {report.TotalNumberOfSessions}");
            _logger.LogWarning($"Total teacher session: {userSessions.Count(_ => _.User.Teacher != null)}");
            _logger.LogWarning($"Total users: {userSessions.Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()}");

            // Get Number of sessions chart
            report.NumberOfSessionsByPeriod = GetNumberOfSessionsChartData(sessionsByInterval, reportSubjects, availableCourses);
            report.SessionsChart = report.NumberOfSessionsByPeriod.Any()
                ? _usageReportChartService.GetLinearChartImagePath(tempReportFolder,
            report.NumberOfSessionsByPeriod,
            TimeZoneInfoHelper.ConvertTimeFromUtc(from, timeZoneId), timePeriod)
                : new ChartModel { ChartImagePath = string.Empty };

            // Get number of teacher sessions chart
            report.NumberOfTeacherUsersByPeriod = await GetTeachersSessionsChartData(sessionsByInterval, schoolId, availableCourses);
            report.TeacherUsersChart = report.NumberOfTeacherUsersByPeriod.Any()
                ? _usageReportChartService.GetLinearChartImagePath(tempReportFolder,
            report.NumberOfTeacherUsersByPeriod,
            TimeZoneInfoHelper.ConvertTimeFromUtc(from, timeZoneId), timePeriod)
                : new ChartModel { ChartImagePath = string.Empty };

            // Get number of student users chart
            report.NumberOfStudentUsersByPeriod = GetNumberOfUsersChartData(sessionsByInterval, reportSubjects, Constant.Roles.Student, availableCourses);
            report.StudentUsersChart = report.NumberOfStudentUsersByPeriod.Any()
                ? _usageReportChartService.GetLinearChartImagePath(tempReportFolder,
            report.NumberOfStudentUsersByPeriod,
            TimeZoneInfoHelper.ConvertTimeFromUtc(from, timeZoneId), timePeriod)
                : new ChartModel { ChartImagePath = string.Empty };

            // Get Page View chart
            var schoolPageViews = (await Db.UserPageViews.AsNoTracking()
                    .Where(_ => _.CreatedAt >= from)
                    .Where(_ => _.CreatedAt < to)
                    .Where(UserPageViewSpec.ForSchool(schoolId))
                    .ToListAsync(cancellationToken))
                .GroupBy(_ => _.PagePath)
                .OrderByDescending(_ => _.Count())
                .Take(AppSettings.UsageReportPagesCount);
            report.PagesChart = _usageReportChartService.GetPagesPieChartImagePath(tempReportFolder, schoolPageViews.ToDictionary(_ => _.Key, _ => _.Count()));

            _usageReportPdfService.GeneratePdf(report);
            return fileName;
        }

        private Task<UsageReportSubjects> GetUsageReportSubjects(Guid schoolId) => Task.FromResult(new UsageReportSubjects());

        private Dictionary<ReportKeyData, List<int>> GetNumberOfSessionsChartData(IEnumerable<KeyValuePair<int, List<UserSessionEntity>>> sessionsByInterval, UsageReportSubjects subjects, List<Guid> availableCourses)
        {
            var chartData = new Dictionary<ReportKeyData, List<int>>();

            var vceBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Vce && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (vceBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyVce, vceBiologyData);
            }

            var saceBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Sace && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (saceBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologySace, saceBiologyData);
            }

            var idBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Sl && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (idBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyIb, idBiologyData);
            }

            var seniorBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Ib && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (seniorBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologySenior, seniorBiologyData);
            }

            var apBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Ap && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (apBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyAp, apBiologyData);
            }

            var hscBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Hsc && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (hscBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyHsc, hscBiologyData);
            }

            //var chemistryData = sessionsByInterval.Select(_ => _.Value.Count(s =>
            //    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry))).ToList();
            //if (chemistryData.Any(_ => _ != 0))
            //{
            //    chartData.Add(subjects.Chemistry, chemistryData);
            //}

            var vceChemistryData = sessionsByInterval.Select(_ => _.Value.Count(s =>
                s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Vce && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (vceChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistryVce, vceChemistryData);
            }

            var saceChemistryData = sessionsByInterval.Select(_ => _.Value.Count(s =>
                s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Sace && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (saceChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistrySace, saceChemistryData);
            }

            var ibChemistryData = sessionsByInterval.Select(_ => _.Value.Count(s =>
                s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Sl && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (ibChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistryIb, ibChemistryData);
            }

            var seniorChemistryData = sessionsByInterval.Select(_ => _.Value.Count(s =>
                s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Ib && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (seniorChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistrySenior, seniorChemistryData);
            }

            //var physicsData = sessionsByInterval.Select(_ =>
            //    _.Value.Count(s =>
            //        s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics))).ToList();
            //if (physicsData.Any(_ => _ != 0))
            //{
            //    chartData.Add(subjects.Physics, physicsData);
            //}

            var vcePhysicsData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Vce && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (vcePhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsVce, vcePhysicsData);
            }

            var sacePhysicsData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Sace && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (sacePhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsSace, sacePhysicsData);
            }

            var ibPhysicsData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Sl && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (ibPhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsIb, ibPhysicsData);
            }

            var seniorPhysicsData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Ib && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))).ToList();
            if (seniorPhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsSenior, seniorPhysicsData);
            }

            var year10Data = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c =>
                        (c.Course.SubjectCode == Constant.Subject.Biology10 || c.Course.SubjectCode == Constant.Subject.Chemistry10 || c.Course.SubjectCode == Constant.Subject.Physics10
                        || c.Course.SubjectCode == Constant.Subject.Forensics10 || c.Course.SubjectCode == Constant.Subject.Marine10 || c.Course.SubjectCode == Constant.Subject.Psychology10
                        || c.Course.SubjectCode == Constant.Subject.Science10 || c.Course.SubjectCode == Constant.Subject.EarthScience || c.Course.SubjectCode == Constant.Subject.Science10Pen
                        || c.Course.SubjectCode == Constant.Subject.Biology10Us || c.Course.SubjectCode == Constant.Subject.Earth10Jpc || c.Course.SubjectCode == Constant.Subject.Applied10Jpc) && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))
                    ))).ToList();
            if (year10Data.Any(_ => _ != 0))
            {
                chartData.Add(subjects.Year10, year10Data);
            }

            var lifeData = sessionsByInterval.Select(_ =>
                _.Value.Count(s =>
                    s.Courses.Any(c =>
                        c.Course.SubjectCode == Constant.Subject.LifeScience && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))
                    ))).ToList();
            if (lifeData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.Life, lifeData);
            }

            return chartData;
        }

        private static Dictionary<ReportKeyData, List<int>> GetNumberOfUsersChartData(IEnumerable<KeyValuePair<int, List<UserSessionEntity>>> sessionsByInterval, UsageReportSubjects subjects, string role, List<Guid> availableCourses)
        {
            if (role != Constant.Roles.Teacher && role != Constant.Roles.Student) throw new ValidationException($"Role {role} not supported for this report");

            var chartData = new Dictionary<ReportKeyData, List<int>>();

            //var biologyData = sessionsByInterval.Select(_ =>
            //    _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology))
            //        .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) ||
            //                    (role == Constant.Roles.Student && _.User.Student != null))
            //        .Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()).ToList();
            //if (biologyData.Any(_ => _ != 0))
            //{
            //    chartData.Add(subjects.Biology, biologyData);
            //}

            var vceBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Vce && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                    .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) ||
                                (role == Constant.Roles.Student && _.User.Student != null))
                    .Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()).ToList();
            if (vceBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyVce, vceBiologyData);
            }

            var saceBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Sace && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                    .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) ||
                                (role == Constant.Roles.Student && _.User.Student != null))
                    .Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()).ToList();
            if (saceBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologySace, saceBiologyData);
            }

            var ibBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Sl && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                    .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) ||
                                (role == Constant.Roles.Student && _.User.Student != null))
                    .Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()).ToList();
            if (ibBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyIb, ibBiologyData);
            }

            var seniorBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Ib && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                    .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) ||
                                (role == Constant.Roles.Student && _.User.Student != null))
                    .Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()).ToList();
            if (seniorBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologySenior, seniorBiologyData);
            }

            var apBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Ap && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                    .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) ||
                                (role == Constant.Roles.Student && _.User.Student != null))
                    .Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()).ToList();
            if (apBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyAp, apBiologyData);
            }

            var hscBiologyData = sessionsByInterval.Select(_ =>
                _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Biology && c.Course.CurriculumCode == Constant.Curriculum.Hsc && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                    .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) ||
                                (role == Constant.Roles.Student && _.User.Student != null))
                    .Select(_ => _.UserId).DistinctBy(_ => _.ToString()).Count()).ToList();
            if (hscBiologyData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.BiologyHsc, hscBiologyData);
            }

            //var chemistryData = sessionsByInterval.Select(_ =>
            //            _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry))
            //                .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
            //                .Select(_ => _.UserId).Distinct().Count()).ToList();
            //if (chemistryData.Any(_ => _ != 0))
            //{
            //    chartData.Add(subjects.Chemistry, chemistryData);
            //}

            var vceChemistryData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Vce && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (vceChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistryVce, vceChemistryData);
            }

            var saceChemistryData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Sace && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (saceChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistrySace, saceChemistryData);
            }

            var ibChemistryData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Sl && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (ibChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistryIb, ibChemistryData);
            }

            var seniorChemistryData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Chemistry && c.Course.CurriculumCode == Constant.Curriculum.Ib && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (seniorChemistryData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.ChemistrySenior, seniorChemistryData);
            }

            //var physicsData = sessionsByInterval.Select(_ =>
            //            _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics))
            //                .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
            //                .Select(_ => _.UserId).Distinct().Count()).ToList();
            //if (physicsData.Any(_ => _ != 0))
            //{
            //    chartData.Add(subjects.Physics, physicsData);
            //}

            var vcePhysicsData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Vce && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (vcePhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsVce, vcePhysicsData);
            }

            var sacePhysicsData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Sace && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (sacePhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsSace, sacePhysicsData);
            }

            var ibPhysicsData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Sl && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (ibPhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsIb, ibPhysicsData);
            }

            var seniorPhysicsData = sessionsByInterval.Select(_ =>
                        _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.Physics && c.Course.CurriculumCode == Constant.Curriculum.Ib && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (seniorPhysicsData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.PhysicsSenior, seniorPhysicsData);
            }

            var year10Data = sessionsByInterval.Select(_ =>
                        _.Value
                            .Where(s => s.Courses.Any(c =>
                                (c.Course.SubjectCode == Constant.Subject.Biology10 || c.Course.SubjectCode == Constant.Subject.Chemistry10 || c.Course.SubjectCode == Constant.Subject.Physics10
                                || c.Course.SubjectCode == Constant.Subject.Forensics10 || c.Course.SubjectCode == Constant.Subject.Marine10 || c.Course.SubjectCode == Constant.Subject.Psychology10 
                                || c.Course.SubjectCode == Constant.Subject.Science10 || c.Course.SubjectCode == Constant.Subject.EarthScience || c.Course.SubjectCode == Constant.Subject.Science10Pen
                                || c.Course.SubjectCode == Constant.Subject.Biology10Us || c.Course.SubjectCode == Constant.Subject.Earth10Jpc || c.Course.SubjectCode == Constant.Subject.Applied10Jpc) && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))
                                ))
                            .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                            .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (year10Data.Any(_ => _ != 0))
            {
                chartData.Add(subjects.Year10, year10Data);
            }

            var lifeData = sessionsByInterval.Select(_ =>
                _.Value.Where(s => s.Courses.Any(c => c.Course.SubjectCode == Constant.Subject.LifeScience && (!availableCourses.Any() || availableCourses.Any(ac => ac == c.CourseId))))
                    .Where(_ => (role == Constant.Roles.Teacher && _.User.Teacher != null) || (role == Constant.Roles.Student && _.User.Student != null))
                    .Select(_ => _.UserId).Distinct().Count()).ToList();
            if (lifeData.Any(_ => _ != 0))
            {
                chartData.Add(subjects.Life, lifeData);
            }

            return chartData;
        }

        private async Task<Dictionary<ReportKeyData, List<int>>> GetTeachersSessionsChartData(IEnumerable<KeyValuePair<int, List<UserSessionEntity>>> sessionsByInterval, Guid schoolId, List<Guid> availableCourses)
        {
            //Constant.Roles.Teacher

            var chartData = new Dictionary<ReportKeyData, List<int>>();

            var teachers = await Db.Teachers.Include(_ => _.Schools).Where(TeacherSpec.ForSchool(schoolId)).ToListAsync();
            var i = 0;

            foreach (var teacher in teachers)
            {
                var teacherData = sessionsByInterval.Select(_ =>
                    _.Value.Count(s =>
                        s.UserId == teacher.TeacherId
                        && (availableCourses.Count is 0 || s.Courses.Any(sc => availableCourses.Any(ac => ac == sc.CourseId))))).ToList();
                if (teacherData.Any(_ => _ != 0))
                {
                    chartData.Add(new ReportKeyData { Name = teacher.GetFullName(), Color = Constant.Colors.List[i] }, teacherData);
                    i++;
                    if (i > Constant.Colors.List.Count - 1)
                        i = 0;
                }
            }

            return chartData;
        }

        private static Constant.TimeInterval GetTimePeriod(DateTime start, DateTime end)
        {
            var interval = end - start;
            if (interval.TotalDays > 800) return Constant.TimeInterval.Month;
            if (interval.TotalDays > 189) return Constant.TimeInterval.Week;
            return Constant.TimeInterval.Day;
        }

        private static Dictionary<int, List<UserSessionEntity>> GetSessionsByInterval(List<UserSessionEntity> sessions,
                                                                                      Constant.TimeInterval interval, DateTime startDate, DateTime endDate)
        {
            var start = startDate;
            var result = new Dictionary<int, List<UserSessionEntity>>();
            var i = 1;

            while (start < endDate)
            {
                var end = GetIntervalEnd(start, interval);

                result.Add(i, [..sessions.Where(_ => _.StartAt >= start && _.StartAt < end)]);

                start = end;
                i++;
            }

            return result;
        }

        private static DateTime GetIntervalEnd(DateTime start, Constant.TimeInterval interval)
        {
            return interval switch
            {
                Constant.TimeInterval.Day => start.AddDays(1),
                Constant.TimeInterval.Week => start.AddDays(7),
                Constant.TimeInterval.Month => start.AddMonths(1),
                _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
            };
        }

        private static string GetIntervalLabel(Constant.TimeInterval interval)
        {
            return interval switch
            {
                Constant.TimeInterval.Day => Strings.Day,
                Constant.TimeInterval.Week => Strings.Week,
                Constant.TimeInterval.Month => Strings.Month,
                _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
            };
        }
    }
}