using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
using BioBrain.AppResources;
using BioBrain.Extensions;
using BioBrain.Helpers;
using BioBrain.ViewModels.Interfaces;
using Common;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;

namespace BioBrain.ViewModels.Implementation
{
    public class QuestionViewModel : IQuestionViewModel
    {
        private string filePath;

        public const string HtmlTag = "<html>";
        public bool IsSecondTry { get; set; }

        public string QuestionText => CustomCssStyles.QuestionsStyles + model.Text;

        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public QuestionTypes QuestionType => (QuestionTypes)model.QuestionType;

        public List<IBaseAnswerViewModel> Answers { get; set; }

        public int QuestionID => model.QuestionID;

        public string Hint => CustomCssStyles.AnswerPopupStyle + StringResource.HintHeader + model.Hint;

        public bool IsHintEnabled => !string.IsNullOrEmpty(model.Hint);

        public int QuestionOrder => model.QuestionOrder;

        private AnswerResultViewModel DefaultAnswer => new AnswerResultViewModel { Score = 0, IsCorrect = false, Text = IsSecondTry ? CustomCssStyles.AnswerPopupStyle + string.Format(StringResource.SecondTryString, GetCorrectAnswersText() + ".", GetCorrectAnswer().FeedBack) : CustomCssStyles.AnswerPopupStyle + string.Format(StringResource.FirstTryString, HintTextForWrongAnswer) };

        private string HintTextForWrongAnswer => IsHintEnabled ? StringResource.HintTextForWrongAnswer : string.Empty;

        private readonly IQuestionModel model;
        private readonly IAnswersRepository answersRepository;
        private readonly IDoneQuestionsRepository doneQuestionsRepository;

        public QuestionViewModel(IQuestionModel model, IAnswersRepository answersRepository, IDoneQuestionsRepository doneQuestionsRepository)
        {
            this.model = model;
            this.answersRepository = answersRepository;
            this.doneQuestionsRepository = doneQuestionsRepository;

            GetData();
        }

