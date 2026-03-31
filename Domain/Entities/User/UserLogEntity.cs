using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.User
{
    public class UserLogEntity : ICreatedEntity
    {
        public Guid UserLogId { get; set; }

        public string Log { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}