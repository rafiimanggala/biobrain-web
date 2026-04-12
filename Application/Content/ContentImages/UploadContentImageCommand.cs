using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using BiobrainWebAPI.Values;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeTypes;

namespace Biobrain.Application.Content.ContentImages
{
    [PublicAPI]
    public class UploadContentImageCommand : ICommand<UploadContentImageCommand.Result>
    {
        public IFormFile File { get; init; }
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";

        [PublicAPI]
        public class Result
        {
            public Guid ImageId { get; set; }
            public string FileLink { get; set; } = "";
            public string Code { get; set; } = "";
        }

        internal class Validator : ValidatorBase<UploadContentImageCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.File).NotNull();
                RuleFor(_ => _.Code).NotEmpty().MaximumLength(100);
                RuleFor(_ => _.File).Must(BeSafeFileType).When(_ => _.File != null);
            }

            private static bool BeSafeFileType(IFormFile file)
            {
                var ext = MimeTypeMap.GetExtension(file.ContentType);
                return !DangerousFileExtensionProvider.Bucket.Contains(ext);
            }
        }

        internal class PermissionCheck : PermissionCheckBase<UploadContentImageCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(UploadContentImageCommand request, IUserSecurityInfo user)
                => user.IsApplicationAdmin();
        }

        internal class Handler : CommandHandlerBase<UploadContentImageCommand, Result>
        {
            private readonly ISessionContext _sessionContext;
            private readonly IConfiguration _configuration;

            public Handler(IDb db, ISessionContext sessionContext, IConfiguration configuration) : base(db)
            {
                _sessionContext = sessionContext;
                _configuration = configuration;
            }

            public override async Task<Result> Handle(UploadContentImageCommand request, CancellationToken cancellationToken)
            {
                var imageId = Guid.NewGuid();
                var extension = MimeTypeMap.GetExtension(request.File.ContentType);
                var fileName = $"{imageId}{extension}";

                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    _configuration.GetSection(ConfigurationSections.StaticFolder).Value ?? "static",
                    AppSettings.ImagesFolderLink,
                    fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await request.File.CopyToAsync(stream, cancellationToken);

                var entity = new ContentImageEntity
                {
                    ImageId = imageId,
                    Code = request.Code.Trim(),
                    FileName = fileName,
                    Description = request.Description?.Trim() ?? "",
                    ContentType = request.File.ContentType,
                    FileSize = request.File.Length,
                    UploadedByUserId = _sessionContext.GetUserId(),
                };

                await Db.ContentImages.AddAsync(entity, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);

                return new Result
                {
                    ImageId = imageId,
                    FileLink = $"/{AppSettings.ImagesFolderLink}/{fileName}",
                    Code = entity.Code,
                };
            }
        }
    }
}
