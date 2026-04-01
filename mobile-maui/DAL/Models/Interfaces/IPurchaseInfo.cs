namespace DAL.Models.Interfaces
{
    public interface IPurchaseInfo
    {
        string Name { get; set; }
        string ProductId { get; set; }
        double Cost { get; set; }
    }
}