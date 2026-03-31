using BiobrainWebAPI.Values;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moovosity.WebApi.Options;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/googleanalytics")]
    [Authorize]
    public class GoogleAnalyticsController : Controller
    {
        private readonly IConfiguration configuration;

        public GoogleAnalyticsController(
            IConfiguration configuration)
            => this.configuration = configuration;

        [HttpGet]
        public IActionResult Get()
        {
            var parameters = configuration.GetSection(ConfigurationSections.GoogleAnalytics)?.Get<DefaultGoogleAnalyticsOptions>();
            return new JsonResult(parameters);
        }
    }
}
