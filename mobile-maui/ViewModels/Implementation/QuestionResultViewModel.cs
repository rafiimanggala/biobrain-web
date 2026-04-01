using CustomControls.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class QuestionResultViewModel : IResultElement
    {
        public Color Color { get; set; }

        public string Name { get; set; }

        public int QuestionID { get; set; }
    }
}