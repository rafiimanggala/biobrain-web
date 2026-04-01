using System.Collections.Generic;

namespace BioBrain.ViewModels.Interfaces
{
    public interface ILetterViewModel
    {
        List<IWordViewModel> Words { get; set; }

        string Letter { get; }
    }
}