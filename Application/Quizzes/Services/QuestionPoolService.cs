using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Quizzes.Services
{
    public interface IQuestionPoolService
    {
        Task<List<QuestionEntity>> GetPooledQuestionsAsync(
            Guid courseId,
            IEnumerable<Guid> contentTreeNodeIds,
            int requestedCount,
            Guid? schoolClassId = null);
    }

    public class QuestionPoolService : IQuestionPoolService
    {
        private readonly IDb _db;

        public QuestionPoolService(IDb db)
        {
            _db = db;
        }

        public async Task<List<QuestionEntity>> GetPooledQuestionsAsync(
            Guid courseId,
            IEnumerable<Guid> contentTreeNodeIds,
            int requestedCount,
            Guid? schoolClassId = null)
        {
            var nodeIds = contentTreeNodeIds.ToList();

            // Get all quiz IDs under the given content tree nodes
            var quizIds = await _db.Quizzes
                .Where(q => nodeIds.Contains(q.ContentTreeId))
                .Select(q => q.QuizId)
                .ToListAsync();

            if (!quizIds.Any())
            {
                // Also check child nodes (for topic-level, get subtopic quizzes)
                var childNodeIds = await GetDescendantNodeIdsAsync(nodeIds);
                quizIds = await _db.Quizzes
                    .Where(q => childNodeIds.Contains(q.ContentTreeId))
                    .Select(q => q.QuizId)
                    .ToListAsync();
            }

            // Get all question IDs from those quizzes
            var questionIds = await _db.QuizQuestions
                .Where(qq => quizIds.Contains(qq.QuizId))
                .Select(qq => qq.QuestionId)
                .Distinct()
                .ToListAsync();

            // Exclude class-specific excluded questions
            if (schoolClassId.HasValue)
            {
                var excludedIds = await _db.ExcludedQuestions
                    .Where(eq => eq.SchoolClassId == schoolClassId.Value
                                 && quizIds.Contains(eq.QuizId))
                    .Select(eq => eq.QuestionId)
                    .Distinct()
                    .ToListAsync();

                questionIds = questionIds
                    .Where(qid => !excludedIds.Contains(qid))
                    .ToList();
            }

            // Load question entities (exclude soft-deleted)
            var questions = await _db.Questions
                .Where(q => questionIds.Contains(q.QuestionId) && q.DeletedAt == null)
                .Include(q => q.Answers)
                .ToListAsync();

            // Shuffle and take requested count
            var random = new Random();
            var shuffled = questions.OrderBy(_ => random.Next()).ToList();

            return shuffled.Take(Math.Min(requestedCount, shuffled.Count)).ToList();
        }

        private async Task<List<Guid>> GetDescendantNodeIdsAsync(List<Guid> parentNodeIds)
        {
            var result = new List<Guid>();
            var currentLevel = parentNodeIds;

            while (currentLevel.Any())
            {
                var children = await _db.ContentTree
                    .Where(ct => ct.ParentId.HasValue
                                 && currentLevel.Contains(ct.ParentId.Value)
                                 && ct.DeletedAt == null)
                    .Select(ct => ct.NodeId)
                    .ToListAsync();

                result.AddRange(children);
                currentLevel = children;
            }

            return result;
        }
    }
}
