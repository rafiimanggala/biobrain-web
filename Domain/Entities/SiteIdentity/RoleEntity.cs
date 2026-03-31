using System;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Domain.Entities.SiteIdentity
{
    public class RoleEntity : IdentityRole<Guid>
    {
        public RoleEntity() 
            : base() { }

        public RoleEntity(string roleName)
            : base(roleName) { }
    }

    
}
