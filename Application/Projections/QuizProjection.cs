using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Projections
{
    public static class QuizProjection
    {
        public static Expression<Func<QuizEntity, ContentData.Quiz>> ToQuizContentData()
            => _ => new ContentData.Quiz
                    {
                        QuizId = _.QuizId,
                        CourseId = _.ContentTreeNode.CourseId,
                        ContentTreeNodeId = _.ContentTreeNode.NodeId,
                        Name = _.Name,
                        QuestionCount = _.QuestionCount,
                        Questions = _.QuizQuestions
                                     .Select(x => new ContentData.Question
                                                  {
                                                      QuestionId = x.QuestionId,
                                                      Header = x.Question.Header,
                                                      Text = x.Question.Text,
                                                      FeedBack = x.Question.FeedBack,
                                                      Hint = x.Question.Hint,
                                                      Order = x.Order,
                                                      QuestionTypeCode = x.Question.QuestionType.QuestionTypeCode,
                                                      QuestionTypeName = x.Question.QuestionType.Name,
                                                      Answers = x.Question
                                                                 .Answers
                                                                 .Select(a => new ContentData.Answer
                                                                              {
                                                                                  AnswerId = a.AnswerId,
                                                                                  AnswerOrder = a.AnswerOrder,
                                                                                  CaseSensitive = a.CaseSensitive,
                                                                                  IsCorrect = a.IsCorrect,
                                                                                  Response = a.Response,
                                                                                  Score = a.Score,
                                                                                  Text = a.Text
                                                                              })
                                                                 .ToList()
                                                                 .ToImmutableList()
                                                  })
                                     .ToList()
                                     .ToImmutableList()
                    };

        public static IQueryable<QuizEntity> PrepareToMapToQuizContentData(this IQueryable<QuizEntity> query)
            => query.Include(_ => _.ContentTreeNode)
                    .Include(_ => _.QuizQuestions)
                    .ThenInclude(_ => _.Question)
                    .ThenInclude(_ => _.Answers)
                    .Include(_ => _.QuizQuestions)
                    .ThenInclude(_ => _.Question)
                    .ThenInclude(_ => _.QuestionType);
    }
}
