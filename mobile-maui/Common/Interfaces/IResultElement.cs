using Microsoft.Maui.Controls;

namespace CustomControls.Interfaces
{
    public interface IResultElement
    {
        string Name { get; set; }

        Color Color { get; set; }

        int QuestionID { get; set; }
    }
}