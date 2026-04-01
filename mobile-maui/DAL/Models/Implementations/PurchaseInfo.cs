using DAL.Models.Interfaces;

namespace DAL.Models.Implementations
{
    public class PurchaseInfo : IPurchaseInfo
    {
        public string Name { get; set; }
        public string ProductId { get; set; }
        public double Cost { get; set; }
    }
}