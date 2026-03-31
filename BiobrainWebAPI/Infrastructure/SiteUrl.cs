using System;
using Biobrain.Application.Interfaces.ExecutionContext;
using BiobrainWebAPI.Core.ErrorHandling.Exceptions;
using BiobrainWebAPI.Values.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BiobrainWebAPI.Infrastructure
{
    public class SiteUrls : ISiteUrls
    {
        private readonly string _host;

        public SiteUrls(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            var siteUrlSettings = configuration.GetSection(SiteUrlSettings.Section)?.Get<SiteUrlSettings>();
            if (siteUrlSettings != null)
            {
                _host = $"{siteUrlSettings.Scheme}://{siteUrlSettings.Host}";
            }
            else
            {
                if (contextAccessor.HttpContext == null)
                    throw new InvalidOperationException("'HttpContext' must not be null.");

                var referer = contextAccessor.HttpContext.Request;
                _host = $"{referer.Scheme}://{referer.Host}";
            }
        }

        private Uri FullUrl(string path)
        {
            if (!Uri.TryCreate($"{_host}/{path}", UriKind.Absolute, out var url)) throw new ServiceException("Can't create Uri.");
            return url;
        }

        public Uri Login() => FullUrl("login");
        public Uri SetPassword(string login, string token) => FullUrl($"set-password?login={login}&token={Uri.EscapeDataString(token)}");
        public Uri SetPasswordAfterRegistration(string login, string token) => FullUrl($"set-password?login={login}&token={Uri.EscapeDataString(token)}&registration");
        public Uri PerformQuiz(Guid quizStudentAssignmentId, Guid courseId) => FullUrl($"quiz/{quizStudentAssignmentId}?courseId={courseId}");
        public Uri PerformAssignedLearningMaterial(Guid learningMaterialUserAssignmentId) 
            => FullUrl($"perform-assigned-learning-material/{learningMaterialUserAssignmentId}");
    }
}
