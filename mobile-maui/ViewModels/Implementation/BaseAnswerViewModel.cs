using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using DAL.Models.Interfaces;

namespace BioBrain.ViewModels.Implementation
{
    public class BaseAnswerViewModel : IBaseAnswerViewModel
    {
        private readonly IAnswerModel answerModel;

        public BaseAnswerViewModel(IAnswerModel answerModel)
        {
            this.answerModel = answerModel;
        }

        public int AnswerID => answerModel.AnswerID;
        public string Text => answerModel.Text;
        public bool IsCorrect => answerModel.IsCorrect;
        public bool CaseSensitive => answerModel.CaseSensitive;
        public string FeedBack => answerModel.Feedback;
        public int Score => answerModel.Score;
        public int Order => answerModel.AnswerOrder;
    }
}