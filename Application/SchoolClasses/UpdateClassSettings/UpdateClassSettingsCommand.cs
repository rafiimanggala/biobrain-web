using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;


namespace Biobrain.Application.SchoolClasses.UpdateClassSettings
{
    [PublicAPI]
    public sealed class UpdateClassSettingsCommand : ICommand<UpdateClassSettingsCommand.Result>
    {
        public Guid SchoolClassId { get; init; }
        public bool HintsDisabled { get; init; }
        public bool SoundDisabled { get; init; }


        [PublicAPI]
        public sealed class Result
        {
            public bool HintsDisabled { get; init; }
            public bool SoundDisabled { get; init; }
        }


        internal sealed class Validator : ValidatorBase<UpdateClassSettingsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolClassId).ExistsInTable(Db.SchoolClasses);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<UpdateClassSettingsCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService) => _db = db;

            protected override bool CanExecute(UpdateClassSettingsCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));

                if (user.IsSchoolAdmin(schoolClass.SchoolId))
                    return true;

                if (user.IsSchoolTeacher(schoolClass.SchoolId))
                    return true;

                return false;
            }
        }


        internal sealed class Handler : CommandHandlerBase<UpdateClassSettingsCommand, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<Result> Handle(UpdateClassSettingsCommand request, CancellationToken cancellationToken)
            {
                var schoolClass = await Db.SchoolClasses
                                          .GetSingleAsync(SchoolClassSpec.ById(request.SchoolClassId), cancellationToken);

                schoolClass.HintsDisabled = request.HintsDisabled;
                schoolClass.SoundDisabled = request.SoundDisabled;

                await Db.SaveChangesAsync(cancellationToken);

                return new Result
                {
                    HintsDisabled = schoolClass.HintsDisabled,
                    SoundDisabled = schoolClass.SoundDisabled,
                };
            }
        }
    }
}
