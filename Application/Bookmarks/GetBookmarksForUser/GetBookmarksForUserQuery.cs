using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Services.Domain.ContentTreeService;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Bookmarks.GetBookmarksForUser
{
    [PublicAPI]
    public sealed class GetBookmarksForUserQuery : IQuery<List<GetBookmarksForUserQuery.Result>>
    {
        public Guid UserId { get; init; }
        public Guid CourseId { get; set; }
        //public Guid? NodeId { get; set; }



        [PublicAPI]
        public record Result
        {
	        public Guid BookmarkId { get; set; }
	        public Guid MaterialId { get; set; }
	        public Guid NodeId { get; set; }
	        public Guid LevelId { get; set; }
	        public List<string> Path { get; set; }
	        public string Header { get; set; }
        }


        internal sealed class Validator : ValidatorBase<GetBookmarksForUserQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetBookmarksForUserQuery>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(GetBookmarksForUserQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return _sessionContext.GetUserId() == request.UserId;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetBookmarksForUserQuery, List<Result>>
        {
	        private readonly IContentTreePathResolver _contentTreePathResolver;
	        private readonly IContentTreeService _contentTreeService;
	        private readonly ITemplateService _templateService;

            public Handler(IDb db, IContentTreePathResolver contentTreePathResolver, IContentTreeService contentTreeService, ITemplateService templateService) : base(db)
            {
	            _contentTreePathResolver = contentTreePathResolver;
	            _contentTreeService = contentTreeService;
	            _templateService = templateService;
            }

            public override async Task<List<Result>> Handle(GetBookmarksForUserQuery request, CancellationToken cancellationToken)
            {
	            var bookmarks = await Db.Bookmarks.AsNoTracking()
		            .Include(x => x.Material)
		            .Where(_ => _.UserId == request.UserId && _.CourseId == request.CourseId)
		            .OrderByDescending(_ => _.CreatedAt)
		            .ToListAsync(cancellationToken);
	            var courseStructure = await _contentTreeService.GetCourseStructure(request.CourseId, cancellationToken);
                var pages = await Db.Pages.AsNoTracking()
		            .Include(_ => _.ContentTreeNode).ThenInclude(_ => _.ParentContentTree)
		            .Include(_ => _.PageMaterials)
		            .Where(_ => _.ContentTreeNode.CourseId == request.CourseId)
		            .ToListAsync(cancellationToken);
                var courseTemplate = await Db.CourseTemplates.AsNoTracking()
	                .Include(_ => _.Template)
	                .Where(_ => _.CourseId == request.CourseId &&
	                            _.Template.Type == Constant.TemplateType.BookmarkPathHeader)
	                .SingleAsync(cancellationToken);
                var result = new List<Result>();

	            foreach (var bookmark in bookmarks.OrderByDescending(_ => _.CreatedAt))
	            {
		            var page = pages.FirstOrDefault(_ => _.PageMaterials.Any(m => m.MaterialId == bookmark.MaterialId));
                    if(page == null /*|| (request.NodeId != null && page.ContentTreeId != request.NodeId)*/) continue;
                    var path = _contentTreePathResolver.ResolveFullPath(page.ContentTreeId, courseStructure);

                    result.Add(new Result
                    {
                        BookmarkId = bookmark.BookmarkId,
                        Path = _contentTreePathResolver.ResolveFullPath(page.ContentTreeId, courseStructure).OrderBy(x => x.Index).Select(x => x.Value).ToList(),
                        Header = $"{_templateService.ApplyTemplate(courseTemplate.Template.Value, path.Select(x => new TemplateValue { Index = x.Index, Name = x.Value }).ToList())} > {bookmark.Material.Header}",
                        MaterialId = bookmark.MaterialId,
                        NodeId = page.ContentTreeNode.ParentId ?? Guid.Empty,
                        LevelId = page.ContentTreeId
                    });
	            }

				return result;
            }
        }
    }
}
