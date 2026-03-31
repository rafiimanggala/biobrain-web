using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Content.Services.ContentCacheService;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.IncrementContentVersion
{
    [PublicAPI]
    public class IncrementContentVersionCommand : ICommand<IncrementContentVersionCommand.Result>
    {
	    public Guid CourseId { get; set; }

	    [PublicAPI]
	    public class Result
	    {
		    public long Version { get; set; }
	    }


		internal class Validator : ValidatorBase<IncrementContentVersionCommand>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck: PermissionCheckBase<IncrementContentVersionCommand>
        {
            private readonly IDb db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => this.db = db;

            protected override bool CanExecute(IncrementContentVersionCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                return false;
                //return true;
            }
        }


        internal class Handler : CommandHandlerBase<IncrementContentVersionCommand, Result>
        {
	        private readonly IContentCacheService _contentCacheService;

			public Handler(IDb db, IContentCacheService contentCacheService)
				: base(db)
                => _contentCacheService = contentCacheService;

            public override async Task<Result> Handle(IncrementContentVersionCommand request,
                                                      CancellationToken cancellationToken)
			{
				var version = await Db.ContentVersion
					.Where(x => x.CourseId == request.CourseId)
					.OrderByDescending(x => x.Version)
					.FirstOrDefaultAsync(cancellationToken) ?? new ContentVersionEntity();

				var dateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				version.ContentFileName = await _contentCacheService.CreateContentFile(request.CourseId, dateTime, cancellationToken);
				version.Version = dateTime;

				if (version.ContentVersionId == Guid.Empty)
				{
					version.CourseId = request.CourseId;
					await Db.ContentVersion.AddAsync(version, cancellationToken);
				}

				await Db.SaveChangesAsync(cancellationToken);

				return new Result{Version = version.Version};
			}

		}
    }
}
