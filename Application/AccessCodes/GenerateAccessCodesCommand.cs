using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.AccessCode;
using Biobrain.Domain.Entities.AccessCodes;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.AccessCodes
{
    [PublicAPI]
    public class GenerateAccessCodesCommand : ICommand<GenerateAccessCodesCommand.Result>
    {
        public string Note { get; set; }
        public List<Guid> CourseIds { get; set; }
        public int NumberOfCodes { get; set; }
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


        internal class Validator : ValidatorBase<GenerateAccessCodesCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleForEach(_ => _.CourseIds).ExistsInTable(db.Courses);
                RuleFor(_ => _.NumberOfCodes).GreaterThan(0);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GenerateAccessCodesCommand>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(GenerateAccessCodesCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<GenerateAccessCodesCommand, Result>
        {
            private readonly IAccessCodeService _accessCodeService;
            public Handler(IDb db, IAccessCodeService accessCodeService) : base(db) => _accessCodeService = accessCodeService;

            public override async Task<Result> Handle(GenerateAccessCodesCommand request, CancellationToken cancellationToken)
            {
                var batch = await Db.AccessCodeBatches.AddAsync(new AccessCodeBatchEntity { Note = request.Note ?? "", NumberOfCodes = request.NumberOfCodes, ExpiryDate = request.ExpiryDate}, cancellationToken);
                await Db.AccessCodeBatchCourses.AddRangeAsync(request.CourseIds.Select(_ => new AccessCodeBatchCourseEntity{AccessCodeBatchId = batch.Entity.AccessCodeBatchId, CourseId = _}), cancellationToken);
                
                var codes = new List<string>();
                for (var i = 0; i < request.NumberOfCodes; i++)
                {
                    var result = await _accessCodeService.TryGetNewAccessCode();
                    while (!result.Success)
                    {
                        result = await _accessCodeService.TryGetNewAccessCode();
                    }
                    codes.Add(result.Code);
                }

                await Db.AccessCodes.AddRangeAsync(codes.Select(_ => new AccessCodeEntity{BatchId = batch.Entity.AccessCodeBatchId, Code = _}), cancellationToken);

                await Db.SaveChangesAsync(cancellationToken);

                var batchEntity = await Db.AccessCodeBatches.AsNoTracking()
                    .Include(_ => _.AccessCodes)
                    .Include(_ => _.Courses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Subject)
                    .Include(_ => _.Courses).ThenInclude(_ => _.Course).ThenInclude(_ => _.Curriculum)
                    .Where(_ => _.AccessCodeBatchId == batch.Entity.AccessCodeBatchId)
                    .GetSingleAsync(cancellationToken);

                return new Result
                {
                    BatchHeader = _accessCodeService.GetBatchHeader(batchEntity),
                    BatchId = batchEntity.AccessCodeBatchId,
                    Codes = batchEntity.AccessCodes
                        .Select(_ => new AccessCode { AccessCodeId = _.AccessCodeId, Code = _.Code }).ToList(),
                    CreatedAtUtc = batchEntity.CreatedAt
                };
            }
        }
    }
}