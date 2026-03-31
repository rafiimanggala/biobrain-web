using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Templates
{
    [PublicAPI]
    public class DeleteTemplateCommand : ICommand<DeleteTemplateCommand.Result>
    {
        public Guid TemplateId { get; set; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<DeleteTemplateCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.TemplateId).ExistsInTable(Db.Templates);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<DeleteTemplateCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(DeleteTemplateCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<DeleteTemplateCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
            {
                var template = await Db.Templates.Where(_ => _.TemplateId == request.TemplateId)
                    .FirstAsync(cancellationToken);
                Db.Templates.Remove(template);
                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
