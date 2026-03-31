using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.WhatsNew;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.WhatsNew.CreateWhatsNew
{
    [PublicAPI]
    public sealed class CreateWhatsNewCommand : IQuery<CreateWhatsNewCommand.Result>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Version { get; set; }

        [PublicAPI]
        public record Result
        {
            public Guid WhatsNewId { get; set; }
        }

        internal sealed class Validator : ValidatorBase<CreateWhatsNewCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Title).NotEmpty().MaximumLength(256);
                RuleFor(_ => _.Content).NotEmpty();
                RuleFor(_ => _.Version).NotEmpty().MaximumLength(32);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<CreateWhatsNewCommand>
        {
            public PermissionCheck(ISecurityService securityService)
                : base(securityService)
            {
            }

            protected override bool CanExecute(CreateWhatsNewCommand request, IUserSecurityInfo user)
            {
                return user.IsApplicationAdmin();
            }
        }

        internal sealed class Handler : QueryHandlerBase<CreateWhatsNewCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(CreateWhatsNewCommand request, CancellationToken cancellationToken)
            {
                var entity = new WhatsNewEntity
                {
                    Title = request.Title,
                    Content = request.Content,
                    Version = request.Version,
                    PublishedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await Db.WhatsNew.AddAsync(entity, cancellationToken);
                await Db.SaveChangesAsync(cancellationToken);

                return new Result { WhatsNewId = entity.WhatsNewId };
            }
        }
    }
}
