using System;
using Biobrain.Domain.Entities.School;

namespace Biobrain.Domain.Entities.User
{
    public class UserSessionSchoolEntity
    {
        public Guid UserSessionSchoolId { get; set; }

        public Guid UserSessionId { get; set; }
        public UserSessionEntity UserSession { get; set; }

        public Guid? SchoolId { get; set; }
        public SchoolEntity School { get; set; }
    }
}