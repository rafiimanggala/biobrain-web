using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.ContentImages
{
    [PublicAPI]
    public class GetContentImagesQuery : ICommand<GetContentImagesQuery.Result>
    {
        public string Search { get; set; } = "";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        [PublicAPI]
        public class Result
        {
            public List<ContentImageItem> Images { get; set; } = new();
            public int TotalCount { get; set; }
        }

        [PublicAPI]
        public class ContentImageItem
        {
            public Guid ImageId { get; set; }
            public string Code { get; set; } = "";
            public string FileName { get; set; } = "";
            public string Description { get; set; } = "";
            public string FileLink { get; set; } = "";
            public long FileSize { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        internal class PermissionCheck : PermissionCheckBase<GetContentImagesQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GetContentImagesQuery request, IUserSecurityInfo user)
                => user.IsApplicationAdmin();
        }

        internal class Handler : CommandHandlerBase<GetContentImagesQuery, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetContentImagesQuery request, CancellationToken cancellationToken)
            {
                var query = Db.ContentImages.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    query = query.Where(i =>
                        i.Code.ToLower().Contains(search) ||
                        i.Description.ToLower().Contains(search));
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var images = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(i => new ContentImageItem
                    {
                        ImageId = i.ImageId,
                        Code = i.Code,
                        FileName = i.FileName,
                        Description = i.Description,
                        FileLink = $"/images/{i.FileName}",
                        FileSize = i.FileSize,
                        CreatedAt = i.CreatedAt,
                    })
                    .ToListAsync(cancellationToken);

                return new Result
                {
                    Images = images,
                    TotalCount = totalCount,
                };
            }
        }
    }
}
