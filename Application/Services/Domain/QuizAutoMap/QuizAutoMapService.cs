using System;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Question;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services.Domain.QuizAutoMap
{
    public class QuizAutoMapService: IQuizAutoMapService
    {
        private readonly IDb _db;

        public QuizAutoMapService(IDb db) => _db = db;

        public async Task MapQuiz(Guid quizId)
        {
            var quiz = await _db.Quizzes
                .Include(_ => _.QuizQuestions)
                .Include(_ => _.QuizExcludedQuestions)
                .Include(_ => _.AutoMapQuiz).ThenInclude(_ => _.QuizQuestions)
                .Where(QuizSpec.ById(quizId))
                .SingleAsync();

            if (quiz?.AutoMapQuiz == null) return;

            var toDelete = quiz.QuizQuestions.Where(_ =>
                quiz.AutoMapQuiz.QuizQuestions.All(aq => aq.QuestionId != _.QuestionId));
            _db.QuizQuestions.RemoveRange(toDelete);

            var excludedToDelete = quiz.QuizExcludedQuestions.Where(_ =>
                quiz.AutoMapQuiz.QuizQuestions.All(aq => aq.QuestionId != _.QuestionId));
            _db.QuizExcludedQuestions.RemoveRange(excludedToDelete);

            var questionIdsToAdd = quiz.AutoMapQuiz.QuizQuestions
                .Where(_ => quiz.QuizQuestions.All(qq => qq.QuestionId != _.QuestionId) && quiz.QuizExcludedQuestions.All(qq => qq.QuestionId != _.QuestionId));

            // All new questions goes to excluded
            await _db.QuizExcludedQuestions.AddRangeAsync(questionIdsToAdd.Select(_ => new QuizExcludedQuestionEntity{QuizId = quizId, QuestionId = _.QuestionId, Order = _.Order}));
            //await _db.QuizQuestions.AddRangeAsync(questionIdsToAdd.Select(_ => new QuizQuestionEntity{QuizId = quizId, QuestionId = _.QuestionId, Order = _.Order}));
            foreach (var quizQuestion in quiz.QuizQuestions)
            {
                var baseQuizQuestion =
                    quiz.AutoMapQuiz.QuizQuestions.FirstOrDefault(_ => _.QuestionId == quizQuestion.QuestionId);
                if(baseQuizQuestion == null) continue;
                quizQuestion.Order = baseQuizQuestion.Order;
            }

            await _db.SaveChangesAsync();
        }
    }
}