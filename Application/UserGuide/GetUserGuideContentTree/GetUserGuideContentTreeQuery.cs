using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.UserGuide.GetUserGuideContentTree
{
    [PublicAPI]
    public sealed class GetUserGuideContentTreeQuery : IQuery<List<GetUserGuideContentTreeQuery.Result>>
    {

        [PublicAPI]
        public record Result
        {
	        public Guid NodeId { get; set; }
	        public Guid? ParentId { get; set; }
	        public string Name { get; set; }
	        public long Order { get; set; }
            public bool IsAvailableForStudent { get; set; }
	        public List<Result> Children { get; set; }
        }


        internal sealed class Validator : ValidatorBase<GetUserGuideContentTreeQuery>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetUserGuideContentTreeQuery>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(GetUserGuideContentTreeQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin() || user.IsTeacher() || user.IsStudent())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetUserGuideContentTreeQuery, List<Result>>
        {
            private readonly ISecurityService _securityService;
            public Handler(IDb db, ISecurityService securityService) : base(db) => _securityService = securityService;

            public override async Task<List<Result>> Handle(GetUserGuideContentTreeQuery request, CancellationToken cancellationToken)
            {
                var result = new List<Result>();
                var nodes = await Db.UserGuideContentTree.AsNoTracking()
                    .OrderBy(_ => _.ParentId)
                    .ThenBy(_ => _.Order)
                    .ToListAsync(cancellationToken);
                var user = await _securityService.GetCurrentUserSecurityInfo();

                foreach (var node in nodes.Where(_ => _.ParentId == null))
                {
                    if(user.IsStudent() && !node.IsAvailableForStudent) continue;

                    var model = new Result
                    {
                        NodeId = node.NodeId,
                        ParentId = node.ParentId,
                        Name = node.Name,
                        Order = node.Order,
                        IsAvailableForStudent = node.IsAvailableForStudent,
                        Children = new List<Result>()
                    };
                    foreach (var subNode in nodes.Where(_ => _.ParentId == node.NodeId))
                    {
                        model.Children.Add(new Result
                        {
                            NodeId = subNode.NodeId,
                            ParentId = subNode.ParentId,
                            Name = subNode.Name,
                            Order = subNode.Order,
                            IsAvailableForStudent = node.IsAvailableForStudent,
                        });
                    }
                    result.Add(model);
                }

				return result;
            }
        }
    }
}
