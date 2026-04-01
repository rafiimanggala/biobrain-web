using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using Common;
using CustomControls;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class QuestionReviewViewModel : IQuestionReviewViewModel
    {
        public string FilePath { get; private set; }

        public string TopicName { get; }

        public bool IsCorrect { get; private set; }

        public Color HexagonColor => IsCorrect ? CustomColors.DarkGreen : CustomColors.Red;

        public string QuestionHeader { get; private set; }

        public string TopicIconPath { get; set; }

        private readonly IAnswersRepository answersRepository;
        private readonly IDoneQuestionsRepository doneQuestionsRepository;
        private readonly IGlossaryRepository glossaryRepository;
        private readonly IQuestionModel model;

        public QuestionReviewViewModel(int questionID, IQuestionsRepository questionsRepository,IAnswersRepository answersRepository, IDoneQuestionsRepository doneQuestionsRepository, IGlossaryRepository glossaryRepository)
        {
            model = questionsRepository.GetByID(questionID);
            var topic = questionsRepository.GetTopic(model.MaterialID);
            TopicName = topic.TopicName.ToUpper();
            TopicIconPath = Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, topic.Image);
            this.answersRepository = answersRepository;
            this.doneQuestionsRepository = doneQuestionsRepository;
            this.glossaryRepository = glossaryRepository;

            SetData();
        }

        private void SetData()
        {
            IsCorrect = doneQuestionsRepository.GetByQuestionID(model.QuestionID).IsCorrect;
            QuestionHeader = "Q" + model.QuestionOrder;
            switch (model.QuestionType)
            {
                case (int)QuestionTypes.MultipleChoice: 
                case (int)QuestionTypes.TrueFalse: 
                case (int)QuestionTypes.FreeText:
                    CreateSimpleFile();
                    break;
                case (int)QuestionTypes.DropDown:
                case (int)QuestionTypes.CompleteSentence:
                    CreateDropDownFile();
                    break;
                case (int)QuestionTypes.OrderList:
                    CreateOrderListFile();
                    break;
                case (int)QuestionTypes.Swipe:
                    CreateSwipeListFile();
                    break;
            }
        }

        private void CreateSwipeListFile()
        {
            try
            {
                var text = model.Text;
                var answers = answersRepository.GetForQuestion(model.QuestionID);
                var correctAnswer = answers.FirstOrDefault(a => a.IsCorrect);
                if (correctAnswer == null) return;

                var group1Answers = string.Empty;
                group1Answers = answers.Where(a => a.AnswerOrder == 1)
                    .Aggregate(group1Answers, (result, a2) => result + a2.Text + ", ");
                group1Answers = group1Answers.Substring(0, group1Answers.Length - 2);
                var group2Answers = string.Empty;
                group2Answers = answers.Where(a => a.AnswerOrder == 2)
                    .Aggregate(group2Answers, (result, a2) => result + a2.Text + ", ");
                group2Answers = group2Answers.Substring(0, group2Answers.Length - 2);


                text = ProcessGroups(text, out var group1, out var group2);
                text = RemoveColumns(text);
                text = RemoveAnswerButton(text);
                text = text.Replace("</html>",
                    $"<div class=\"correctBlock\"><p class=\"correctString\">{StringResource.CorrectString} </p><p class=\"answerString\">{group1}:&nbsp;{group1Answers};</p><p class=\"answerString\">{group2}:&nbsp;{group2Answers};</p><p class=\"feedback\">{correctAnswer.Feedback.Replace("<p>", "<p class=\"feedback\">")}</p></div></html>");
                text = RemoveHeader(text);

                FilePath = WriteDataToFile(CustomCssStyles.QuestionsStyles + text);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void CreateOrderListFile()
        {
            var text = model.Text;
            var answers = answersRepository.GetForQuestion(model.QuestionID);
            var correctAnswer = answers.FirstOrDefault(a => a.IsCorrect);
            if (correctAnswer == null) return;

            string pattern = @"<[\s]*div[\s]*class=""container""[\s]*>([\s\S]+?)</div>";
            var reg = new Regex(pattern);
            text = reg.Replace(text,
                $"<div class=\"correctBlock\"><p class=\"correctString\">{StringResource.CorrectString} </p><p class=\"answerString\">{correctAnswer.Text}</p><p class=\"feedback\">{correctAnswer.Feedback.Replace("<p>", "<p class=\"feedback\">")}</p></div>");
            text = RemoveHeader(text);
            text = RemoveAnswerButton(text);

            FilePath = WriteDataToFile(CustomCssStyles.QuestionsStyles + text);
        }

        private void CreateDropDownFile()
        {
            //< span class="selectBox">
            var text = model.Text;
            var answers = answersRepository.GetForQuestion(model.QuestionID);
            var correctAnswer = answers.Where(a => a.IsCorrect).ToList();
            if (!correctAnswer.Any()) return;

            string pattern = @"<[\s]*span[\s]*class=""selectBox( wide)?""[\s]*>([\s\S]+?)</span>";
            var reg = new Regex(pattern);
            var match = reg.Match(text);
            var i = 0;
            while (match.Success)
            {
                text = text.Replace(match.Value, $"<span class=\"answerWord\">{correctAnswer[i].Text}</span>");
                match = match.NextMatch();
                i++;
            }
            text = RemoveAnswerButton(text);
            text = RemoveHeader(text);

            pattern = @"<[\s]*Button[\s]*class=""answerButton""[\s\S]+?</Button>";
            reg = new Regex(pattern);
            text = reg.Replace(text, string.Empty /*$"<p>{correctAnswer.First().Feedback}</p>"*/);

            FilePath = WriteDataToFile(CustomCssStyles.QuestionsStyles + text);
        }

        private string RemoveHeader(string text)
        {
            var pattern = @"<h2 class=""questionText"">[\s\S]+</h2>";
            var reg = new Regex(pattern);
            return reg.Replace(text, string.Empty);
        }

        private void CreateSimpleFile()
        {
            var text = model.Text;
            var answers = answersRepository.GetForQuestion(model.QuestionID);
            var correctAnswer = answers.FirstOrDefault(a=>a.IsCorrect);
            if (correctAnswer == null) return;

            string pattern = @"<form[\s\S]+</form>";
            var reg = new Regex(pattern);
            text = reg.Replace(text,
                $"<div class=\"correctBlock\"><p class=\"correctString\">{StringResource.CorrectString} </p><p class=\"answerString\">{correctAnswer.Text}</p><p class=\"feedback\">{correctAnswer.Feedback.Replace("<p>", "<p class=\"feedback\">")}</p></div>");
            text = RemoveHeader(text);

            FilePath = WriteDataToFile(CustomCssStyles.QuestionsStyles + text);
        }

        private string WriteDataToFile(string text)
        {
            const string fileName = "m.html";
            var paths = DependencyService.Get<IFilesPath>();
            var fileWorker = DependencyService.Get<IWorkingWithFiles>();
            fileWorker.CreateHtmlFile(fileName, text);
            return Path.Combine("file://" + paths.PagesPath, fileName);
        }

        public IWordViewModel GetGlossryTerm(string reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            var glossryModel = glossaryRepository.GetByRef(reference);
            return App.Container.Resolve<IWordViewModel>(new ParameterOverride("wordID", glossryModel.TermID));
        }

        protected const string Content = "content";
        protected const string Class = "class";
        protected const string CenterOpenTag = "<center>";
        protected const string OpenSpanTag = "<span>";
        protected const string CloseSpanTag = "</span>";
        protected const string ButtonPattern = "<[Bb]utton.*?class=\"(?'" + Class + "'.*?)\"(?s).*?>(?'" + Content + "'(?s).*?)</[Bb]utton>";
        protected const string LeftColumnHeaderClass = "leftColumnHeader";
        protected const string RightColumnHeaderClass = "rightColumnHeader";
        private string ProcessGroups(string html, out string group1, out string group2)
        {
            group1 = string.Empty;
            group2 = string.Empty;
            foreach (var match in new Regex(ButtonPattern).Matches(html).Cast<Match>().OrderByDescending(m => m.Index))
            {
                if (match.Groups[Class]?.Value == LeftColumnHeaderClass)
                {
                    group1 = match.Groups[Content].Value.Replace(OpenSpanTag, string.Empty).Replace(CloseSpanTag, string.Empty);
                    html = html.Remove(match.Index, match.Length);
                    if (html.Substring(match.Index - CenterOpenTag.Length, CenterOpenTag.Length) == CenterOpenTag)
                        html = html.Remove(match.Index - CenterOpenTag.Length, CenterOpenTag.Length);
                }
                if (match.Groups[Class]?.Value == RightColumnHeaderClass)
                {
                    if (html.Substring(match.Index + match.Length + 1, CenterOpenTag.Length) == CenterOpenTag)
                        html = html.Remove(match.Index + match.Length + 1, CenterOpenTag.Length);
                    group2 = match.Groups[Content].Value.Replace(OpenSpanTag, string.Empty).Replace(CloseSpanTag, string.Empty);
                    html = html.Remove(match.Index, match.Length);
                }
            }

            return html;
        }
        
        protected const string ColumnsPattern = "<div[^>]*?class=\"container\"(?s)[^>]*?>(?'content'(?s).*?)</div>";
        private string RemoveColumns(string html)
        {
            foreach (var match in new Regex(ColumnsPattern).Matches(html).Cast<Match>().OrderByDescending(m => m.Index))
            {
                html = html.Remove(match.Index, match.Length);
            }
            return html;
        }

        protected const string AnswerButtonClass = "answerButton";
        private string RemoveAnswerButton(string html)
        {
            foreach (var match in new Regex(ButtonPattern).Matches(html).Cast<Match>().OrderByDescending(m => m.Index))
            {
                if (match.Groups["class"]?.Value == AnswerButtonClass)
                    html = html.Remove(match.Index, match.Length);
            }
            return html;
        }
    }
}