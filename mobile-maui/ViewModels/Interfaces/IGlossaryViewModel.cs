namespace BioBrain.ViewModels.Interfaces
{
    public interface IGlossaryViewModel : IBasePurchasableViewModel
    {
        string ErrorText { get; }
        int GetWordID(string word);
        bool CanNavigate(string letter);
    }
}