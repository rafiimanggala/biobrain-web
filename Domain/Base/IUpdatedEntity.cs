using System;

namespace Biobrain.Domain.Base
{
    public interface IUpdatedEntity
    {
        DateTime UpdatedAt { get; set; }
    }
}
