using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Accounts.Queries
{
    [PublicAPI]
    public sealed class GetStudentAccountStateQuery : IQuery<GetStudentAccountStateQuery.Result>
    {
        public Guid UserId { get; init; }


        [PublicAPI]
        public record Result
        {
	        public bool IsFromArchivedSchool { get; init; }
	        public bool IsCountryFieldsFilled { get; init; }
	        public bool IsPaymentDetailsFilled { get; init; }
        }

        internal sealed class Validator : ValidatorBase<GetStudentAccountStateQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.UserId).ExistsInTable(Db.Students);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetStudentAccountStateQuery>
        {
            private readonly ISessionContext _sessionContext;
            
            public PermissionCheck(ISecurityService securityService, ISessionContext sessionContext) 
                : base(securityService)
                => _sessionContext = sessionContext;

            protected override bool CanExecute(GetStudentAccountStateQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return _sessionContext.GetUserId() == request.UserId;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetStudentAccountStateQuery, Result>
        {

            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetStudentAccountStateQuery request, CancellationToken cancellationToken)
            {
	            var student = await Db.Students.AsNoTracking()
		            .Include(_ => _.Schools).ThenInclude(_ => _.School)
		            .Include(_ => _.User).ThenInclude(_ => _.PaymentDetails)
		            .Where(_ => _.StudentId == request.UserId)
		            .FirstOrDefaultAsync(cancellationToken);

	            return new Result
	            {
		            IsFromArchivedSchool = student.Schools.Any() && student.Schools.All(s => s.School.Status == Constant.SchoolStatus.Archive || (s.School.EndDateUtc != null && s.School.EndDateUtc <= DateTime.UtcNow)),
		            IsCountryFieldsFilled = !string.IsNullOrEmpty(student.Country),
		            IsPaymentDetailsFilled = student.User.PaymentDetails != null && student.User.PaymentDetails.Any()
	            };
            }
        }
    }
}
