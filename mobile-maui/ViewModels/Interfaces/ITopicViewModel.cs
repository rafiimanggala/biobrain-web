namespace BioBrain.ViewModels.Interfaces
{
    public interface ITopicViewModel
    {
        int TopicID { get; }
        string Name { get; }
        bool IsDone { get; set; }
        string CompletionString { get; set; }
        string DarkIconPath { get; }
        int MaterialID { get; }
    }
}