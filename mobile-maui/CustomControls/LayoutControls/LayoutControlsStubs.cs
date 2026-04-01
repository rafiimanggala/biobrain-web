using System;

// Shared types (enums, event args) used by CustomControls.LayoutControls.
// PopupStyles, PopupCloseEventArgs, PopupContentChangigEventArgs moved here since
// PopupWebView.xaml.cs is now a partial class and these types are shared across controls.

namespace CustomControls.LayoutControls
{
    /// <summary>
    /// Popup display styles used by various views.
    /// </summary>
    public enum PopupStyles
    {
        Glossary,
        Answer,
        Hint,
        Message
    }

    /// <summary>
    /// Event args for popup close events.
    /// </summary>
    public class PopupCloseEventArgs : EventArgs
    {
        public PopupCloseEventArgs() { }

        public PopupCloseEventArgs(object result)
        {
            Results = result;
        }

        public object Results { get; set; }
    }

    /// <summary>
    /// Event args for popup content changing events.
    /// Note: original typo "Changig" preserved for compatibility.
    /// </summary>
    public class PopupContentChangigEventArgs : EventArgs
    {
        public PopupContentChangigEventArgs() { }

        public PopupContentChangigEventArgs(string result)
        {
            Results = result;
        }

        public string Results { get; set; }
    }

    /// <summary>
    /// Event args for search bar events.
    /// </summary>
    public class SearchEventArgs : EventArgs
    {
        public SearchEventArgs() { }

        public SearchEventArgs(string searchText)
        {
            SearchText = searchText;
        }

        public string SearchText { get; set; }
    }

    /// <summary>
    /// Event args for avatar/item selection events.
    /// </summary>
    public class SelectedEventArgs : EventArgs
    {
        public string Path { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Event args for hexagon result control touch events.
    /// </summary>
    public class HexagonResultControlEventArgs : EventArgs
    {
        public HexagonResultControlEventArgs() { }

        public HexagonResultControlEventArgs(int id)
        {
            SelectedElementID = id;
        }

        public int SelectedElementID { get; set; }
    }

    /// <summary>
    /// Event args for alphabet control touch events.
    /// </summary>
    public class AlphabetControlEventArgs : EventArgs
    {
        public AlphabetControlEventArgs(char letter) { Letter = letter; }

        public char Letter { get; set; }
    }
}
