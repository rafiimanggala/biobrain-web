namespace BioBrain.ViewModels.Implementation
{
    public class AnswerResultViewModel
    {
        public bool IsCorrect { get; set; }

        public bool IsSecondTry { get; set; }

        public string Text { get; set; }

        public string FilePath { get; set; }

        public int Score { get; set; }
    }
}