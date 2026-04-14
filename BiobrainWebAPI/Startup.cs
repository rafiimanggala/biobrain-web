using System;
using Biobrain.Application;
using Biobrain.Infrastructure.Notifications;
using Biobrain.Infrastructure.Payments;
using BiobrainWebAPI.Authorization;
using BiobrainWebAPI.Core.Configurations;
using BiobrainWebAPI.Core.ErrorHandling;
using BiobrainWebAPI.Core.Middleware;
using BiobrainWebAPI.Values;
using DataAccessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MobileAgentWeb.Core.Configurations;

namespace BiobrainWebAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			Console.WriteLine("[Startup] ConfigureServices starting...");
			Console.Out.Flush();
			//services.AddControllers();
			services.AddCors(options =>
			{
				options.AddPolicy("PinPayments", builder => builder.SetIsOriginAllowed(s => s == "https://test-api.pinpayments.com"));
				options.AddPolicy("AllowFrontend", builder =>
				{
					var siteUrl = Configuration.GetSection("SiteUrl");
					var scheme = siteUrl?.GetValue<string>("Scheme") ?? "https";
					var host = siteUrl?.GetValue<string>("Host") ?? "biobrain-web.web.app";
					builder.WithOrigins($"{scheme}://{host}", "https://biobrain-web.web.app", "http://localhost:4200")
					       .AllowAnyHeader()
					       .AllowAnyMethod()
					       .AllowCredentials();
				});
			});

            services.AddMvc(options =>
            {
                options.Filters.Add<ServiceExceptionFilter>();
                options.EnableEndpointRouting = false;
            });

			Console.WriteLine("[Startup] ConfigureAppSettings..."); Console.Out.Flush();
			services.ConfigureAppSettings(Configuration);

			Console.WriteLine("[Startup] ConfigureDatabase..."); Console.Out.Flush();
			services.ConfigureDatabase(Configuration);

			Console.WriteLine("[Startup] ConfigureJwtIdentity..."); Console.Out.Flush();
			services.ConfigureJwtIdentity(Configuration);

			Console.WriteLine("[Startup] ConfigureSingletonServiceLayer..."); Console.Out.Flush();
			services.ConfigureSingletonServiceLayer();

			Console.WriteLine("[Startup] ConfigureServiceLayer..."); Console.Out.Flush();
			services.ConfigureServiceLayer();

			Console.WriteLine("[Startup] AddApplication..."); Console.Out.Flush();
            services.AddApplication(Configuration);
			Console.WriteLine("[Startup] AddNotifications..."); Console.Out.Flush();
            services.AddNotifications(Configuration);
			Console.WriteLine("[Startup] AddPayments..."); Console.Out.Flush();
            services.AddPayments(Configuration);

			Console.WriteLine("[Startup] AddSwaggerGen..."); Console.Out.Flush();
            services.AddSwaggerGen(options =>
            {
                var defaultSchemaIdSelector = options.SchemaGeneratorOptions.SchemaIdSelector;
                options.CustomSchemaIds(t => t.DeclaringType != null ? $"{defaultSchemaIdSelector(t.DeclaringType)}_{defaultSchemaIdSelector(t)}" : defaultSchemaIdSelector(t));
            });
			Console.WriteLine("[Startup] ConfigureServices DONE"); Console.Out.Flush();
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BiobrainWebContext db)
		{
			Console.WriteLine("[Startup] Configure starting..."); Console.Out.Flush();
			try
			{
				// Ensure uuid-ossp extension exists before running migrations
				Console.WriteLine("[Startup] Creating uuid-ossp extension..."); Console.Out.Flush();
				db.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

				// Drop stale migration history if tables don't actually exist
				try
				{
					var hasUsers = db.Database.ExecuteSqlRaw("SELECT 1 FROM \"AspNetUsers\" LIMIT 1");
					Console.WriteLine("[Startup] Tables exist, running migrations..."); Console.Out.Flush();
				}
				catch
				{
					Console.WriteLine("[Startup] Tables missing — dropping stale migration history..."); Console.Out.Flush();
					try { db.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS \"__EFMigrationsHistory\" CASCADE;"); } catch { }
				}

				Console.WriteLine("[Startup] Running migrations..."); Console.Out.Flush();
				db.Database.Migrate();
				Console.WriteLine("[Startup] Migrations completed successfully"); Console.Out.Flush();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Startup] Database init error (non-fatal): {ex.Message}");
				Console.WriteLine("[Startup] Continuing without migration — existing database will be used as-is");
				Console.Out.Flush();
			}

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			Console.WriteLine("[Startup] Seeding database..."); Console.Out.Flush();
			SeedDatabase(app);
			Console.WriteLine("[Startup] Seed completed"); Console.Out.Flush();
			//app.UseHttpsRedirection();

			app.UseCors("AllowFrontend");

			Console.WriteLine("[Startup] Configuring auth..."); Console.Out.Flush();
			app.UseAuthentication();
			app.UseAuthorization();

			Console.WriteLine("[Startup] Configuring image proxy..."); Console.Out.Flush();
			app.UseImageProxy();

			Console.WriteLine("[Startup] Configuring routing..."); Console.Out.Flush();
			app.ConfigureRouting();

			Console.WriteLine("[Startup] Registering static files..."); Console.Out.Flush();
			app.RegisterStaticFiles(Configuration.GetSection(ConfigurationSections.StaticFolder).Value, Configuration.GetSection(ConfigurationSections.CacheFolder).Value);

			Console.WriteLine("[Startup] Configuring Swagger..."); Console.Out.Flush();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
			Console.WriteLine("[Startup] Configure DONE - app ready"); Console.Out.Flush();

            //app.UseEndpoints(endpoints =>
            //{
            //	endpoints.MapControllers();
            //	endpoints.MapDefaultControllerRoute();
            //});
        }

		private void SeedDatabase(IApplicationBuilder app)
        {
            var seedDataScope = app.ApplicationServices.CreateScope();

			var rolesAndAdminSeeder = seedDataScope.ServiceProvider.GetService<DefaultUserDataSeed>();
			rolesAndAdminSeeder.Seed().Wait();
		}
	}
}
