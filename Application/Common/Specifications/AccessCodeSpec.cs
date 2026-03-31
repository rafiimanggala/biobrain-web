using Biobrain.Domain.Entities.AccessCodes;

namespace Biobrain.Application.Specifications
{
    public static class AccessCodeSpec
    {

        public static Spec<AccessCodeEntity> ByAccessCode(string accessCode)
        {
            var accessCodeParam = accessCode.ToUpper();
            return new(_ => _.Code == accessCodeParam);
        }
    }
}
