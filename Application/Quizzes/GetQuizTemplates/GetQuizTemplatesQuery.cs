using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GetQuizTemplates
{
    [PublicAPI]
    public class GetQuizTemplatesQuery : ICommand<GetQuizTemplatesQuery.Result>
    {
        public Guid TeacherId { get; set; }
        public Guid? CourseId { get; set; }


        [PublicAPI]
        public class Result
        {
            public List<TemplateItem> Templates { get; set; } = new();
        }

        [PublicAPI]
        public class TemplateItem
        {
            public Guid TemplateId { get; set; }
            public string Name { get; set; }
            public int QuestionCount { get; set; }
            public Guid CourseId { get; set; }
            public bool HintsEnabled { get; set; }
            public bool SoundEnabled { get; set; }
            public DateTime CreatedAt { get; set; }
        }


        internal class PermissionCheck : PermissionCheckBase<GetQuizTemplatesQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GetQuizTemplatesQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                return user.IsTeacherAccountOwner(request.TeacherId);
            }
        }


        internal class Handler : CommandHandlerBase<GetQuizTemplatesQuery, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetQuizTemplatesQuery request, CancellationToken cancellationToken)
            {
                var query = Db.QuizTemplates
                    .Where(t => t.CreatedByTeacherId == request.TeacherId);

                if (request.CourseId.HasValue)
                    query = query.Where(t => t.CourseId == request.CourseId.Value);

                var templates = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new TemplateItem
                    {
                        TemplateId = t.TemplateId,
                        Name = t.Name,
                        QuestionCount = t.QuestionCount,
                        CourseId = t.CourseId,
                        HintsEnabled = t.HintsEnabled,
                        SoundEnabled = t.SoundEnabled,
                        CreatedAt = t.CreatedAt,
                    })
                    .ToListAsync(cancellationToken);

                return new Result { Templates = templates };
            }
        }
    }
}
