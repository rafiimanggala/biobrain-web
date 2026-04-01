using System.Collections.Generic;
using System.ComponentModel;
using CustomControls.Interfaces;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IMaterialsViewModel : IBasePurchasableViewModel, INotifyPropertyChanged
    {
        string TopicIconPath { get; set; }
        bool IsHaveQuestions { get; }
        string AreaName { get; set; }
        string TopicName { get; set; }
        string LevelName { get; set; }
        List<ILevelBarElement> Levels { get; set; }
        string Text { get; }
        string FilePath { get; set; }
        int MaterialID { get; }
        int TopicID { get; set; }
        int AreaID { get; set; }
        bool IsDone { get; set; }
        void PropertiesChanged();
        IWordViewModel GetGlossryTerm(string reference);
        void SetQuizButtonText();
        int GetMaterial(int topicId, int levelId);
    }
}