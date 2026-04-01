namespace Common.Interfaces
{
    public interface IStatEntryViewModel
    {
        string Topic { get; set; }
        string Level { get; set; }
        string Score { get; set; }
        string Date { get; set; }
        int TopicId { get; set; }
        int MaterialId { get; set; }
        bool IsNeedToSend { get; set; }
    }
}