using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.AccessCode;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.AccessCodes
{
    [PublicAPI]
    public class UpdateAccessCodeBatchExpiryDateCommand : ICommand<UpdateAccessCodeBatchExpiryDateCommand.Result>
    {
        public Guid BatchId { get; set; }
        public DateTime ExpiryDate { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid BatchId { get; init; }
            public string BatchHeader { get; init; }
            public List<AccessCode> Codes { get; init; }
            public DateTime CreatedAtUtc { get; init; }
        }

        [PublicAPI]
        public class AccessCode
        {
            public Guid AccessCodeId { get; init; }
            public string Code { get; init; }
        }


        internal class Validator : ValidatorBase<UpdateAccessCodeBatchExpiryDateCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.BatchId).ExistsInTable(Db.AccessCodeBatches);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<UpdateAccessCodeBatchExpiryDateCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(UpdateAccessCodeBatchExpiryDateCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<UpdateAccessCodeBatchExpiryDateCommand, Result>
        {
            private readonly IAccessCodeService _accessCodeService;
            public Handler(IDb db, IAccessCodeService accessCodeService) : base(db) => _accessCodeService = accessCodeService;

            public override async Task<Result> Handle(UpdateAccessCodeBatchExpiryDateCommand request, CancellationToken cancellationToken)
            {
                var batch = await Db.AccessCodeBatches
                    .Include(_ => _.AccessCodes)
                    .Include(_ => _.Courses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
                    .Include(_ => _.Courses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Curriculum)
                    .Where(_ => _.AccessCodeBatchId == request.BatchId).FirstOrDefaultAsync(cancellationToken);
                batch.ExpiryDate = request.ExpiryDate;
                

                await Db.SaveChangesAsync(cancellationToken);

                return new Result
                {
                    BatchHeader = _accessCodeService.GetBatchHeader(batch),
                    BatchId = batch.AccessCodeBatchId,
                    Codes = batch.AccessCodes
                        .Select(_ => new AccessCode { AccessCodeId = _.AccessCodeId, Code = _.Code }).ToList(),
                    CreatedAtUtc = batch.CreatedAt
                };
            }
        }
    }
}