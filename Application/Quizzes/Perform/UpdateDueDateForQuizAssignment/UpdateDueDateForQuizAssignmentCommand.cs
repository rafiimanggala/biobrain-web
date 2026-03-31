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

namespace Biobrain.Application.Quizzes.Perform.UpdateDueDateForQuizAssignment
{
    [PublicAPI]
    public sealed class UpdateDueDateForQuizAssignmentCommand : ICommand<UpdateDueDateForQuizAssignmentCommand.Result>
    {
            public Guid QuizAssignmentId { get; set; }
            public DateTime DueDateUtc { get; init; }
            public DateTime DueDateLocal { get; init; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class Validator : ValidatorBase<UpdateDueDateForQuizAssignmentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.QuizAssignmentId).ExistsInTable(Db.QuizAssignments);
            }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<UpdateDueDateForQuizAssignmentCommand>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService,
                                   IDb db) 
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(UpdateDueDateForQuizAssignmentCommand request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;
                
                var assignment = _db.QuizAssignments.Include(_ => _.SchoolClass).Where(QuizAssignmentSpec.ById(request.QuizAssignmentId)).GetSingle();

                return user.IsSchoolTeacher(assignment.SchoolClass.SchoolId) || user.IsSchoolAdmin(assignment.SchoolClass.SchoolId);
            }
        }


        internal sealed class Handler : CommandHandlerBase<UpdateDueDateForQuizAssignmentCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(UpdateDueDateForQuizAssignmentCommand request, CancellationToken cancellationToken)
            {
                var assignment = Db.QuizAssignments.Include(_ => _.QuizStudentAssignments).Where(QuizAssignmentSpec.ById(request.QuizAssignmentId)).GetSingle();
                assignment.DueAtUtc = request.DueDateUtc;
                assignment.DueAtLocal = request.DueDateLocal;
                foreach (var studentAssignment in assignment.QuizStudentAssignments)
                {
                    studentAssignment.DueAtUtc = request.DueDateUtc;
                    studentAssignment.DueAtLocal = request.DueDateLocal;
                }
                await Db.SaveChangesAsync(cancellationToken);
                return new Result();
            }

        }
    }
}
