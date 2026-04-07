using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.GetStudentCustomQuizzes
{
    [PublicAPI]
    public class GetStudentCustomQuizzesQuery : ICommand<GetStudentCustomQuizzesQuery.Result>
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }


        [PublicAPI]
        public class Result
        {
            public List<Item> Quizzes { get; set; } = new();
        }


        [PublicAPI]
        public class Item
        {
            public Guid QuizId { get; set; }
            public string Name { get; set; }
            public int QuestionCount { get; set; }
            public DateTime CreatedAt { get; set; }
            public List<Guid> ContentTreeNodeIds { get; set; } = new();
        }


        internal class Validator : ValidatorBase<GetStudentCustomQuizzesQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetStudentCustomQuizzesQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GetStudentCustomQuizzesQuery request, IUserSecurityInfo user)
                => user.IsAccountOwner(request.UserId);
        }


        internal class Handler : CommandHandlerBase<GetStudentCustomQuizzesQuery, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(GetStudentCustomQuizzesQuery request, CancellationToken cancellationToken)
            {
                var quizzes = await Db.Quizzes.AsNoTracking()
                    .Include(_ => _.ContentTreeNode)
                    .Where(_ => _.Type == QuizType.StudentCustom
                                && _.CreatedByUserId == request.UserId
                                && _.ContentTreeNode.CourseId == request.CourseId)
                    .OrderByDescending(_ => _.CreatedAt)
                    .Take(50)
                    .Select(_ => new Item
                    {
                        QuizId = _.QuizId,
                        Name = _.Name,
                        QuestionCount = _.QuestionCount ?? 0,
                        CreatedAt = _.CreatedAt,
                        ContentTreeNodeIds = new List<Guid> { _.ContentTreeId },
                    })
                    .ToListAsync(cancellationToken);

                return new Result { Quizzes = quizzes };
            }
        }
    }
}
