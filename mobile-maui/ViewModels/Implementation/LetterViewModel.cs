using System;
using System.Collections.Generic;
using System.Linq;
using BioBrain.ViewModels.Interfaces;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;
// using Xamarin.Forms.Internals; // TODO: Review MAUI equivalent

namespace BioBrain.ViewModels.Implementation
{
    public class LetterViewModel : ILetterViewModel
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IGlossaryRepository glossaryRepository;

        public LetterViewModel(char letter, IGlossaryRepository glossaryRepository)
        {
            this.glossaryRepository = glossaryRepository;
            Letter = letter.ToString();
            Words = new List<IWordViewModel>();
            try { GetWords(); }
            catch (Exception ex) { logger.Log($"LetterViewModel constructor error: {ex.Message}"); }
        }

        private void GetWords()
        {
            Words = new List<IWordViewModel>();
            var models = glossaryRepository.GetByLetter(Letter);
            models.OrderBy(m => m.Term).ToList().ForEach(m => Words.Add(App.Container.Resolve<IWordViewModel>(new ParameterOverride("wordID", m.TermID))));
        }

        public List<IWordViewModel> Words { get; set; }

        public string Letter { get; }
    }
}