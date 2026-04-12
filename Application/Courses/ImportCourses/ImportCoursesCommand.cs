using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Course;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Courses.ImportCourses
{
    [PublicAPI]
    public class ImportCoursesCommand : ICommand<ImportCoursesCommand.Result>
    {
        public List<CourseImportItem> Courses { get; set; } = new();

        [PublicAPI]
        public class CourseImportItem
        {
            public int SubjectCode { get; set; }
            public int CurriculumCode { get; set; }
            public int Year { get; set; }
            public string SubHeader { get; set; }
            public string Postfix { get; set; }
            public bool IsForSell { get; set; }
            public bool IsBase { get; set; }
            public CourseGroup CourseGroup { get; set; }
        }

        [PublicAPI]
        public record Result(int SuccessCount, int ErrorCount, List<string> Errors);


        internal class Validator : ValidatorBase<ImportCoursesCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Courses).NotEmpty();

                RuleForEach(_ => _.Courses).ChildRules(c =>
                {
                    c.RuleFor(_ => _.SubjectCode).GreaterThan(0);
                    c.RuleFor(_ => _.CurriculumCode).GreaterThan(0);
                    c.RuleFor(_ => _.Year).GreaterThan(0);
                });
            }
        }


        internal class PermissionCheck : PermissionCheckBase<ImportCoursesCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(ImportCoursesCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<ImportCoursesCommand, Result>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(IDb db, ILogger<Handler> logger) : base(db)
            {
                _logger = logger;
            }

            public override async Task<Result> Handle(ImportCoursesCommand request, CancellationToken cancellationToken)
            {
                var successCount = 0;
                var errorCount = 0;
                var errors = new List<string>();

                for (var i = 0; i < request.Courses.Count; i++)
                {
                    var item = request.Courses[i];
                    try
                    {
                        var existing = await Db.Courses.SingleOrDefaultAsync(
                            c => c.SubjectCode == item.SubjectCode
                                 && c.CurriculumCode == item.CurriculumCode
                                 && c.Year == item.Year,
                            cancellationToken);

                        if (existing != null)
                        {
                            existing.SubHeader = item.SubHeader;
                            existing.Postfix = item.Postfix;
                            existing.IsForSell = item.IsForSell;
                            existing.IsBase = item.IsBase;
                            existing.Group = item.CourseGroup;
                            existing.LastContentUpdateUtc = DateTime.UtcNow;
                        }
                        else
                        {
                            var course = new CourseEntity
                            {
                                CourseId = Guid.NewGuid(),
                                SubjectCode = item.SubjectCode,
                                CurriculumCode = item.CurriculumCode,
                                Year = item.Year,
                                SubHeader = item.SubHeader,
                                Postfix = item.Postfix,
                                IsForSell = item.IsForSell,
                                IsBase = item.IsBase,
                                Group = item.CourseGroup,
                                LastContentUpdateUtc = DateTime.UtcNow
                            };
                            await Db.Courses.AddAsync(course, cancellationToken);
                        }

                        await Db.SaveChangesAsync(cancellationToken);
                        successCount++;
                    }
                    catch (Exception e)
                    {
                        errorCount++;
                        errors.Add($"Row {i + 1}: {e.Message}");
                        _logger.LogError(e, "Failed to import course row {RowIndex}", i + 1);
                    }
                }

                return new Result(successCount, errorCount, errors);
            }
        }
    }
}
