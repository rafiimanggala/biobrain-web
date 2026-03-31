using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Student;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.GetStudents
{
    [PublicAPI]
    public sealed class GetStudentsListQuery : IQuery<List<GetStudentsListQuery.Result>>
    {
        public DateTime FromDateUtc { get; set; }
        public DateTime ToDateUtc { get; set; }


        [PublicAPI]
        public record Result
        {
            public Guid StudentId { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string Email { get; set; }
            public DateTime RegisteredAt { get; set; }
            public DateTime? SubscribedAt { get; set; }
            public List<string> LicenseTypes { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetStudentsListQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetStudentsListQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetStudentsListQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken)
            {
                return Db.Students.AsNoTracking()
                         .Where(StudentSpec.ForDates(request.FromDateUtc, request.ToDateUtc))
                         .Include(x => x.User).ThenInclude(_ => _.ScheduledPayments).ThenInclude(_ => _.ScheduledPaymentCourses)
                         .Include(_ => _.Schools)
                         .Select(_ => new Result
                                      {
                                          StudentId = _.StudentId,
                                          FirstName = _.FirstName,
                                          LastName = _.LastName,
                                          Email = _.User.Email,
                                          RegisteredAt = _.CreatedAt,
                                          LicenseTypes = GetLicenseTypes(_),
                                          SubscribedAt = GetSubscribedAt(_)


                         })
                         .OrderByDescending(_ => _.RegisteredAt).ThenBy(_ => _.FirstName).ThenBy(_ => _.LastName)
                         .ToListAsync(cancellationToken);
            }

            private static List<string> GetLicenseTypes(StudentEntity student)
            {
                var result = new List<string>();

                if(student.Schools.Any()) result.Add("School");

                var subscriptions = student.User.ScheduledPayments.Where(_ =>
                    _.Status == ScheduledPaymentStatus.StoppedByUser || _.Status == ScheduledPaymentStatus.Success).ToList();
                if (subscriptions.Any())
                {
                    if(subscriptions.Any(_ => _.Type == ScheduledPaymentType.FreeTrial))
                        result.Add("Free Trial");
                    if(subscriptions.Any(_ => _.Type == ScheduledPaymentType.Recurring))
                        result.Add("Paid Subscription");
                    if (subscriptions.Any(_ => _.Type == ScheduledPaymentType.AccessCode))
                        result.Add("Access code");
                    if (subscriptions.Any(_ => _.Type == ScheduledPaymentType.Voucher))
                        result.Add("Voucher");
                }

                return result;
            }

            private static DateTime? GetSubscribedAt(StudentEntity student)
            {
                var subscriptions = student.User.ScheduledPayments.Where(_ =>
                    _.Status == ScheduledPaymentStatus.StoppedByUser || _.Status == ScheduledPaymentStatus.Success)
                    .Where(_ => _.Type != ScheduledPaymentType.FreeTrial).ToList();
                if (subscriptions.Any())
                    return subscriptions.Min(_ => _.CreatedAt);

                return null;
            }
        }
    }
}
