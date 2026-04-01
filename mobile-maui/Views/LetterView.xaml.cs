using System;
using BioBrain.Extensions;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common.Enums;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class LetterView
    {
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();
        private ILetterViewModel ViewModel
        {
            get => (ILetterViewModel)BindingContext;
            set => BindingContext = value;
        }

        public LetterView(ILetterViewModel viewModel) : base(MenuItemsEnum.Glossary)
        {
            analyticTracker.SetView("Glossary Words List Page", nameof(LetterView));
            InitializeComponent();
            ViewModel = viewModel;
            AddWords();
        }

        private void AddWords()
        {
            WordsStack.Children.Clear();
            foreach (var word in ViewModel.Words)
            {
                var content = new ContentView
                {
                    Style = (Style)Application.Current.Resources["WordLabelContentStyle"],
                    Content = new Label
                    {
                        Text = word.Term,
                        Style = (Style)Application.Current.Resources["WordLabelStyle"],

                    }
                };
                var gesture = new TapGestureRecognizer
                {
                    Command = new Command(GestureOnTapped),
                    CommandParameter = word.WordID
                };
                content.GestureRecognizers.Add(gesture);

                WordsStack.Children.Add(content);
            }
        }

        private async void GestureOnTapped(object parameter)
        {
            var wordID = (int) parameter;
            await Navigation.MoveToWord(wordID);
        }

        private void TopBaner_OnShare(object sender, EventArgs e)
        {
            if (Popup.IsVisible)
            {
                Popup.ClosePopup();
            }
            else
            {
                Popup.InitPopup();
            }
        }
    }
}
