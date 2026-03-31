using System.IO;
using System.Net;
using BiobrainWebAPI.Values;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace BiobrainWebAPI.Core.Configurations
{
    public static class ConfigureStaticFilesExtension
    {
        public static IApplicationBuilder RegisterStaticFiles(this IApplicationBuilder app, string staticFolderName, string cacheFolderName)
        {
            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            app.UseDefaultFiles(options);
            if (!string.IsNullOrEmpty(staticFolderName))
            {
	            var staticFolderPath = Path.Combine(Directory.GetCurrentDirectory(), staticFolderName);
	            if (!Directory.Exists(staticFolderPath))
		            Directory.CreateDirectory(staticFolderPath);

	            app.UseStaticFiles(new StaticFileOptions
	            {
		            FileProvider = new PhysicalFileProvider(staticFolderPath),
		            RequestPath = "/" + AppSettings.StaticFolderLink
	            });

	            // ImagesFolder
	            var imagesPath = Path.Combine(staticFolderPath, AppSettings.ImagesFolderLink);
	            if (!Directory.Exists(imagesPath))
		            Directory.CreateDirectory(imagesPath);
	            app.UseStaticFiles(new StaticFileOptions
	            {
		            FileProvider = new PhysicalFileProvider(imagesPath),
		            RequestPath = "/" + AppSettings.ImagesFolderLink
                });

                // UserGuideImagesFolder
                var userGuideImagesPath = Path.Combine(staticFolderPath, AppSettings.UserGuideImagesFolderLink);
                if (!Directory.Exists(userGuideImagesPath))
                    Directory.CreateDirectory(userGuideImagesPath);
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(userGuideImagesPath),
                    RequestPath = "/" + AppSettings.UserGuideImagesFolderLink
                });

                var contentPath = Path.Combine(staticFolderPath, AppSettings.ContentFolderLink);
                if (!Directory.Exists(contentPath))
                    Directory.CreateDirectory(contentPath);
			}

            if (!string.IsNullOrEmpty(cacheFolderName))
			{
				var cacheFolderPath = Path.Combine(Directory.GetCurrentDirectory(), cacheFolderName);
				if (!Directory.Exists(cacheFolderPath))
					Directory.CreateDirectory(cacheFolderPath);

				var reportPath = Path.Combine(cacheFolderPath, AppSettings.ReportFolderLink);
				if (!Directory.Exists(reportPath))
					Directory.CreateDirectory(reportPath);

				app.UseStaticFiles(new StaticFileOptions
				{
					FileProvider = new PhysicalFileProvider(reportPath),
					RequestPath = "/" + AppSettings.ReportFolderLink,
					//OnPrepareResponse = OnPrepareResponseForStaticFile
				});
			}

            // FontsFolder
			var fontsPath = Path.Combine(Directory.GetCurrentDirectory(), AppSettings.RootFolderLink, AppSettings.FontsFolderLink);
            if (!Directory.Exists(fontsPath))
	            Directory.CreateDirectory(fontsPath);
            app.UseStaticFiles(new StaticFileOptions
            {
	            FileProvider = new PhysicalFileProvider(fontsPath),
	            RequestPath = "/" + AppSettings.FontsFolderLink
            });

            app.UseStaticFiles();

            return app;
        }

        private static void OnPrepareResponseForStaticFile(StaticFileResponseContext context)
		{
			if ((context.Context.User.Identity?.IsAuthenticated ?? false) == false)
			{
				// respond HTTP 401 Unauthorized.
				context.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			}
		}
    }
}