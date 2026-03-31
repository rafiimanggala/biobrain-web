using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Domain.Entities.Material;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Projections
{
    public static class PageProjection
    {
        public static Expression<Func<PageEntity, ContentData.Page>> ToPageContentData()
            => _ => new ContentData.Page
                    {
                        PageId = _.PageId,
                        CourseId = _.ContentTreeNode.CourseId,
                        ContentTreeNodeId = _.ContentTreeNode.NodeId,
                        Materials = _.PageMaterials
                                     .Select(x => new ContentData.Material
                                                  {
                                                      Order = x.Order,
                                                      Text = x.Material.Text,
                                                      Header = x.Material.Header,
                                                      MaterialId = x.MaterialId,
                                                      VideoLink = x.Material.VideoLink
                                     })
                                     .ToList()
                                     .ToImmutableList()
                    };

        public static IQueryable<PageEntity> PrepareToMapToPageContentData(this IQueryable<PageEntity> query)
            => query.Include(_ => _.PageMaterials)
                    .ThenInclude(_ => _.Material);
    } 
}
