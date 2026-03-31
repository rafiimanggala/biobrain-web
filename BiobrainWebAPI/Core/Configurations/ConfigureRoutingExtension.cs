using Microsoft.AspNetCore.Builder;

namespace MobileAgentWeb.Core.Configurations
{
    public static class ConfigureRoutingExtension
    {
        public static void ConfigureRouting(this IApplicationBuilder app)
        {
            app.MapWhen(context => context.Request.Path.Value.StartsWith("/api"), ApiRouting)
                .Use(async (context, next) =>
                {
                    await next();
                    if (context.Response.StatusCode == 404)
                    {

                        if (context.Request.Path.Value?.ToLower()?.StartsWith("/images") == true)
                            return;

                        context.Request.Path = "/";
                        await next();
                    }
                });
        }

        private static void ApiRouting(IApplicationBuilder branch)
        {
            branch.UseMvc(routes => { routes.MapRoute("api", "api/{controller}/{action}/{id?}"); });
        }
    }
}