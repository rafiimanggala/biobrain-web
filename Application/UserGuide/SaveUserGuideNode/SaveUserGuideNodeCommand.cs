using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.UserGuide;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.UserGuide.SaveUserGuideNode
{
    [PublicAPI]
    public sealed class SaveUserGuideNodeCommand : IQuery<SaveUserGuideNodeCommand.Result>
    {
        public Guid? NodeId { get; init; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public bool IsAvailableForStudent { get; set; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<SaveUserGuideNodeCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Name).NotNull().NotEmpty();
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<SaveUserGuideNodeCommand>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(SaveUserGuideNodeCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<SaveUserGuideNodeCommand, Result>
        {
	        public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(SaveUserGuideNodeCommand request, CancellationToken cancellationToken)
            {
                if (request.NodeId == null)
                {
                    var subTree = await Db.UserGuideContentTree.Where(_ => _.ParentId == request.ParentId)
                        .ToListAsync(cancellationToken);
                    var maxOrder = subTree.Any() ? subTree.Max(_ => _.Order) : -1;
                    await Db.UserGuideContentTree.AddAsync(new UserGuideContentTreeEntity
                    {
                        Name = request.Name,
                        ParentId = request.ParentId,
                        Order = maxOrder + 1,
                        IsAvailableForStudent = request.IsAvailableForStudent,

                    }, cancellationToken);
                }
                else
                {
                    var node = await Db.UserGuideContentTree.Where(_ => _.NodeId == request.NodeId)
                        .GetSingleAsync(cancellationToken);

                    node.Name = request.Name;
                    node.ParentId = request.ParentId;
                    node.IsAvailableForStudent = request.IsAvailableForStudent;
                }

                await Db.SaveChangesAsync(cancellationToken);
				return new Result
				{
                };
            }
        }
    }
}
