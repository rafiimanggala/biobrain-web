using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Material;
using JetBrains.Annotations;

namespace Biobrain.Application.Bookmarks.CreateBookmarkForUser
{
    [PublicAPI]
    public sealed class CreateBookmarkForUserCommand : IQuery<CreateBookmarkForUserCommand.Result>
    {
        public Guid UserId { get; init; }
        public Guid CourseId { get; set; }
        public Guid MaterialId { get; set; }


        [PublicAPI]
        public record Result
        {
	        public Guid BookmarkId { get; set; }
	        public Guid MaterialId { get; set; }
	        public Guid CourseId { get; set; }
        }


        internal sealed class Validator : ValidatorBase<CreateBookmarkForUserCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                RuleFor(_ => _.MaterialId).ExistsInTable(Db.Materials);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<CreateBookmarkForUserCommand>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(CreateBookmarkForUserCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return _sessionContext.GetUserId() == request.UserId;
            }
        }


        internal sealed class Handler : QueryHandlerBase<CreateBookmarkForUserCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(CreateBookmarkForUserCommand request, CancellationToken cancellationToken)
            {
	            var bookmark = await Db.Bookmarks.AddAsync(new BookmarkEntity
	            {
                    CourseId = request.CourseId,
                    UserId = request.UserId,
                    MaterialId = request.MaterialId
	            }, cancellationToken);
	            await Db.SaveChangesAsync(cancellationToken);

				return new Result
				{
					BookmarkId = bookmark.Entity.BookmarkId,
					CourseId = bookmark.Entity.CourseId,
					MaterialId = bookmark.Entity.MaterialId
                };
            }
        }
    }
}
