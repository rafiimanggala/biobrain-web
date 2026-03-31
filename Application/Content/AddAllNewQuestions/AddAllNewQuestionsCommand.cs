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
using Biobrain.Domain.Entities.Question;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Content.AddAllNewQuestions
{
    [PublicAPI]
    public class AddAllNewQuestionsCommand : ICommand<List<AddAllNewQuestionsCommand.Result>>
    {
	    public Guid CourseId { get; set; }

	    [PublicAPI]
	    public class Result
	    {
		    public string NodeName { get; set; }
			public int NumberOfQuestions { get; set; }
	    }


		internal class Validator : ValidatorBase<AddAllNewQuestionsCommand>
        {
            public Validator(IDb db) : base(db)
            {
				RuleFor(_ => _.CourseId).ExistsInTable(db.Courses).Must(_ => db.Courses.Where(x => x.CourseId == _).Include(x => x.Curriculum).All(x => !x.Curriculum.IsGeneric)).WithMessage("Can't do this for Generic course");
			}
        }


        internal class PermissionCheck: PermissionCheckBase<AddAllNewQuestionsCommand>
        {
            private readonly IDb db;

            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => this.db = db;

            protected override bool CanExecute(AddAllNewQuestionsCommand request, IUserSecurityInfo user) =>
                //ToDo Uncomment
                //if (user.IsApplicationAdmin()) return true;
                //return false;
                true;
        }


        internal class Handler : CommandHandlerBase<AddAllNewQuestionsCommand, List<Result>>
        {
	        private readonly ILogger _logger;


			public Handler(IDb db, ILogger<AddAllNewQuestionsCommand> logger)
				: base(db)
                => _logger = logger;

            public override async Task<List<Result>> Handle(AddAllNewQuestionsCommand request,
                                                            CancellationToken cancellationToken)
            {
	            var result = new List<Result>();

				var destCourse = await Db.Courses.Where(_ => _.CourseId == request.CourseId)
					.Include(x => x.Subject)
					.Include(x => x.Curriculum)
					.FirstOrDefaultAsync(cancellationToken);
				var sourceCourse = await Db.Courses
					.Where(_ => _.SubjectCode == destCourse.SubjectCode && _.CurriculumCode == Constant.Curriculum.Ib)
					.FirstOrDefaultAsync(cancellationToken);

				var quizzes = await Db.Quizzes
					.Include(_ => _.ContentTreeNode)
					.ThenInclude(x => x.ParentContentTree)
					.Include(x => x.QuizQuestions)
					.ThenInclude(x => x.Question)
					.ThenInclude(x => x.QuizQuestions)
					.ThenInclude(x => x.Quiz)
					.ThenInclude(x => x.ContentTreeNode)
					.Include(x => x.QuizQuestions)
					.ThenInclude(x => x.Question)
					.ThenInclude(x => x.QuizQuestions)
					.ThenInclude(x => x.Quiz)
					.ThenInclude(x => x.QuizQuestions)
					.Where(_ => _.ContentTreeNode.CourseId == destCourse.CourseId)
					.ToListAsync(cancellationToken);

				foreach (var quiz in quizzes)
				{
					var resultEntry = new Result{NumberOfQuestions = 0, NodeName = $"{quiz.ContentTreeNode.ParentContentTree.Name} - {quiz.ContentTreeNode.Name}"};
					result.Add(resultEntry);
					if(!quiz.QuizQuestions.Any()) continue;
					var sourceCourseQuiz = quiz.QuizQuestions.First().Question.QuizQuestions.FirstOrDefault(x => x.Quiz.ContentTreeNode.CourseId == sourceCourse.CourseId)?.Quiz;
					if(sourceCourseQuiz == null) continue;
					foreach (var quizQuestion in sourceCourseQuiz.QuizQuestions)
					{
						// Add new question if not exist
						if (quiz.QuizQuestions.All(x => x.QuestionId != quizQuestion.QuestionId))
						{
							resultEntry.NumberOfQuestions++;
							Db.QuizQuestions.Add(new QuizQuestionEntity{QuizId = quiz.QuizId, QuestionId = quizQuestion.QuestionId, Order = quiz.QuizQuestions.Max(x => x.Order)+1});
						}

						//ToDo Delete old questions?
					}
				}

				await Db.SaveChangesAsync(cancellationToken);

				_logger.LogInformation($"Course {destCourse.Curriculum.Name} - {destCourse.Subject.Name} ({destCourse.CourseId}) was updated with questions:\n{string.Join('\n', result.Select(x => $"{x.NodeName} - added {x.NumberOfQuestions} questions"))}");
				return result;
			}

		}
    }
}
