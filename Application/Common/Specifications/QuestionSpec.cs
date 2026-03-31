using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Question;


namespace Biobrain.Application.Specifications
{
    public static class QuestionSpec
    {
        public static Spec<QuestionEntity> ByIds(IEnumerable<Guid> questionIds) => new(_ => questionIds.Contains(_.QuestionId));
    }
}
