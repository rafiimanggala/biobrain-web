using System;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Application.Specifications
{
    public static class UserSpec
    {
        public static Spec<UserEntity> ById(Guid id) => new(_ => _.Id == id);
        public static Spec<UserEntity> WithLoginName(string name) => new(_ => name != null && _.NormalizedUserName == name.ToUpper());
        public static Spec<UserEntity> WithLoginNameOrEmail(string s) => new(_ => s != null && _.NormalizedEmail == s.ToUpper() || s != null && _.NormalizedUserName == s.ToUpper());
    }
}