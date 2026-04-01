using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Behaviours;
using Biobrain.Application.Content.Internal;
using Biobrain.Application.Content.Services;
using Biobrain.Application.Content.Services.ContentCacheService;
using Biobrain.Application.LearningMaterialAssignments.Services;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Services.Domain.AccessCode;
using Biobrain.Application.Services.Domain.AvailableCourses;
using Biobrain.Application.Services.Domain.ContentTreeService;
using Biobrain.Application.Services.Domain.QuizAutoMap;
using Biobrain.Application.Services.Domain.Reports;
using Biobrain.Application.Services.Domain.TrackSession;
using Biobrain.Application.Services.Domain.Voucher;
using Biobrain.Application.Services.Hosted;
using Biobrain.Application.Services.AI;
using Biobrain.Application.Services.Auth;
using Biobrain.Application.Students;
using DinkToPdf;
using DinkToPdf.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biobrain.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPermissionCheckBehavior<,>));

            AddValidators(services);
            AddPermissionChecks(services);
            RegisterServices(services);

            services.AddTransient<ISecurityService, SecurityService>();

            return services;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Transient
            services.AddTransient<IContentTreeDataBuilder, ContentTreeDataBuilder>();
            services.AddTransient<IContentPageDataBuilder, ContentPageDataBuilder>();
            services.AddTransient<IContentQuizDataBuilder, ContentQuizDataBuilder>();
            services.AddTransient<IContentGlossaryTermBuilder, ContentGlossaryTermBuilder>();
            services.AddTransient<ICreateAccountService, CreateAccountService>();
            services.AddTransient<ILearningMaterialNameService, LearningMaterialNameService>();
            services.AddTransient<IAssignLearningMaterialService, AssignLearningMaterialService>();
            services.AddTransient<IContentTreeService, ContentTreeService>();
            services.AddTransient<IAssignLearningMaterialsNotificationService, AssignLearningMaterialsNotificationService>();
            services.AddTransient<IContentTreePathResolver, ContentTreePathResolver>();
            services.AddTransient<ITemplateService, TemplateService>();
            services.AddTransient<IContentCacheService, ContentCacheService>();
            services.AddTransient<IRefreshClaimsService, RefreshClaimsService>();
            services.AddTransient<IAvailableCoursesService, AvailableCoursesService>();
            services.AddTransient<ITrackSessionService, TrackSessionService>();
            services.AddTransient<IUsageReportChartService, UsageReportChartService>();
            services.AddTransient<IUsageReportPdfService, UsageReportPdfService>();
            services.AddTransient<IAccessCodeService, AccessCodeService>();
            services.AddTransient<IQuizAutoMapService, QuizAutoMapService>();
            services.AddTransient<IQuizStreakService, QuizStreakService>();
            services.AddTransient<IUsageReportService, UsageReportService>();
            services.AddTransient<IVoucherService, VoucherService>();
            services.AddTransient<IJoinStudentToSchoolClassWithAccessCodeService, JoinStudentToSchoolClassWithAccessCodeService>();
            services.AddTransient<ISamlService, SamlService>();

            // AI
            services.AddScoped<IPerformanceInsightsService, PerformanceInsightsService>();
            services.AddScoped<IAskBiobrainService, AskBiobrainService>();
            services.AddScoped<IPracticeSetGeneratorService, PracticeSetGeneratorService>();

            // Hosted
            services.AddHostedService<TempFileDeleterService>();
            services.AddHostedService<TempHistoryCleanerService>();
            services.AddHostedService<FreeTrialCheckerService>();
            services.AddHostedService<WelcomeEmailService>();
            services.AddHostedService<WeeklyInsightsHostedService>();

            // PDF converter — DinkToPdf requires libwkhtmltox native lib which
            // causes SIGSEGV on some cloud platforms (e.g. Render).
            // Set DISABLE_PDF=true to skip loading the native library entirely.
            if (Environment.GetEnvironmentVariable("DISABLE_PDF") != "true")
            {
                RegisterPdfConverter(services);
            }
            else
            {
                Console.WriteLine("[DI] PDF export disabled via DISABLE_PDF env var");
                services.AddSingleton<IConverter>(sp => throw new InvalidOperationException("PDF export is not available on this platform."));
            }
        }

        private static void AddValidators(IServiceCollection services)
        {
            foreach (var (@interface, implementation) in GetImplementationsOfGenericInterface(typeof(IValidator<>)))
                services.AddTransient(@interface, implementation);
        }

        private static void AddPermissionChecks(IServiceCollection services)
        {
            foreach (var (@interface, implementation) in GetImplementationsOfGenericInterface(typeof(IPermissionCheck<>)))
                services.AddTransient(@interface, implementation);
        }

        /// <summary>
        /// Isolated in a separate method so the JIT compiler only loads the DinkToPdf
        /// assembly (and its libwkhtmltox native dependency) when this method is actually called.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void RegisterPdfConverter(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }

        private static IEnumerable<(Type @interface, Type implementation)> GetImplementationsOfGenericInterface(Type baseInterface) => from type in typeof(DependencyInjection).Assembly.GetTypes()
                                                                                                                                       where !type.IsAbstract && !type.IsGenericTypeDefinition
                                                                                                                                       from @interface in type.GetInterfaces()
                                                                                                                                       where @interface.IsGenericType
                                                                                                                                       where @interface.GetGenericTypeDefinition() == baseInterface
                                                                                                                                       select (@interface, type);
    }
}
