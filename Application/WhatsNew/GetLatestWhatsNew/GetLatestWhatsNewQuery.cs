using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.WhatsNew.GetLatestWhatsNew
{
    [PublicAPI]
    public sealed class GetLatestWhatsNewQuery : IQuery<GetLatestWhatsNewQuery.Result>
    {
        [PublicAPI]
        public record Result
        {
            public Guid? WhatsNewId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public string Version { get; set; }
            public DateTime? PublishedAt { get; set; }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetLatestWhatsNewQuery>
        {
            public PermissionCheck(ISecurityService securityService)
                : base(securityService)
            {
            }

            protected override bool CanExecute(GetLatestWhatsNewQuery request, IUserSecurityInfo user)
            {
                return user.IsApplicationAdmin() || user.IsTeacher() || user.IsStudent();
            }
        }

        internal sealed class Handler : QueryHandlerBase<GetLatestWhatsNewQuery, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetLatestWhatsNewQuery request, CancellationToken cancellationToken)
            {
                var entry = await Db.WhatsNew.AsNoTracking()
                    .Where(_ => _.IsActive)
                    .OrderByDescending(_ => _.PublishedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entry == null)
                {
                    return new Result();
                }

                return new Result
                {
                    WhatsNewId = entry.WhatsNewId,
                    Title = entry.Title,
                    Content = entry.Content,
                    Version = entry.Version,
                    PublishedAt = entry.PublishedAt
                };
            }
        }
    }
}
