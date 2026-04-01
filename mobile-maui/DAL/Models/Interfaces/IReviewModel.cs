using Newtonsoft.Json.Serialization;

namespace DAL.Models.Interfaces
{
    public interface IReviewModel
    {
        string Body { get; set; }
        int CreateAt { get; set; }
        string UID { get; set; }
        string Email { get; set; }
    }
}