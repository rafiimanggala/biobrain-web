using System;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IAreaViewModel
    {
        string Name { get; }
        int Id { get; }
        bool IsDone { get; set; }
        string CompletionString { get; set; }
        bool IsComingSoon { get; }
    }
}
