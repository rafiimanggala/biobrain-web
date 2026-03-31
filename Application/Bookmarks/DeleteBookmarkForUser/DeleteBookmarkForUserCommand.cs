using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Bookmarks.DeleteBookmarkForUser
{
    [PublicAPI]
    public sealed class DeleteBookmarkForUserCommand : IQuery<DeleteBookmarkForUserCommand.Result>
    {
        public Guid UserId { get; init; }
        public Guid BookmarkId { get; set; }


        [PublicAPI]
        public record Result;


        internal sealed class Validator : ValidatorBase<DeleteBookmarkForUserCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.BookmarkId).ExistsInTable(Db.Bookmarks);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<DeleteBookmarkForUserCommand>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(DeleteBookmarkForUserCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return _sessionContext.GetUserId() == request.UserId;
            }
        }


        internal sealed class Handler : QueryHandlerBase<DeleteBookmarkForUserCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteBookmarkForUserCommand request, CancellationToken cancellationToken)
            {
	            var bookmark = await Db.Bookmarks.SingleAsync(_ => _.BookmarkId == request.BookmarkId, cancellationToken);
				Db.Bookmarks.Remove(bookmark);
	            await Db.SaveChangesAsync(cancellationToken);

				return new Result();
            }
        }
    }
}
