using System;
using System.Globalization;
using BioBrain.AppResources;
using BioBrain.Helpers;
using BioBrain.ViewModels.Interfaces;
using Common.Interfaces;
using DAL.Models.Interfaces;
using DAL.Repositorys.Interfaces;
// using Unity; // Replaced by MAUI DI
using Unity.Resolution;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    class WordViewModel : IWordViewModel
    {
        readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        private readonly IGlossaryModel model;
        private readonly IGlossaryRepository glossaryRepository;

        public WordViewModel(int wordID, IGlossaryRepository glossaryRepository)
        {
            this.glossaryRepository = glossaryRepository;
            try { model = glossaryRepository.GetByID(wordID); }
            catch (Exception ex) { logger.Log($"WordViewModel constructor error: {ex.Message}"); }
        }

        public IWordViewModel GetGlossaryTerm(string reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            var glossaryModel = glossaryRepository.GetByRef(reference);
            return glossaryModel == null ? null : App.Container.Resolve<IWordViewModel>(new ParameterOverride("wordID", glossaryModel.TermID));
        }

        public int WordID => model?.TermID ?? 0;
        public string Term => model == null ? string.Empty : (string.IsNullOrEmpty(model.Header) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.Term) : model.Header);
        public string Defenition => model?.Definition ?? string.Empty;
        public string PopupHtmlText => CustomCssStyles.GlossaryPopupStyle +
            Defenition.Replace("<html>", $"<html><span class=\"termin\">{Header}</span>");
        public string HtmlText => CustomCssStyles.GlossaryPopupStyle + Defenition;
        public string FilePath => CreateFile(HtmlText);
        public string PopupFilePath => CreateFile(PopupHtmlText);

        private string Header => model == null ? string.Empty : (string.IsNullOrEmpty(model.Header) ? Term.ToUpper() : model.Header);
        private string CreateFile(string text)
        {
            return FileHelper.WriteFile(text, FileTypes.Glossary);
        }
    }
}
