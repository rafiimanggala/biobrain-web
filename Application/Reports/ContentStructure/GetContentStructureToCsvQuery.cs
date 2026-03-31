using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Services.Domain.ContentTreeService;
using Biobrain.Domain.Constants;
using BiobrainWebAPI.Values;
using Csv;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Biobrain.Application.Reports.ContentStructure
{
    [PublicAPI]
    public sealed class GetContentStructureToCsvQuery : IQuery<GetContentStructureToCsvQuery.Result>
    {
        public Guid CourseId { get; set; }

        [PublicAPI]
        public record Result
        {
            public string FileUrl { get; set; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetContentStructureToCsvQuery>
        {

            public PermissionCheck(ISecurityService securityService)
                : base(securityService)
            {
            }

            protected override bool CanExecute(GetContentStructureToCsvQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }

        internal sealed class Handler : QueryHandlerBase<GetContentStructureToCsvQuery, Result>
        {
            private readonly IConfiguration _configuration;
            private readonly IContentTreeService _contentTreeService;
            private readonly IContentTreePathResolver _contentTreePathResolver;

            public Handler(IDb db, IConfiguration configuration, IContentTreeService contentTreeService, IContentTreePathResolver contentTreePathResolver) : base(db)
            {
                _configuration = configuration;
                _contentTreeService = contentTreeService;
                _contentTreePathResolver = contentTreePathResolver;
            }

            public override async Task<Result> Handle(GetContentStructureToCsvQuery request, CancellationToken cancellationToken)
            {

                var header = await GetHeader(request, cancellationToken);
                var rows = await GetRows(request, header.Length, cancellationToken);

                var fileName = Guid.NewGuid() + ".csv";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink, fileName);



                var csv = CsvWriter.WriteToText(header, rows);
                await File.WriteAllTextAsync(filePath, csv, cancellationToken);

                return new Result
                { FileUrl = new Uri($"/{AppSettings.ReportFolderLink}/{fileName}", UriKind.Relative).ToString() };
            }

            private async Task<string[]> GetHeader(GetContentStructureToCsvQuery request, CancellationToken ct)
            {
                var structure = await Db.ContentTreeMeta.Where(_ => _.CourseId == request.CourseId).ToListAsync(ct);

                var headerRow = structure.OrderBy(_ => _.Depth).Select(_ => _.Name).ToList();
                headerRow.Add("Header");

                return headerRow.ToArray();
            }

            private async Task<List<string[]>> GetRows(GetContentStructureToCsvQuery request, int columnsCount, CancellationToken cancellationToken)
            {
                var content = await Db.PageMaterials.AsNoTracking()
                    .Include(_ => _.Page).ThenInclude(_ => _.ContentTreeNode)
                    .Include(_ => _.Material)
                    .Where(_ => _.Page.ContentTreeNode.CourseId == request.CourseId)
                    .ToListAsync(cancellationToken);

                var courseStructure = await _contentTreeService.GetCourseStructure(request.CourseId, cancellationToken);

                var courseTemplate = await Db.CourseTemplates.AsNoTracking()
                    .Include(_ => _.Template)
                    .Where(_ => _.CourseId == request.CourseId &&
                                _.Template.Type == Constant.TemplateType.QuizResultsQuizHeader)
                    .SingleAsync(cancellationToken);

                var data = new List<KeyValuePair<long, string[]>>();
                content.ForEach(_ =>
                {
                    var fullPath = _contentTreePathResolver.ResolveFullPath(_.Page.ContentTreeId,
                        courseStructure);
                    data.Add(new KeyValuePair<long, string[]>(GetSummaryOrder(fullPath)*1000 + _.Order,
                        courseTemplate == null
                                ? GetEmptyRow(columnsCount)
                                : fullPath.Select(_ => _.Value).Append(_.Material.Header).ToArray()
                     ));
                });

                var rows = new List<string[]>();

                foreach(var p in data.OrderBy(_ => _.Key))
                {
                    rows.Add(p.Value);
                }

                return rows;
            }

            private string[] GetEmptyRow(int columnsCount)
            {
                var result = new List<string>();
                for (var i = 0; i < columnsCount; i++)
                {
                    result.Add(string.Empty);
                }

                return result.ToArray();
            }

            private long GetSummaryOrder(List<PathItem> path)
            {
                var result = 0L;
                foreach (var item in path)
                {
                    result = result * 1000 + item.OrderInTree + 1;
                }

                return result;
            }

        }
    }
}
