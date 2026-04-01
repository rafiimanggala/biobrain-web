namespace CustomControls.Interfaces
{
    public interface ILevelBarElement
    {
        string Name { get; }
        int Key { get; }
        bool IsSelected { get; set; }
    }
}