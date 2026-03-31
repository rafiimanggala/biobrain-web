using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.User
{
    public class UserPageViewEntity : ICreatedEntity, IDeletedEntity
    {
        public Guid UserPageViewId { get; set; }

        public string PagePath { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid? SchoolId { get; set; }
        public SchoolEntity School { get; set; }

        public Guid? CourseId { get; set; }
        public CourseEntity Course { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}