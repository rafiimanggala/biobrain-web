using System.Collections.Generic;
using Common.Enums;
using CustomControls.Interfaces;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IResultViewModel
    {
        List<IResultElement> Questions { get; set; }

        int Percent { get; set; }

        string HeaderString { get; set; }

        int AreaID { get; set; }

        int NextMaterialID { get; set; }

        string TopicIconPath { get; set; }

        int NextTopicMaterialID { get; set; }

        bool IsNextLevelVisible { get; set; }

        bool IsNextTopicVisible { get; set; }

        string AreaName { get; set; }
        string TopicName { get; set; }
        string LevelName { get; set; }

        void SaveRateResult(RateResult result);
        bool IsRateNeed();
    }
}