using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Material
{
    public class MaterialEntity: IDeletedEntity
    {
        public Guid MaterialId { get; set; }

        public Guid CourseId { get; set; }

        public string Text { get; set; }

        public string Header { get; set; }

        public string VideoLink { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}