        private void GetData()
        {
            switch (QuestionType)
            {
                case QuestionTypes.MultipleChoice:
                case QuestionTypes.FreeText:
                case QuestionTypes.DropDown:
                case QuestionTypes.CompleteSentence:
                case QuestionTypes.TrueFalse:
                    break;
                case QuestionTypes.OrderList:
                    model.Text = model.Text.Replace(HtmlTag, HtmlTag + CustomCssStyles.OlQuestionsScripts);
                    break;
                case QuestionTypes.Swipe:
                    model.Text = model.Text.Replace(HtmlTag, HtmlTag + CustomCssStyles.SwQuestionsScripts);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Answers = new List<IBaseAnswerViewModel>();
            var answerModels = answersRepository.GetForQuestion(QuestionID);

            foreach (var answerModel in answerModels)
            {
                var answer = App.Container.Resolve<IBaseAnswerViewModel>(new ParameterOverride("answerModel", answerModel));
                Answers.Add(answer);
            }
        }

        public AnswerResultViewModel CheckAnswer(WebViewResponseModel answer, bool isWriteFile = true)
        {
            AnswerResultViewModel result;
            switch (QuestionType)
            {
                case QuestionTypes.MultipleChoice:
                    result = CheckMultiChoice(answer.ParsedResponse.Values.First());
                    break;
                case QuestionTypes.FreeText:
                    result = CheckFreeTextResponse(answer.ParsedResponse.Values.First());
                    break;
                case QuestionTypes.TrueFalse:
                    result = CheckTrueFalseResponse(answer.ParsedResponse.Values.First());
                    break;
                case QuestionTypes.DropDown:
                case QuestionTypes.CompleteSentence:
                    result = CheckMultipleResponse(answer.ParsedResponse);
                    break;
                case QuestionTypes.Swipe:
                    result = CheckSwipeResponse(answer.ParsedResponse);
                    break;
                case QuestionTypes.OrderList:
                    result = CheckOrderListResponse(answer.ParsedResponse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            result.IsSecondTry = IsSecondTry;
            if (isWriteFile)
                result.FilePath = FileHelper.WriteFile(result.Text, FileTypes.Answer);
            if (!IsSecondTry)
                SaveData(result);

            IsSecondTry = true;
            return result;
        }

        private void SaveData(AnswerResultViewModel result)
        {
            doneQuestionsRepository.Insert(new DoneQuestionModel {Score = result.Score, IsCorrect = result.IsCorrect, QuestionID = QuestionID});
        }

        private AnswerResultViewModel CheckMultiChoice(string answerOrder)
        {
	        if (!int.TryParse(answerOrder, out var answerNumber))
                return DefaultAnswer;
            var answer = Answers.FirstOrDefault(a => a.Order == answerNumber);
            if (answer == null) return DefaultAnswer;
            return answer.IsCorrect ? new AnswerResultViewModel {IsCorrect = true, Text = CustomCssStyles.AnswerPopupStyle + GetFeedBack(answer), Score = answer.Score} : DefaultAnswer;
        }

        private AnswerResultViewModel CheckMultipleResponse(Dictionary<string, string> responses)
        {
            var answers = new List<IBaseAnswerViewModel>();
            var i = 0;
            foreach (var answer in responses.Select(response => Answers.FirstOrDefault(a => (a.Order == i + 1) && (a.Text == response.Value))))
            {
                if (answer == null || !answer.IsCorrect) return DefaultAnswer;
                answers.Add(answer);
                i++;
            }
            return new AnswerResultViewModel { IsCorrect = true, Text = CustomCssStyles.AnswerPopupStyle + GetFeedBack(answers.First()), Score = answers.First().Score };
        }

        private AnswerResultViewModel CheckFreeTextResponse(string answer)
        {
            answer = answer.RemoveExtraSpaces();
            var answerModel = Answers.FirstOrDefault(a => string.Equals(a.Text.RemoveTags(), answer, Answers.First().CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase));
            if (answerModel == null) return DefaultAnswer;
            return answerModel.IsCorrect ? new AnswerResultViewModel { IsCorrect = true, Text = CustomCssStyles.AnswerPopupStyle + GetFeedBack(answerModel), Score = answerModel.Score } : DefaultAnswer;
        }

        private AnswerResultViewModel CheckTrueFalseResponse(string answer)
        {
            var answerModel = Answers.FirstOrDefault(a => string.Equals(a.Text, answer, StringComparison.CurrentCultureIgnoreCase));
            if (answerModel == null) return DefaultAnswer;
            if (answerModel.CaseSensitive && !string.Equals(answer, answerModel.Text, StringComparison.CurrentCulture))
            {
                answerModel = Answers.FirstOrDefault(a => string.Equals(a.Text, answer, StringComparison.CurrentCulture));
                if (answerModel == null) return DefaultAnswer;
            }
            return answerModel.IsCorrect ? new AnswerResultViewModel { IsCorrect = true, Text = CustomCssStyles.AnswerPopupStyle + GetFeedBack(answerModel), Score = answerModel.Score } : DefaultAnswer;
        }

        private AnswerResultViewModel CheckSwipeResponse(Dictionary<string, string> responses)
        {
            //in response dictionary Key = {one of the answers}, Value={response{№}} because of answer - uniq and response only two
            var answerModels = Answers.Where(a => a.Order == 1);
            //If left column not full
            // Symbol = is forbidden inside SW answers so replace with equal
            if (answerModels.Any(answer => responses.Where(r => r.Value == Settings.Responce1).All(r => r.Key != ReplaceSymbolsForSwipe(answer.Text))))
                return DefaultAnswer;

            answerModels = Answers.Where(a => a.Order == 2);
            //If right column not full
            // Symbol = is forbidden inside SW answers so replace with equal
            if (answerModels.Any(answer => responses.Where(r => r.Value == Settings.Responce2).All(r => r.Key != ReplaceSymbolsForSwipe(answer.Text))))
                return DefaultAnswer;

            answerModels = Answers.Where(a => a.Order == 0);
            //If any answer from middle column (order=0) in left or right column
            if (answerModels.Any(answer => responses.Any(r => r.Key == ReplaceSymbolsForSwipe(answer.Text) )))
                return DefaultAnswer;

            return new AnswerResultViewModel { IsCorrect = true, Text = CustomCssStyles.AnswerPopupStyle + GetFeedBack(Answers.First()), Score = Answers.First().Score };
        }

        private string ReplaceSymbolsForSwipe(string text)
        {
	        // Symbol = and / is forbidden inside SW answers so replace with equal and div
	        return text.Replace("=", "equal").Replace("/", "div");
        }

        private AnswerResultViewModel CheckOrderListResponse(Dictionary<string, string> responses)
        {
            //If all responses have correct order
            if (Answers.TrueForAll(a => responses.Where(r=> r.Key == Settings.Responce +a.Order).Any(r => r.Value.RemoveTags() == a.Text.RemoveTags())))
                return new AnswerResultViewModel { IsCorrect = true, Text = CustomCssStyles.AnswerPopupStyle + GetFeedBack(Answers.First()), Score = Answers.First().Score };
            return DefaultAnswer;
        }

        public IBaseAnswerViewModel GetCorrectAnswer()
        {
            return Answers.FirstOrDefault(a => a.IsCorrect);
        }

        private string GetCorrectAnswersText()
        {
            var result = string.Empty;
            if (QuestionType == QuestionTypes.DropDown || QuestionType == QuestionTypes.CompleteSentence)
            {
                Answers.FindAll(a => a.IsCorrect).ForEach(a => result = $"{result}{a.Text}, ");
                result = result.Remove(result.Length - 2);
            }
            else
            {
                result = GetCorrectAnswer().Text;
            }
            return result;
        }

        private string GetFeedBack(IBaseAnswerViewModel answerModel)
        {
            return string.IsNullOrEmpty(answerModel.FeedBack) ? StringResource.YouAcirrecrString : answerModel.FeedBack.Replace("<html>", StringResource.YouAreCorrectReplaceString);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}