using System;
using System.Collections.Generic;
using Biobrain.Domain.Entities.School;

namespace Biobrain.Application.Specifications
{
    public static class SchoolSpec
    {
        public static Spec<SchoolEntity> ById(Guid id) => new(_ => _.SchoolId == id);
        public static Spec<SchoolEntity> ByIds(List<Guid> ids) => new(_ => ids.Contains(_.SchoolId));
        public static Spec<SchoolEntity> OtherSchools(Guid id) => new(_ => _.SchoolId != id);
        public static Spec<SchoolEntity> WithName(string name) => new(_ => _.Name == name);
    }
}