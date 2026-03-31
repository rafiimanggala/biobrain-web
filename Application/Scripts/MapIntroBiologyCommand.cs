using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Material;
using Biobrain.Domain.Entities.Question;
using Biobrain.Domain.Entities.Quiz;
using DataAccessLayer.WebAppEntities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Scripts
{
    [PublicAPI]
    public class MapCoursesCommand : ICommand<MapCoursesCommand.Result>
    {



        [PublicAPI]
        public class Result
        {

        }


        internal class Validator : ValidatorBase<MapCoursesCommand>
        {
            public Validator(IDb db) : base(db)
            {

            }
        }


        internal class PermissionCheck: PermissionCheckBase<MapCoursesCommand>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(MapCoursesCommand request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal class Handler : CommandHandlerBase<MapCoursesCommand, Result>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<Result> Handle(MapCoursesCommand request, CancellationToken cancellationToken)
            {
                var seniorId = Guid.Parse("7E35EF61-98DB-4C5A-82B7-F1FD514629D5");
                var introId = Guid.Parse("39483A86-C89C-4F8F-AECE-C9D24F10A0F1");

                var seniorMeta = await Db.ContentTreeMeta.AsNoTracking()
                    .ToListAsync(cancellationToken);       
                var introMeta = await Db.ContentTreeMeta.AsNoTracking().Where(_ => _.CourseId == introId)
                    .ToListAsync(cancellationToken);

                var seniorTree = await Db.ContentTree.AsNoTracking().Where(_ => _.CourseId == seniorId && _.DeletedAt == null).Include(_ => _.ContentTreeMeta).OrderBy(_ => _.ContentTreeMeta.Depth)
                    .ToListAsync(cancellationToken);

                var contentTreeMap = new Dictionary<Guid, Guid>();

                foreach (var ste in seniorTree)
                {
                    var id = Guid.NewGuid();
                    contentTreeMap.Add(ste.NodeId, id);
                    await Db.ContentTree.AddAsync(new ContentTreeEntity
                    {
                        NodeId = id,
                        CourseId = introId,
                        ContentTreeMetaId = GetMetaId(ste.ContentTreeMetaId, seniorMeta, introMeta),
                        IconId = ste.IconId,
                        IsAvailableInDemo = ste.IsAvailableInDemo,
                        Name = ste.Name,
                        Order = ste.Order,
                        ParentId = ste.ParentId == null ? null : contentTreeMap[ste.ParentId.Value],
                    }, cancellationToken);
                }

                var seniorQuizzes = await Db.Quizzes.AsNoTracking()
                    .Include(_ => _.QuizQuestions)
                    .Include(_ => _.ContentTreeNode)
                    .Where(_ => _.ContentTreeNode.CourseId == seniorId && _.ContentTreeNode.DeletedAt == null)
                    .ToListAsync(cancellationToken);
                foreach (var seniorQuiz in seniorQuizzes)
                {
                    var id = Guid.NewGuid();
                    await Db.Quizzes.AddAsync(new QuizEntity
                    {
                        QuizId = id,
                        AutoMapQuizId = seniorQuiz.QuizId,
                        ContentTreeId = contentTreeMap[seniorQuiz.ContentTreeId],
                    }, cancellationToken);
                    foreach (var seniorQuizQuestion in seniorQuiz.QuizQuestions)
                    {
                        await Db.QuizQuestions.AddAsync(new QuizQuestionEntity
                        {
                            Order = seniorQuizQuestion.Order,
                            QuestionId = seniorQuizQuestion.QuestionId,
                            QuizId = id,
                        }, cancellationToken);
                    }
                }

                var seniorPages = await Db.Pages.AsNoTracking()
                    .Include(_ => _.ContentTreeNode)
                    .Include(_ => _.PageMaterials)
                    .Where(_ => _.ContentTreeNode.CourseId == seniorId && _.ContentTreeNode.DeletedAt == null)
                    .ToListAsync(cancellationToken);

                foreach (var seniorPage in seniorPages)
                {
                    var id = Guid.NewGuid();
                    await Db.Pages.AddAsync(new PageEntity
                    {
                        PageId = id,
                        ContentTreeId = contentTreeMap[seniorPage.ContentTreeId],
                    }, cancellationToken);
                    foreach (var seniorPageMaterial in seniorPage.PageMaterials)
                    {
                        await Db.PageMaterials.AddAsync(new PageMaterialEntity
                        {
                            Order = seniorPageMaterial.Order,
                            MaterialId = seniorPageMaterial.MaterialId,
                            PageId = id,
                        }, cancellationToken);
                    }
                }

                await Db.SaveChangesAsync(cancellationToken);

                return new Result {};
            }

            public Guid GetMetaId(Guid seniorMetaId, List<ContentTreeMetaEntity> seniorMeta, List<ContentTreeMetaEntity> introMeta)
            {
                var seniorMetaEnt = seniorMeta.First(_ => _.ContentTreeMetaId == seniorMetaId);
                var introMetaEnt = introMeta.First(_ => _.Depth == seniorMetaEnt.Depth);
                return introMetaEnt.ContentTreeMetaId;
            }
        }
    }
}