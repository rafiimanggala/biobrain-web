using Microsoft.Maui.Graphics;

namespace CustomControls
{
    public static class CustomColors
    {
        public static Color DarkGreen =>
#if Chemistry
            DarkMainGreen;
#elif Biology
            Color.FromArgb("#4ead33");
#elif Physics
            Color.FromArgb("#4ead33");
#endif
        public static Color GrayColor => Color.FromArgb("#484848");
        public static Color LightGreen => Color.FromArgb("#95D65C");
        public static Color Red => Color.FromArgb("#e30918");
        public static Color LightRed => Color.FromArgb("#ffbcad");
        public static Color DarkGray => Color.FromArgb("#55565a");
        public static Color LightDarkBlue => Color.FromArgb("#98d5e9");
        public static Color LightDarkGreen => Color.FromArgb("#66b331");

        //PeriodicTable
        public static string DarkMainGreenString => "#0A552A";
        public static string LightMainGreenString => "#23A23E";
        public static Color DarkMainGreen => Color.FromArgb(DarkMainGreenString);
        public static Color LightMainGreen => Color.FromArgb(LightMainGreenString);
        public static Color ElementDarkBlue => Color.FromArgb("#006098");
        public static Color ElementLightBlue => Color.FromArgb("#00A1DF");
        public static Color ElementDarkGreen => Color.FromArgb("#0A552A");
        public static Color ElementLightGreen => Color.FromArgb("#23A23E");

#if Biology
        public static string DarkMainString => "#004876";
        public static string LightMainString => "#00a0dd";
        public static Color DarkMain => Color.FromArgb("#004876");
        public static Color LightMain => Color.FromArgb("#00a0dd");
#endif
#if Chemistry
        public static string DarkMainString => DarkMainGreenString;
        public static string LightMainString => LightMainGreenString;
        public static Color DarkMain => Color.FromArgb(DarkMainString);
        public static Color LightMain => Color.FromArgb(LightMainString);
#endif
#if Physics
        public static string DarkMainString => "#203f48";
        public static string LightMainString => "#257885";
        public static Color DarkMain => Color.FromArgb(DarkMainString);
        public static Color LightMain => Color.FromArgb(LightMainString);
#endif
    }
}
