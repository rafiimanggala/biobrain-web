using System.Collections.Immutable;
using Biobrain.Domain.Entities.Content;
using DataAccessLayer.WebAppEntities;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static class ContentDataExtension
    {
        public static ContentData.Icon ToContentData(this IconEntity entity)
        {
            if (entity is null)
                return null;

            return new ContentData.Icon
                   {
                       Name = entity.Name,
                       FileName = entity.FileName,
                       Reference = entity.Reference
                   };
        }

        public static ContentData.ContentTree ToContentData(this ContentTreeEntity entity, ImmutableList<ContentData.ContentTree> nodes) => new()
                                                                                                                                            {
                                                                                                                                                NodeId = entity.NodeId,
                                                                                                                                                ParentId = entity.ParentId,
                                                                                                                                                CourseId = entity.CourseId,
                                                                                                                                                Name = entity.Name,
                                                                                                                                                Order = entity.Order,
                                                                                                                                                IsAvailableInDemo = entity.IsAvailableInDemo,
                                                                                                                                                Icon = entity.Icon.ToContentData(),
                                                                                                                                                ContentTreeMeta = entity.ContentTreeMeta.ToContentData(),
                                                                                                                                                Nodes = nodes
                                                                                                                                            };

        public static ContentData.ContentTreeMeta ToContentData(this ContentTreeMetaEntity entity) => new()
                                                                                                      {
                                                                                                          ContentTreeMetaId = entity.ContentTreeMetaId,
                                                                                                          Name = entity.Name,
                                                                                                          Depth = entity.Depth,
                                                                                                          CouldAddEntry = entity.CouldAddEntry,
                                                                                                          CouldAddContent = entity.CouldAddContent,
                                                                                                          AutoExpand = entity.AutoExpand,
                                                                                                      };

        public static ContentData.GlossaryTerm ToContentData(this ContentData.GlossaryTerm entity) => new()
                                                                                                      {
                                                                                                          TermId = entity.TermId,
                                                                                                          SubjectCode = entity.SubjectCode,
                                                                                                          Ref = entity.Ref,
                                                                                                          Term = entity.Term,
                                                                                                          Definition = entity.Definition,
                                                                                                          Header = entity.Header
                                                                                                      };
    }
}
