using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.QuizAutoMap;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Glossary;
using Biobrain.Domain.Entities.Material;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using DataAccessLayer.WebAppEntities;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Content.ImportContentFromJson
{
    [PublicAPI]
    public class ImportContentFromJsonCommand : ICommand<ImportContentFromJsonCommand.Result>
    {
	    public long MajorVersion { get; set; }
	    public long MinorVersion { get; set; }

	    public List<ContentTreeEntity> Structure { get; init; }
	    public List<ContentTreeMetaEntity> StructureMeta { get; init; }
		public List<IconEntity> Icons { get; init; }

		public List<PageEntity> Pages { get; init; }
		public List<MaterialEntity> Materials { get; init; }
		public List<PageMaterialEntity> PageMaterialLinks { get; init; }

		public List<QuizEntity> Quizzes { get; init; }
		public List<QuestionEntity> Questions { get; init; }
		public List<AnswerEntity> Answers { get; init; }
		public List<QuizQuestionEntity> QuizQuestionLinks { get; init; }

		public List<TermModel> Glossary { get; init; }


		[PublicAPI]
        public class Result
        {
	        public Guid CourseId { get; set; }
	        public int RowsAdded { get; set; }
            public int RowsUpdated { get; set; }
            public int RowsDeleted { get; set; }
            public string Log { get; set; }
            public long Major { get; set; }
            public long Minor { get; set; }
        }


		[PublicAPI]
        public class TermModel
        {
            public Guid TermId { get; set; }

            public string Ref { get; set; }

            public string Term { get; set; }

            public string Definition { get; set; }

            public string Header { get; set; }

            public Guid CourseId { get; set; }
        }


        internal class Validator : ValidatorBase<ImportContentFromJsonCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.Structure).NotNull().NotEmpty();
                RuleFor(_ => _.StructureMeta).NotNull().NotEmpty();
                //RuleFor(_ => _.Icons).NotNull().NotEmpty();
                RuleFor(_ => _.Pages).NotNull().NotEmpty();
                RuleFor(_ => _.Materials).NotNull().NotEmpty();
                RuleFor(_ => _.PageMaterialLinks).NotNull().NotEmpty();
                RuleFor(_ => _.Quizzes).NotNull().NotEmpty();
                RuleFor(_ => _.Answers).NotNull().NotEmpty();
                RuleFor(_ => _.QuizQuestionLinks).NotNull().NotEmpty();
                //RuleFor(_ => _.Glossary).NotNull().NotEmpty();
            }
        }


        internal class PermissionCheck: PermissionCheckBase<ImportContentFromJsonCommand>
        {
            //private readonly IDb db;

            public PermissionCheck(ISecurityService securityService/*, IDb db*/) : base(securityService)
            {
                //this.db = db;
            }

            protected override bool CanExecute(ImportContentFromJsonCommand request, IUserSecurityInfo user)
            {
                //ToDo Uncomment
                if (user.IsApplicationAdmin()) return true;

                return false;
            }
        }


        internal class Handler : CommandHandlerBase<ImportContentFromJsonCommand, Result>
		{
			private readonly ILogger logger;
            private readonly IQuizAutoMapService _quizAutoMapService;

            public Handler(IDb db, ILogger<ImportContentFromJsonCommand> logger, IQuizAutoMapService quizAutoMapService) : base(db)
			{
				this.logger = logger;
                this._quizAutoMapService = quizAutoMapService;
			}

            public override async Task<Result> Handle(ImportContentFromJsonCommand request, CancellationToken cancellationToken)
            {
	            var result = new Result();

	            await using var transaction = await Db.BeginTransactionAsync(cancellationToken);
	            try
	            {
		            var inputCourses = request.Structure.Select(x => x.CourseId).Distinct().ToList();
		            if (inputCourses.Count != 1) throw new ValidationException("Can't import multiple courses");
		            result.CourseId = inputCourses[0];

                    var courses = await Db.Courses.Select(_ => new {_.SubjectCode, _.CourseId}).ToListAsync(cancellationToken);
                    var courseToSubjectMap = courses.ToDictionary(_ => _.CourseId, _ => _.SubjectCode);
                    var courseToUpdate = await Db.Courses.Where(CourseSpec.ById(inputCourses[0])).SingleAsync(cancellationToken);

                    result = await ImportEntities(request.Icons, result, entity => entity.IconId, _=>false, false, cancellationToken);
		            result = await ImportEntities(request.StructureMeta, result, entity => entity.ContentTreeMetaId, _=>_.CourseId==inputCourses[0], true, cancellationToken);
		            result = await ImportEntities(request.Structure, result, entity => entity.NodeId, entity => entity.NodeId,
                        _ =>
                        {
                            var dbEntity = Db.ContentTree.AsNoTracking().FirstOrDefault(db => db.NodeId == _.NodeId);
                            if (dbEntity != null)
                                _.IsAvailableInDemo = dbEntity.IsAvailableInDemo;
                            return _;
                        }, _ => _.CourseId == inputCourses[0], true, cancellationToken);
		            result = await ImportEntities(request.Pages, result, entity => entity.PageId, _ => false, false, cancellationToken);
		            result = await ImportEntities(request.Quizzes, result, entity => entity.QuizId, _ => false, false, cancellationToken);
		            result = await ImportEntities(request.Materials, result, entity => entity.MaterialId, _ => _.CourseId == inputCourses[0], true, cancellationToken);

					//Remove and import PageMaterialLinks
					(await Db.PageMaterials.Include(x => x.Page)
							.ThenInclude(x => x.ContentTreeNode)
							.Where(x => inputCourses.Contains(x.Page.ContentTreeNode.CourseId))
							.ToListAsync(cancellationToken))
						.ForEach(x => Db.Remove(x));
					result = await ReplaceEntities(request.PageMaterialLinks, entity => new object[]{entity.PageId, entity.MaterialId}, result, cancellationToken);
		            
                    result = await ImportEntities(request.Questions, result, entity => entity.QuestionId, _ => _.CourseId == inputCourses[0], true, cancellationToken);
		            result = await ImportEntities(request.Answers, result, entity => entity.AnswerId, _ => _.CourseId == inputCourses[0], true, cancellationToken);

					//Remove and import Quiz question links
					Db.QuizQuestions.RemoveRange((await Db.QuizQuestions.Include(x => x.Quiz)
						.ThenInclude(x => x.ContentTreeNode)
						.Where(x => inputCourses.Contains(x.Quiz.ContentTreeNode.CourseId)).ToListAsync(cancellationToken)));
					result = await ReplaceEntities(request.QuizQuestionLinks, entity => new object[] { entity.QuestionId, entity.QuizId }, result, cancellationToken);

                    // Import Glossary
                    result = await ImportEntities(request.Glossary,
                                                  result,
                                                  entity => entity.TermId,
                                                  dbEntity => dbEntity.TermId,
                                                  entity => new TermEntity
                                                            {
                                                                TermId = entity.TermId,
                                                                Ref = entity.Ref,
                                                                Term = entity.Term,
                                                                Definition = entity.Definition,
                                                                Header = entity.Header,
                                                                SubjectCode = courseToSubjectMap[entity.CourseId],
                                                                DeletedAt = null,
                                                            },
                                                  dbEntity => dbEntity.SubjectCode == courseToSubjectMap[inputCourses[0]],
                                                  true,
                                                  cancellationToken);

                    courseToUpdate.LastContentUpdateUtc = DateTime.UtcNow;

                    await Db.SaveChangesAsync(cancellationToken);
		            await transaction.CommitAsync(cancellationToken);
                }
	            catch (Exception e)
	            {
		            result.Log = result.Log + $"\n\nError while importing data: {e}";
					logger.LogError(result.Log);
		            await transaction.RollbackAsync(cancellationToken);
	            }

                try
                {
                    // Apply maps
                    var importedQuizIds = request.Quizzes.Select(_ => _.QuizId);
                    var quizzesToMap = await Db.Quizzes.Where(_ => _.AutoMapQuizId != null && importedQuizIds.Contains(_.AutoMapQuizId.Value))
                        .Select(_ => _.QuizId).ToListAsync(cancellationToken);
                    foreach (var quizId in quizzesToMap)
                    {
                        await _quizAutoMapService.MapQuiz(quizId);
                    }

                    result.Log += $"Mapped {quizzesToMap.Count} quizzes";
                }
                catch(Exception e)
                {
                    logger.LogError(e, "Exception while mapping quizzes");
                    result.Log += $"Map quizzes exception {e}";
                }

	            return result;
            }

            private Task<Result> ImportEntities<TModel>(List<TModel> entities,
                                                        Result result,
                                                        Func<TModel, Guid> inputModelKeySelector,
                                                        Func<TModel, bool> dbModelWhere,
                                                        bool needDeleteOther,
                                                        CancellationToken cancellationToken)
                where TModel : class
            {
                return ImportEntities(entities,
                                      result,
                                      inputModelKeySelector,
                                      inputModelKeySelector,
                                      _ => _,
                                      dbModelWhere,
                                      needDeleteOther,
                                      cancellationToken);
            }

            private async Task<Result> ImportEntities<TModel, TDbModel>(List<TModel> entities,
                                                                        Result result,
                                                                        Func<TModel, Guid> inputModelKeySelector,
                                                                        Func<TDbModel, Guid> dbModelKeySelector,
                                                                        Func<TModel, TDbModel> entityConvertor,
                                                                        Func<TDbModel, bool> dbModelWhere,
                                                                        bool needDeleteOther,
                                                                        CancellationToken cancellationToken)
                where TModel : class
                where TDbModel : class
            {
                var updated = 0;
                var added = 0;
                var dbEntities = await Db.Set<TDbModel>().AsNoTracking().ToListAsync(cancellationToken);
                foreach (var entity in entities)
                {
                    var convertedEntity = entityConvertor(entity);
                    var dbEntity = dbEntities.FirstOrDefault(x => dbModelKeySelector(x) == inputModelKeySelector(entity));
                    if (dbEntity != null)
                    {
                        Db.Update(convertedEntity);
                        updated++;
                    }
                    else
                    {
                        await Db.AddAsync(convertedEntity, cancellationToken);
                        added++;
                    }
                }

                result.RowsAdded += added;
                result.RowsUpdated += updated;
                var message = $"{typeof(TDbModel).Name} imported. Added: {added}, updated: {updated}";
                result.Log += message + " \n";
                logger.LogInformation(message);

                // Soft delete if deletable
                if(needDeleteOther)
	                foreach (var dbModel in dbEntities.Where(dbModelWhere).Where(_ => !entities.Select(inputModelKeySelector).Contains(dbModelKeySelector(_))))
	                {
		                if(! (dbModel is IDeletedEntity)) break;
	                    ((IDeletedEntity)dbModel).DeletedAt = DateTime.UtcNow;
	                    Db.Update(dbModel);
	                }

                return result;
            }

            private async Task<Result> ReplaceEntities<T>(List<T> newEntities, Func<T, object[]> keySelector, Result result, CancellationToken cancellationToken) where T : class
			{
				var removed = 0;
				var added = 0;
				//var dbEntities = await Db.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

				//Remove old
				//dbEntities.ForEach(x =>
				//{
				//	Db.Remove(x);
				//	removed++;
				//});

				// Add new
				foreach (var entity in newEntities)
				{
					var dbEntity = await Db.FindAsync<T>(keySelector(entity), cancellationToken);
					if (dbEntity != null)
					{
						Db.Remove(dbEntity);
						removed++;
					}
					await Db.AddAsync(entity, cancellationToken);
					added++;
				}

				result.RowsAdded += added;
				result.RowsDeleted += removed;
				var message = $"{typeof(T).FullName} imported. Added: {added}, removed: {removed}";
				result.Log += message + " \n";
				logger.LogInformation(message);

				return result;
			}

        }
    }
}
