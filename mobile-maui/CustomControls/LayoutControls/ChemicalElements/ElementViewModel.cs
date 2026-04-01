using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls.ChemicalElements
{
    /// <summary>
    /// View model for a single chemical element in the periodic table.
    /// </summary>
    public class ElementViewModel : IElementViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int AtomicNumber { get; set; }
        public float MassNumber { get; set; }
        public Color BackgroundColor { get; set; } = Colors.Gray;
        public Color BorderColor { get; set; } = Colors.Transparent;
        public Color FontColor { get; set; } = Colors.Black;
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsGroupFirst { get; set; }
        public bool IsPeriodFirsr { get; set; }
        public string BlockName { get; set; } = string.Empty;
    }
}
