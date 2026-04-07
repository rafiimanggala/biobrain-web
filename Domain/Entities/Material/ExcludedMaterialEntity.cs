using System;
using Biobrain.Domain.Entities.SchoolClass;

namespace Biobrain.Domain.Entities.Material
{
    public class ExcludedMaterialEntity
    {
        public Guid ExcludedMaterialId { get; set; }

        public Guid SchoolClassId { get; set; }
        public SchoolClassEntity SchoolClass { get; set; }

        public Guid MaterialId { get; set; }
        public MaterialEntity Material { get; set; }

        public DateTime ExcludedAtUtc { get; set; }
    }
}
