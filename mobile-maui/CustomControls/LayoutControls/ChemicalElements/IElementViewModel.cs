using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls.ChemicalElements
{
    /// <summary>
    /// Interface for chemical element view model used in periodic table display.
    /// </summary>
    public interface IElementViewModel
    {
        string Name { get; set; }
        string ShortName { get; set; }
        int AtomicNumber { get; set; }
        float MassNumber { get; set; }
        Color BackgroundColor { get; set; }
        Color BorderColor { get; set; }
        Color FontColor { get; set; }
        int X { get; set; }
        int Y { get; set; }
        bool IsGroupFirst { get; set; }
        bool IsPeriodFirsr { get; set; }
        string BlockName { get; set; }
    }
}
