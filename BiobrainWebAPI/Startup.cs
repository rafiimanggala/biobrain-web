using Biobrain.Application;
using Biobrain.Infrastructure.Notifications;
using Biobrain.Infrastructure.Payments;
using BiobrainWebAPI.Authorization;
using BiobrainWebAPI.Core.Configurations;
using BiobrainWebAPI.Core.ErrorHandling;
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
			//services.AddControllers();
			services.AddCors(options => options.AddPolicy("PinPayments", builder => builder.SetIsOriginAllowed(s => s == "https://test-api.pinpayments.com")));

            services.AddMvc(options =>
            {
                options.Filters.Add<ServiceExceptionFilter>();
                options.EnableEndpointRouting = false;
            });

			services.ConfigureAppSettings(Configuration);

			services.ConfigureDatabase(Configuration);

			services.ConfigureJwtIdentity(Configuration);

			services.ConfigureSingletonServiceLayer();

			services.ConfigureServiceLayer();

            services.AddApplication(Configuration);
            services.AddNotifications(Configuration);
            services.AddPayments(Configuration);


            services.AddSwaggerGen(options =>
            {
                var defaultSchemaIdSelector = options.SchemaGeneratorOptions.SchemaIdSelector;
                options.CustomSchemaIds(t => t.DeclaringType != null ? $"{defaultSchemaIdSelector(t.DeclaringType)}_{defaultSchemaIdSelector(t)}" : defaultSchemaIdSelector(t));
            });
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BiobrainWebContext db)
		{
			db.Database.Migrate();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			SeedDatabase(app);
			//app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.ConfigureRouting();

			app.RegisterStaticFiles(Configuration.GetSection(ConfigurationSections.StaticFolder).Value, Configuration.GetSection(ConfigurationSections.CacheFolder).Value);

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });

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
