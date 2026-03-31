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

namespace Biobrain.Application.Content.GetContentTreeMeta
{
    [PublicAPI]
    public sealed class GetContentTreeMetaByCourseIdQuery : IQuery<List<GetContentTreeMetaByCourseIdQuery.Result>>
    {
	    public Guid CourseId { get; init; }

        [PublicAPI]
        public record Result
        {
            public Guid ContentTreeMetaId { get; init; }
            public long Depth { get; init; }
            public string Name { get; init; }
            public bool IsCanAddChildren { get; set; }
            public bool IsCanAttachContent { get; set; }
            public bool IsCanCopyIn { get; set; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetContentTreeMetaByCourseIdQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetContentTreeMetaByCourseIdQuery request, IUserSecurityInfo user) =>
                // ToDo: Has access to course
                true;
        }


        internal sealed class Handler : QueryHandlerBase<GetContentTreeMetaByCourseIdQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetContentTreeMetaByCourseIdQuery request, CancellationToken cancellationToken)
            {
	            return Db.ContentTreeMeta
		            .Where(x => x.CourseId == request.CourseId && x.DeletedAt == null)
		            .AsNoTracking()
		            .Select(_ => new Result
		            {
			            ContentTreeMetaId = _.ContentTreeMetaId,
			            Name = _.Name,
			            Depth = _.Depth,
                        IsCanAddChildren = _.CouldAddEntry,
                        IsCanAttachContent = _.CouldAddContent,
                        IsCanCopyIn = _.CouldCopyIn
		            })
		            .OrderBy(x => x.Depth)
		            .ToListAsync(cancellationToken);
            }
        }
    }
}