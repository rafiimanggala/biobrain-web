using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using FluentValidation;
using JetBrains.Annotations;

namespace Biobrain.Application.Teachers.UpdateTeacherDetails
{
    [PublicAPI]
    public class UpdateTeacherDetailsCommand : ICommand<UpdateTeacherDetailsCommand.Result>
    {
        public Guid TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid TeacherId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }


        internal class Validator : ValidatorBase<UpdateTeacherDetailsCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
                RuleFor(_ => _.FirstName).NotEmpty();
                RuleFor(_ => _.LastName).NotEmpty();
            }
        }


        internal class PermissionCheck: PermissionCheckBase<UpdateTeacherDetailsCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(UpdateTeacherDetailsCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                if (user.IsTeacherAccountOwner(request.TeacherId)) return true;

                //var teacher = _db.Teachers.GetSingle(TeacherSpec.ById(request.TeacherId));
                //if (user.IsSchoolAdmin(teacher.SchoolId)) return true;
                
                return false;
            }
        }


        internal class Handler : CommandHandlerBase<UpdateTeacherDetailsCommand, Result>
        {
            private readonly IRefreshClaimsService _refreshClaimsService;

            public Handler(IDb db, IRefreshClaimsService refreshClaimsService) : base(db) => _refreshClaimsService = refreshClaimsService;

            public override async Task<Result> Handle(UpdateTeacherDetailsCommand request, CancellationToken cancellationToken)
            {
                var teacher = await Db.Teachers.GetSingleAsync(TeacherSpec.ById(request.TeacherId), cancellationToken);
                teacher.FirstName = request.FirstName;
                teacher.LastName = request.LastName;

                await Db.SaveChangesAsync(cancellationToken);
                await _refreshClaimsService.RefreshClaims(teacher.TeacherId);

                return new Result
                       {
                           TeacherId = teacher.TeacherId,
                           FirstName = teacher.FirstName,
                           LastName = teacher.LastName
                       };
            }
        }
    }
}
