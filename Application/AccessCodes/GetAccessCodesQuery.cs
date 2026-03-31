using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.AccessCode;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.AccessCodes
{
    [PublicAPI]
    public sealed class GetAccessCodesQuery : IQuery<GetAccessCodesQuery.Result>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }


        [PublicAPI]
        public class Result
        {
            public List<Batch> Batches { get; init; }
            public int PageNumber { get; init; }
            public int PageSize { get; init; }
            public int TotalLength { get; set; }
        }


        [PublicAPI]
        public class Batch
        {
            public Guid BatchId { get; init; }
            public string BatchHeader { get; init; }
            public List<AccessCode> Codes { get; init; }
            public List<AccessCode> UsedCodes { get; init; }
            public DateTime ExpiryDateUtc { get; init; }
            public DateTime CreatedAtUtc { get; init; }
        }

        [PublicAPI]
        public class AccessCode
        {
            public Guid AccessCodeId { get; init; }
            public string Code { get; init; }
            public DateTime? UsedAtUtc { get; init; }
        }

        internal sealed class Validator : ValidatorBase<GetAccessCodesQuery>
        {
            public Validator(IDb db) : base(db)
            {
                //RuleFor(_ => _.RootContentNodeId).ExistsInTable(Db.ContentTree);
                RuleFor(_ => _.PageNumber).GreaterThan(0);
                RuleFor(_ => _.PageSize).GreaterThan(10);
            }
        }

        internal sealed class PermissionCheck : PermissionCheckBase<GetAccessCodesQuery>
        {
            public PermissionCheck(ISecurityService securityService) 
                : base(securityService)
            {
            }

            protected override bool CanExecute(GetAccessCodesQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetAccessCodesQuery, Result>
        {
            private readonly IAccessCodeService _accessCodeService;

            public Handler(IDb db, IAccessCodeService accessCodeService) : base(db) => _accessCodeService = accessCodeService;

            public override async Task<Result> Handle(GetAccessCodesQuery request, CancellationToken cancellationToken)
            {
                var batches = await Db.AccessCodeBatches.AsNoTracking()
                    .Include(_ => _.AccessCodes)
                    .Include(_ => _.UsedAccessCodes)
                    .Include(_ => _.Courses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
                    .Include(_ => _.Courses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Curriculum)
                    .OrderByDescending(_ => _.CreatedAt).ThenBy(_ => _.NumberOfCodes)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);
                var length = await Db.AccessCodeBatches.CountAsync(cancellationToken);

                return new Result
                {
                    PageSize = request.PageSize,
                    PageNumber = request.PageNumber,
                    TotalLength = length,
                    Batches = batches.Select(_ => new Batch
                    {
                        BatchHeader = _accessCodeService.GetBatchHeader(_), BatchId = _.AccessCodeBatchId,
                        CreatedAtUtc = _.CreatedAt,
                        ExpiryDateUtc = _.ExpiryDate ?? _.CreatedAt.AddMonths(13),
                        Codes = _.AccessCodes.Select(ac => new AccessCode
                            { AccessCodeId = ac.AccessCodeId, Code = ac.Code }).ToList(),
                        UsedCodes = _.UsedAccessCodes?.Select(ac => new AccessCode
                            { AccessCodeId = ac.AccessCodeId, Code = ac.Code, UsedAtUtc = ac.CreatedAt}).ToList(),
                    }).ToList()
                };
            }
        }
    }
}
