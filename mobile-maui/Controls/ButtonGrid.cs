using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace BioBrain.Controls
{
    enum ButtonState
    {
        Unpresed,
        Presed
    }

    class ButtonGrid : View
    {
        public Color PresedBackGroundColor { get; set; }

        public Color UnpresedBackGroundColor { get; set; }

        private ButtonState state = ButtonState.Unpresed;

        public ButtonGrid()
        {
            Focused += ChangeColor;
            Unfocused += ChangeColorBack;
            PresedBackGroundColor = Colors.Blue;
            UnpresedBackGroundColor = Colors.Transparent;
            BackgroundColor = Colors.Transparent;
        }

        private void ChangeColor(object sender, FocusEventArgs focusEventArgs)
        {
            if (state != ButtonState.Unpresed) return;
            BackgroundColor = PresedBackGroundColor;
            state = ButtonState.Presed;
        }

        private void ChangeColorBack(object sender, FocusEventArgs focusEventArgs)
        {
            if (state != ButtonState.Presed) return;
            BackgroundColor = UnpresedBackGroundColor;
            state = ButtonState.Unpresed;
        }
    }
}
