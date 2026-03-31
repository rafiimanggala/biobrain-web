using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using Biobrain.Application.Common.Validators;
using FluentValidation;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using MimeTypes;
using System.IO;
using Microsoft.Extensions.Logging;
using BiobrainWebAPI.Values;
using Microsoft.Extensions.Configuration;

namespace Biobrain.Application.UserGuide.UploadUserGuideImageCommand
{
    public sealed partial class UploadUserGuideImageCommand: ICommand<UploadUserGuideImageCommand.Result>
    {
        public IFormFile File { get; init; }
        public Guid FileId { get; set; }

        public record Result(string FileLink);

        internal sealed class Validator : ValidatorBase<UploadUserGuideImageCommand>
        {
            public Validator(IDb db)
                : base(db)
            {
                RuleFor(_ => _.File).Must(BeSafeFileType);
            }

            private static bool BeSafeFileType(IFormFile file)
            {
                var fileExtension = MimeTypeMap.GetExtension(file.ContentType);
                return !DangerousFileExtensionProvider.Bucket.Contains(fileExtension);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<UploadUserGuideImageCommand>
        {
            private readonly ISessionContext _sessionContext;

            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext)
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(UploadUserGuideImageCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return false;
            }
        }

        internal sealed class Handler : CommandHandlerBase<UploadUserGuideImageCommand, Result>
        {
            private readonly ILogger _logger;
            private readonly IConfiguration _configuration;

            public Handler(IDb db,
                ILogger<UploadUserGuideImageCommand> logger,
                IConfiguration configuration)
                : base(db)
            {
                _logger = logger;
                _configuration = configuration;
            }

            public override async Task<Result> Handle(UploadUserGuideImageCommand request, CancellationToken ct)
            {
                var fileName = await SaveFile(request.File, Guid.NewGuid(), ct);

                return new Result($"/{AppSettings.UserGuideImagesFolderLink}/{fileName}");
            }

            public async Task<string> SaveFile(IFormFile file, Guid fileId, CancellationToken ct)
            {
                _logger.LogInformation($"Save file Name = '{file.FileName}' is starting.");

                var extension = MimeTypeMap.GetExtension(file.ContentType);
                var fileName = $"{fileId}{extension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                    _configuration.GetSection(ConfigurationSections.StaticFolder).Value,
                    AppSettings.UserGuideImagesFolderLink, fileName);

                _logger.LogInformation($"Save physical file to location '{filePath}' is starting.");
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream, ct);
                _logger.LogInformation($"Save physical file to location '{filePath}' has completed.");

                _logger.LogInformation($"Save file Id = '{fileId}'; Name = '{file.FileName}' has completed.");
                return fileName;
            }
        }
    }
}