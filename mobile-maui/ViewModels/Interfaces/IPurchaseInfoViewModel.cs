namespace BioBrain.ViewModels.Interfaces
{
    public interface IPurchaseInfoViewModel
    {
        string Name { get; }
        string PurchaseId { get; }
        string PurchaseLocalId { get; }
        double Cost { get; }
        string PurchaseDisplayName { get; }
    }
}