namespace DAL.Models.Interfaces
{
    public interface IFirebasePurchaseModel
    {
	    string ProductId { get; set; }
        int Date { get; set; }
        string DisplayDate { get; set; }
        string Id { get; set; }
        string Token { get; set; }
        string State { get; set; }
    }
}