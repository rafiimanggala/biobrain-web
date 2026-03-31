using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Biobrain.Application.Common.Models
{
    [PublicAPI]
    public class CollectionUpdateModel<T>
    {
        public List<T> ToAdd { get; set; } = new List<T>();
        public List<T> ToRemove { get; set; } = new List<T>();

        public IEnumerable<T> All => ToAdd.Union(ToRemove);
    }
}