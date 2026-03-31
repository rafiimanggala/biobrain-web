using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Material
{
    public class BookmarkEntity : ICreatedEntity
    {
        public Guid BookmarkId { get; set; }

        public Guid CourseId { get; set; }
        public CourseEntity Course { get; set; }

        public Guid MaterialId { get; set; }
        public MaterialEntity Material { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}