using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Templates;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Templates
{
    [PublicAPI]
    public partial class SaveTemplateCommand : ICommand<SaveTemplateCommand.Result>
    {
        public Guid? TemplateId { get; set; }
        public string Template { get; init; }
        public int TemplateType { get; init; }
        public List<Guid> Courses { get; init; }


        [PublicAPI]
        public class Result
        {
        }


        internal class Validator : ValidatorBase<SaveTemplateCommand>
        {
            public Validator(IDb db) : base(db)
            {
            }
        }


        internal class PermissionCheck : PermissionCheckBase<SaveTemplateCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(SaveTemplateCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;
                return false;
            }
        }


        internal class Handler : CommandHandlerBase<SaveTemplateCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(SaveTemplateCommand request, CancellationToken cancellationToken)
            {
                Guid templateId;
                if (request.TemplateId == null)
                {
                    var result = await Db.Templates.AddAsync(new TemplateEntity
                    {
                        Type = request.TemplateType,
                        Value = request.Template
                    }, cancellationToken);
                    await Db.SaveChangesAsync(cancellationToken);
                    templateId = result.Entity.TemplateId;
                }
                else
                {
                    templateId = request.TemplateId.Value;
                    var template = await Db.Templates.AsNoTracking().Where(_ => _.TemplateId == templateId).FirstOrDefaultAsync(cancellationToken);
                    template.Value = request.Template;
                    template.Type = request.TemplateType;
                    await Db.SaveChangesAsync(cancellationToken);
                }

                await UpdateCourses(templateId, request.Courses);
                return new Result();
            }

            private async Task UpdateCourses(Guid templateId, List<Guid> courses)
            {
                var existing = await Db.CourseTemplates.Where(_ => _.TemplateId == templateId).ToListAsync();
                Db.CourseTemplates.RemoveRange(existing.Where(_ => courses.All(c => c != _.CourseId)));
                foreach (var courseId in courses.Where(_ => existing.All(c => c.CourseId != _)))
                {
                    await Db.CourseTemplates.AddAsync(new CourseTemplateEntity
                    {
                        TemplateId = templateId,
                        CourseId = courseId
                    });
                }

                await Db.SaveChangesAsync();
            }
        }
    }
}
