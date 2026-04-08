using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.UpdateMaterial
{
    [PublicAPI]
    public sealed class UpdateMaterialCommand : ICommand<UpdateMaterialCommand.Result>
    {
        public Guid MaterialId { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public string VideoLink { get; set; }

        [PublicAPI]
        public sealed class Result;

        internal class Validator : ValidatorBase<UpdateMaterialCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.MaterialId).ExistsInTable(Db.Materials);
                RuleFor(_ => _.Header).NotEmpty().MaximumLength(500);
                RuleFor(_ => _.Text).NotEmpty();
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<UpdateMaterialCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(UpdateMaterialCommand request, IUserSecurityInfo user)
                => user.IsApplicationAdmin();
        }

        internal sealed class Handler : CommandHandlerBase<UpdateMaterialCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
            {
                var material = await Db.Materials
                    .Where(x => x.MaterialId == request.MaterialId && x.DeletedAt == null)
                    .FirstAsync(cancellationToken);

                material.Header = request.Header?.Trim() ?? "";
                material.Text = request.Text ?? "";
                material.VideoLink = request.VideoLink?.Trim() ?? "";

                await Db.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
