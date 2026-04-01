using DAL.Models.Interfaces;

namespace DAL.Models.Implementations
{
    public class FirebasePurchaseModel : IFirebasePurchaseModel
    {
	    public string ProductId { get; set; }
        public int Date { get; set; }
        public string DisplayDate { get; set; }
        public string Id { get; set; }
        public string Token { get; set; }
        public string State { get; set; }
    }
}