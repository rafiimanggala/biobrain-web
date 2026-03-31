using System;
using System.Collections.Generic;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.User
{
    public class UserSessionEntity : IDeletedEntity
    {
        public Guid UserSessionId { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public List<UserSessionSchoolEntity> Schools { get; set; }
        public List<UserSessionCourseEntity> Courses { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime LastTrack { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}