using System.Collections.Generic;

namespace BioBrain.ViewModels.Interfaces
{
    public interface ILevelsViewModel
    {
        List<ILevelViewModel> Levels { get; set; }

        string TopicName { get; }

        string TopicIconPath { get; }

        string TopicImagePath { get; }
    }
}