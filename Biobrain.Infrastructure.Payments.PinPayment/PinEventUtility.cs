using Biobrain.Infrastructure.Payments.PinPayments.Models;
using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments
{
    public static class PinEventUtility
    {
        public static PinEvent ParseEvent(string json) => JsonConvert.DeserializeObject<PinEvent>(json);
    }
}
