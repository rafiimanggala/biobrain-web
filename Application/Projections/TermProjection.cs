using System;
using System.Linq.Expressions;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Domain.Entities.Glossary;


namespace Biobrain.Application.Projections
{
    public static class TermProjection
    {
        public static Expression<Func<TermEntity, ContentData.GlossaryTerm>> ToGlossaryTermContentData()
            => _ => new ContentData.GlossaryTerm
                    {
                        TermId = _.TermId,
                        SubjectCode = _.SubjectCode,
                        Ref = _.Ref,
                        Term = _.Term,
                        Definition = _.Definition,
                        Header = _.Header
                    };
    }
}
