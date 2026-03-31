using System;

namespace Biobrain.Domain.Base
{
    public interface ICreatedEntity
    {
        DateTime CreatedAt { get; set; }
    }
}
