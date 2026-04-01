using DAL.Models.Interfaces;
using Newtonsoft.Json;

namespace DAL.Models.Implementations
{
    public class ReviewModel : IReviewModel
    {
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("createdAt")]
        public int CreateAt { get; set; }
        [JsonProperty("uid")]
        public string UID { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}