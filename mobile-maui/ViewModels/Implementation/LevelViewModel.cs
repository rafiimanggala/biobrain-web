using BioBrain.ViewModels.Interfaces;
using CustomControls.Interfaces;
using DAL.Models.Interfaces;

namespace BioBrain.ViewModels.Implementation
{
    public class LevelViewModel : ILevelViewModel
    {
        public string Name => typeModel?.LevelName.ToUpper();
        int ILevelBarElement.Key => MaterialID;
        public string ShortName => typeModel?.LevelShortName;
        public string Image => typeModel.Image;
        public int MaterialID => materialModel?.MaterialID ?? -typeModel?.LevelTypeID ?? -1;
        public bool IsDone { get; set; }
        public bool IsSelected { get; set; }

        private readonly IMaterialModel materialModel;
        private readonly ILevelTypeModel typeModel;

        public LevelViewModel(IMaterialModel materialModel, ILevelTypeModel typeModel)
        {
            this.materialModel = materialModel;
            this.typeModel = typeModel;
        }
    }
}
