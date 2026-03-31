using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.Perform.UnassignQuizzesToClass
{
    [PublicAPI]
    public sealed class UnassignQuizToClassCommand : ICommand<UnassignQuizToClassCommand.Result>
    {
            public Guid QuizAssignmentId { get; init; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<UnassignQuizToClassCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizAssignmentId).ExistsInTable(Db.QuizAssignments);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<UnassignQuizToClassCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db) 
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(UnassignQuizToClassCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;
                
                var assignment = _db.QuizAssignments.Include(_ => _.SchoolClass).Where(QuizAssignmentSpec.ById(request.QuizAssignmentId)).GetSingle();

                return user.IsSchoolTeacher(assignment.SchoolClass.SchoolId) || user.IsSchoolAdmin(assignment.SchoolClass.SchoolId);
            }
        }


        internal sealed class Handler : CommandHandlerBase<UnassignQuizToClassCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UnassignQuizToClassCommand request, CancellationToken cancellationToken)
            {
                var assignment = Db.QuizAssignments.Include(_ => _.SchoolClass).Where(QuizAssignmentSpec.ById(request.QuizAssignmentId)).GetSingle();
                Db.QuizAssignments.Remove(assignment);
                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }

        }
    }
}
