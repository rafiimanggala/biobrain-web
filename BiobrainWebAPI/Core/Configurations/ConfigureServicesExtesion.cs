using System;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Quizzes.Services;
using Biobrain.Application.Services.AI;
using BiobrainWebAPI.Authorization;
using BiobrainWebAPI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BiobrainWebAPI.Core.Configurations
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServiceLayer(this IServiceCollection service)
        {
	        ConfigureSeeders(service);
            service.AddScoped<ISiteUrls, SiteUrls>();
            service.AddScoped<IQuestionPoolService, QuestionPoolService>();

            ConfigureAiServices(service);

            return service;
        }

        private static void ConfigureSeeders(IServiceCollection service)
        {
            service.AddTransient<DefaultUserDataSeed>();
        }

        private static void ConfigureAiServices(IServiceCollection service)
        {
            service.AddHttpClient("Anthropic", client =>
            {
                client.BaseAddress = new Uri("https://api.anthropic.com/");
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            });
            service.AddScoped<IAiService, ClaudeAiService>();
        }
    }
}