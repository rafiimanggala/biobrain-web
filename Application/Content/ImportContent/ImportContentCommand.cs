using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Material;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using DataAccessLayer.WebAppEntities;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Biobrain.Application.Content.ImportContent
{
    [PublicAPI]
    public class ImportContentCommand : ICommand<ImportContentCommand.Result>
    {
        public Guid CourseId { get; set; }
        public string JsonContent { get; set; }

        [PublicAPI]
        public class Result
        {
            public int TopicsCreated { get; set; }
            public int SubtopicsCreated { get; set; }
            public int MaterialsCreated { get; set; }
            public int QuestionsCreated { get; set; }
            public string Log { get; set; } = "";
        }

        public class TopicModel
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("subtopics")]
            public List<SubtopicModel> Subtopics { get; set; } = new();
        }

        public class SubtopicModel
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("materials")]
            public List<MaterialModel> Materials { get; set; } = new();

            [JsonProperty("questions")]
            public List<QuestionModel> Questions { get; set; } = new();
        }

        public class MaterialModel
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }
        }

        public class QuestionModel
        {
            [JsonProperty("header")]
            public string Header { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("hint")]
            public string Hint { get; set; }

            [JsonProperty("feedback")]
            public string Feedback { get; set; }

            [JsonProperty("answers")]
            public List<AnswerModel> Answers { get; set; } = new();
        }

        public class AnswerModel
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("isCorrect")]
            public bool IsCorrect { get; set; }
        }

        public class ImportPayload
        {
            [JsonProperty("topics")]
            public List<TopicModel> Topics { get; set; } = new();
        }

        internal class Validator : ValidatorBase<ImportContentCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.CourseId).ExistsInTable(Db.Courses);
                RuleFor(_ => _.JsonContent).NotEmpty();
            }
        }

        internal class PermissionCheck : PermissionCheckBase<ImportContentCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(ImportContentCommand request, IUserSecurityInfo user)
            {
                return user.IsApplicationAdmin();
            }
        }

        internal class Handler : CommandHandlerBase<ImportContentCommand, Result>
        {
            private readonly ILogger _logger;

            public Handler(IDb db, ILogger<ImportContentCommand> logger) : base(db)
            {
                _logger = logger;
            }

            public override async Task<Result> Handle(ImportContentCommand request, CancellationToken cancellationToken)
            {
                var result = new Result();

                ImportPayload payload;
                try
                {
                    payload = JsonConvert.DeserializeObject<ImportPayload>(request.JsonContent);
                }
                catch (JsonException ex)
                {
                    result.Log = $"JSON parse error: {ex.Message}";
                    return result;
                }

                if (payload?.Topics == null || payload.Topics.Count == 0)
                {
                    result.Log = "No topics found in JSON.";
                    return result;
                }

                await using var transaction = await Db.BeginTransactionAsync(cancellationToken);
                try
                {
                    var questionTypes = await Db.QuestionTypes
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    var topicMeta = await Db.ContentTreeMeta
                        .AsNoTracking()
                        .Where(m => m.CourseId == request.CourseId && m.Depth == 0)
                        .FirstOrDefaultAsync(cancellationToken);

                    var subtopicMeta = await Db.ContentTreeMeta
                        .AsNoTracking()
                        .Where(m => m.CourseId == request.CourseId && m.Depth == 1)
                        .FirstOrDefaultAsync(cancellationToken);

                    var leafMeta = await Db.ContentTreeMeta
                        .AsNoTracking()
                        .Where(m => m.CourseId == request.CourseId && m.CouldAddContent)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (topicMeta == null || subtopicMeta == null || leafMeta == null)
                    {
                        result.Log = "Content tree meta not found for this course. Ensure the course has a valid content tree structure.";
                        return result;
                    }

                    // Find the root node of the course (node with no parent)
                    var existingTopics = await Db.ContentTree
                        .AsNoTracking()
                        .Where(n => n.CourseId == request.CourseId && n.ParentId == null && n.DeletedAt == null)
                        .ToListAsync(cancellationToken);

                    var maxTopicOrder = existingTopics.Any()
                        ? existingTopics.Max(t => t.Order)
                        : -1;

                    var topicOrder = maxTopicOrder + 1;

                    foreach (var topic in payload.Topics)
                    {
                        // Create topic node
                        var topicNode = new ContentTreeEntity
                        {
                            CourseId = request.CourseId,
                            ContentTreeMetaId = topicMeta.ContentTreeMetaId,
                            Name = topic.Name,
                            Order = topicOrder++,
                            ParentId = null
                        };
                        var topicEntry = await Db.AddAsync(topicNode, cancellationToken);
                        var topicNodeId = ((ContentTreeEntity)topicEntry.Entity).NodeId;
                        result.TopicsCreated++;

                        var subtopicOrder = 0;
                        foreach (var subtopic in topic.Subtopics ?? new List<SubtopicModel>())
                        {
                            // Create subtopic node
                            var subtopicNode = new ContentTreeEntity
                            {
                                CourseId = request.CourseId,
                                ContentTreeMetaId = subtopicMeta.ContentTreeMetaId,
                                Name = subtopic.Name,
                                Order = subtopicOrder++,
                                ParentId = topicNodeId
                            };
                            var subtopicEntry = await Db.AddAsync(subtopicNode, cancellationToken);
                            var subtopicNodeId = ((ContentTreeEntity)subtopicEntry.Entity).NodeId;
                            result.SubtopicsCreated++;

                            // Create materials under a leaf node
                            if (subtopic.Materials != null && subtopic.Materials.Count > 0)
                            {
                                var materialLeafNode = new ContentTreeEntity
                                {
                                    CourseId = request.CourseId,
                                    ContentTreeMetaId = leafMeta.ContentTreeMetaId,
                                    Name = "Materials",
                                    Order = 0,
                                    ParentId = subtopicNodeId
                                };
                                var materialLeafEntry = await Db.AddAsync(materialLeafNode, cancellationToken);
                                var materialLeafNodeId = ((ContentTreeEntity)materialLeafEntry.Entity).NodeId;

                                // Create page for the leaf node
                                var page = new PageEntity
                                {
                                    ContentTreeId = materialLeafNodeId
                                };
                                var pageEntry = await Db.AddAsync(page, cancellationToken);
                                var pageId = ((PageEntity)pageEntry.Entity).PageId;

                                var materialOrder = 0;
                                foreach (var material in subtopic.Materials)
                                {
                                    var materialEntity = new MaterialEntity
                                    {
                                        CourseId = request.CourseId,
                                        Header = material.Name,
                                        Text = material.Content
                                    };
                                    var matEntry = await Db.AddAsync(materialEntity, cancellationToken);
                                    var materialId = ((MaterialEntity)matEntry.Entity).MaterialId;

                                    await Db.AddAsync(new PageMaterialEntity
                                    {
                                        PageId = pageId,
                                        MaterialId = materialId,
                                        Order = materialOrder++
                                    }, cancellationToken);

                                    result.MaterialsCreated++;
                                }
                            }

                            // Create questions under a quiz leaf node
                            if (subtopic.Questions != null && subtopic.Questions.Count > 0)
                            {
                                var quizLeafNode = new ContentTreeEntity
                                {
                                    CourseId = request.CourseId,
                                    ContentTreeMetaId = leafMeta.ContentTreeMetaId,
                                    Name = "Quiz",
                                    Order = 1,
                                    ParentId = subtopicNodeId
                                };
                                var quizLeafEntry = await Db.AddAsync(quizLeafNode, cancellationToken);
                                var quizLeafNodeId = ((ContentTreeEntity)quizLeafEntry.Entity).NodeId;

                                var quizEntity = new QuizEntity
                                {
                                    ContentTreeId = quizLeafNodeId,
                                    Name = $"{subtopic.Name} Quiz",
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                };
                                var quizEntry = await Db.AddAsync(quizEntity, cancellationToken);
                                var quizId = ((QuizEntity)quizEntry.Entity).QuizId;

                                var questionOrder = 0;
                                foreach (var question in subtopic.Questions)
                                {
                                    var questionTypeCode = ResolveQuestionTypeCode(question.Type, questionTypes);

                                    var questionEntity = new QuestionEntity
                                    {
                                        CourseId = request.CourseId,
                                        QuestionTypeCode = questionTypeCode,
                                        Header = question.Header ?? "",
                                        Text = question.Text ?? "",
                                        Hint = question.Hint ?? "",
                                        FeedBack = question.Feedback ?? ""
                                    };
                                    var qEntry = await Db.AddAsync(questionEntity, cancellationToken);
                                    var questionId = ((QuestionEntity)qEntry.Entity).QuestionId;

                                    // Create answers
                                    var answerOrder = 0;
                                    foreach (var answer in question.Answers ?? new List<AnswerModel>())
                                    {
                                        await Db.AddAsync(new AnswerEntity
                                        {
                                            QuestionId = questionId,
                                            CourseId = request.CourseId,
                                            Text = answer.Text ?? "",
                                            IsCorrect = answer.IsCorrect,
                                            AnswerOrder = answerOrder++,
                                            Score = answer.IsCorrect ? 1 : 0
                                        }, cancellationToken);
                                    }

                                    // Link question to quiz
                                    await Db.AddAsync(new QuizQuestionEntity
                                    {
                                        QuizId = quizId,
                                        QuestionId = questionId,
                                        Order = questionOrder++
                                    }, cancellationToken);

                                    result.QuestionsCreated++;
                                }
                            }
                        }
                    }

                    await Db.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    result.Log = $"Import completed. Topics: {result.TopicsCreated}, Subtopics: {result.SubtopicsCreated}, Materials: {result.MaterialsCreated}, Questions: {result.QuestionsCreated}";
                    _logger.LogInformation(result.Log);
                }
                catch (Exception ex)
                {
                    result.Log = $"Import failed: {ex.Message}";
                    _logger.LogError(ex, "Error during content import");
                    await transaction.RollbackAsync(cancellationToken);
                }

                return result;
            }

            private static long ResolveQuestionTypeCode(string typeName, List<QuestionTypeEntity> questionTypes)
            {
                if (string.IsNullOrWhiteSpace(typeName))
                    return 0; // Default to MultipleChoice

                var match = questionTypes.FirstOrDefault(qt =>
                    qt.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                    return match.QuestionTypeCode;

                // Fallback mapping
                return typeName.ToLower() switch
                {
                    "multiplechoice" or "multiple_choice" => 0,
                    "truefalse" or "true_false" => 4,
                    "shortanswer" or "short_answer" or "freetext" => 1,
                    _ => 0
                };
            }
        }
    }
}
