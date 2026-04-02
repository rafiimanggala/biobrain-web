using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BiobrainWebAPI.Core.Middleware
{
    public class ImageProxyMiddleware
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private const string BinaryLaneBaseUrl = "http://43.229.63.59:8090";

        private readonly RequestDelegate _next;
        private readonly ILogger<ImageProxyMiddleware> _logger;

        public ImageProxyMiddleware(RequestDelegate next, ILogger<ImageProxyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            // Proxy /Images/* and /images/* to BinaryLane
            if (path.StartsWith("/Images/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/user-guide-images/", StringComparison.OrdinalIgnoreCase))
            {
                await ProxyImage(context, path);
                return;
            }

            await _next(context);
        }

        private async Task ProxyImage(HttpContext context, string path)
        {
            var url = $"{BinaryLaneBaseUrl}{path}";

            try
            {
                var response = await HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                var content = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/png";

                context.Response.StatusCode = 200;
                context.Response.ContentType = contentType;
                context.Response.Headers["Cache-Control"] = "public, max-age=86400";
                await context.Response.Body.WriteAsync(content, 0, content.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to proxy image from {Url}", url);
                context.Response.StatusCode = 502;
            }
        }
    }

    public static class ImageProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseImageProxy(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ImageProxyMiddleware>();
        }
    }
}
