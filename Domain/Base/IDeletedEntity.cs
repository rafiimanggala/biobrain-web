using System;

namespace Biobrain.Domain.Base
{
    public interface IDeletedEntity
    {
        DateTime? DeletedAt { get; set; }
    }
}
