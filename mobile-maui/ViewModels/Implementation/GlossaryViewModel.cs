using System;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class GlossaryViewModel : BasePurchasableViewModel, IGlossaryViewModel
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IGlossaryRepository glossaryRepository;
        public string ErrorText => CustomCssStyles.GlossaryPopupStyle + StringResource.WordNotFoundString;

        //Used through binding
        public bool IsTableVisible => Settings.IsPeriodicTableVisible;

        public GlossaryViewModel(IGlossaryRepository glossaryRepository)
        {
            this.glossaryRepository = glossaryRepository;
        }

        public int GetWordID(string word)
        {
            try
            {
                var glossaryModel = glossaryRepository.GetByTerm(word);
                if (glossaryModel == null) return -1;
                return glossaryModel.TermID;
            }
            catch (Exception ex)
            {
                logger.Log($"GlossaryViewModel GetWordID error: {ex.Message}");
                return -1;
            }
        }

        public bool CanNavigate(string letter)
        {
            try
            {
                return glossaryRepository.GetByLetter(letter).Count > 0 || !Settings.IsDemo;
            }
            catch (Exception ex)
            {
                logger.Log($"GlossaryViewModel CanNavigate error: {ex.Message}");
                return false;
            }
        }
    }
}