using System.Collections.Generic;
using Common;
using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls.ChemicalElements
{
    public static class ChemicalElementsData
    {
        public static Dictionary<string, IElementViewModel> ChemicalElements = new Dictionary<string, IElementViewModel>
        {
            {"H", new ElementViewModel { Name = "Hydrogen", X = 0, Y = 0, AtomicNumber = 1, ShortName = "H", MassNumber = 1.008f, BackgroundColor = Colors.White, FontColor = CustomColors.ElementDarkBlue, IsGroupFirst = true, IsPeriodFirsr = true, BlockName = StringResource.SBlockString}},
            {"Li", new ElementViewModel { Name = "Lithium", X=0, Y=1, AtomicNumber = 3, ShortName = "Li", MassNumber = 6.94f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White, IsPeriodFirsr = true}},
            {"Na", new ElementViewModel { Name = "Sodium", X=0, Y=2, AtomicNumber = 11, ShortName = "Na", MassNumber = 22.990f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White, IsPeriodFirsr = true}},
            {"K", new ElementViewModel { Name = "Potassium", X=0, Y=3, AtomicNumber = 19, ShortName = "K", MassNumber = 39.098f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White, IsPeriodFirsr = true}},
            {"Rb", new ElementViewModel { Name = "Rubidium", X=0, Y=4, AtomicNumber = 37, ShortName = "Rb", MassNumber = 85.4678f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White, IsPeriodFirsr = true}},
            {"Cs", new ElementViewModel { Name = "Caesium", X=0, Y=5, AtomicNumber = 55, ShortName = "Cs", MassNumber = 132.905f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White, IsPeriodFirsr = true}},
            {"Fr", new ElementViewModel { Name = "Francium", X=0, Y=6, AtomicNumber = 87, ShortName = "Fr", MassNumber = 223f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White, IsPeriodFirsr = true}},

            {"Be", new ElementViewModel { Name = "Beryllium", X=1, Y=1, AtomicNumber = 4, ShortName = "Be", MassNumber = 9.012f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White, IsGroupFirst = true}},
            {"Mg", new ElementViewModel { Name = "Magnesium", X=1, Y=2, AtomicNumber = 12, ShortName = "Mg", MassNumber = 24.305f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White}},
            {"Ca", new ElementViewModel { Name = "Calcium", X=1, Y=3, AtomicNumber = 20, ShortName = "Ca", MassNumber = 40.078f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White}},
            {"Sr", new ElementViewModel { Name = "Strontium", X=1, Y=4, AtomicNumber = 38, ShortName = "Sr", MassNumber = 87.62f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White}},
            {"Ba", new ElementViewModel { Name = "Barium", X=1, Y=5, AtomicNumber = 56, ShortName = "Ba", MassNumber = 137.327f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White}},
            {"Ra", new ElementViewModel { Name = "Radium", X=1, Y=6, AtomicNumber = 88, ShortName = "Ra", MassNumber = 226f, BackgroundColor = CustomColors.ElementDarkBlue, FontColor = Colors.White}},

            {"Sc", new ElementViewModel { Name = "Scandium", X=2, Y=3, AtomicNumber = 21, ShortName = "Sc", MassNumber = 44.955908f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true, BlockName = StringResource.DBlockString}},
            {"Y", new ElementViewModel { Name = "Yttrium", X=2, Y=4, AtomicNumber = 39, ShortName = "Y", MassNumber = 88.90584f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"La", new ElementViewModel { Name = "Lanthanum", X=2, Y=5, AtomicNumber = 57, ShortName = "La", MassNumber = 138.91f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Ac", new ElementViewModel { Name = "Actinium", X=2, Y=6, AtomicNumber = 89, ShortName = "Ac", MassNumber = 1.008f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Ti", new ElementViewModel { Name = "Titanium", X=3, Y=3, AtomicNumber = 22, ShortName = "Ti", MassNumber = 47.867f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Zr", new ElementViewModel { Name = "Zirconium", X=3, Y=4, AtomicNumber = 40, ShortName = "Zr", MassNumber = 91.224f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Hf", new ElementViewModel { Name = "Hafnium", X=3, Y=5, AtomicNumber = 72, ShortName = "Hf", MassNumber = 178.49f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Rf", new ElementViewModel { Name = "Rutherfordium", X=3, Y=6, AtomicNumber = 104, ShortName = "Rf", MassNumber = 267f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"V", new ElementViewModel { Name = "Vanadium", X=4, Y=3, AtomicNumber = 23, ShortName = "V", MassNumber = 50.9415f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Nb", new ElementViewModel { Name = "Niobium", X=4, Y=4, AtomicNumber = 41, ShortName = "Nb", MassNumber = 92.90637f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Ta", new ElementViewModel { Name = "Tantalum", X=4, Y=5, AtomicNumber = 73, ShortName = "Ta", MassNumber = 180.94788f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Db", new ElementViewModel { Name = "Dubnium", X=4, Y=6, AtomicNumber = 105, ShortName = "Db", MassNumber = 268f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Cr", new ElementViewModel { Name = "Chromium", X=5, Y=3, AtomicNumber = 24, ShortName = "Cr", MassNumber = 51.9961f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Mo", new ElementViewModel { Name = "Molybdenum", X=5, Y=4, AtomicNumber = 42, ShortName = "Mo", MassNumber = 95.95f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"W", new ElementViewModel { Name = "Tungsten", X=5, Y=5, AtomicNumber = 74, ShortName = "W", MassNumber = 183.84f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Sg", new ElementViewModel { Name = "Seaborgium", X=5, Y=6, AtomicNumber = 106, ShortName = "Sg", MassNumber = 269f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Mn", new ElementViewModel { Name = "Manganese", X=6, Y=3, AtomicNumber = 25, ShortName = "Mn", MassNumber = 54.938044f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Tc", new ElementViewModel { Name = "Technetium", X=6, Y=4, AtomicNumber = 43, ShortName = "Tc", MassNumber = 98f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Re", new ElementViewModel { Name = "Rhenium", X=6, Y=5, AtomicNumber = 75, ShortName = "Re", MassNumber = 186.207f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Bh", new ElementViewModel { Name = "Bohrium", X=6, Y=6, AtomicNumber = 107, ShortName = "Bh", MassNumber = 270f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Fe", new ElementViewModel { Name = "Iron", X=7, Y=3, AtomicNumber = 26, ShortName = "Fe", MassNumber = 55.845f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Ru", new ElementViewModel { Name = "Ruthenium", X=7, Y=4, AtomicNumber = 44, ShortName = "Ru", MassNumber = 101.07f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Os", new ElementViewModel { Name = "Osmium", X=7, Y=5, AtomicNumber = 76, ShortName = "Os", MassNumber = 190.23f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Hs", new ElementViewModel { Name = "Hassium", X=7, Y=6, AtomicNumber = 108, ShortName = "Hs", MassNumber = 277f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Co", new ElementViewModel { Name = "Cobalt", X=8, Y=3, AtomicNumber = 27, ShortName = "Co", MassNumber = 58.933194f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Rh", new ElementViewModel { Name = "Rhodium", X=8, Y=4, AtomicNumber = 45, ShortName = "Rh", MassNumber = 102.90550f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Ir", new ElementViewModel { Name = "Iridium", X=8, Y=5, AtomicNumber = 77, ShortName = "Ir", MassNumber = 192.217f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Mt", new ElementViewModel { Name = "Meitnerium", X=8, Y=6, AtomicNumber = 109, ShortName = "Mt", MassNumber = 278f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Ni", new ElementViewModel { Name = "Nickel", X=9, Y=3, AtomicNumber = 28, ShortName = "Ni", MassNumber = 58.6934f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Pd", new ElementViewModel { Name = "Palladium", X=9, Y=4, AtomicNumber = 46, ShortName = "Pd", MassNumber = 106.42f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Pt", new ElementViewModel { Name = "Platinum", X=9, Y=5, AtomicNumber = 78, ShortName = "Pt", MassNumber = 195.084f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Ds", new ElementViewModel { Name = "Darmstadtium", X=9, Y=6, AtomicNumber = 110, ShortName = "Ds", MassNumber = 281f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Cu", new ElementViewModel { Name = "Copper", X=10, Y=3, AtomicNumber = 29, ShortName = "Cu", MassNumber = 63.546f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Ag", new ElementViewModel { Name = "Silver", X=10, Y=4, AtomicNumber = 47, ShortName = "Ag", MassNumber = 107.8682f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Au", new ElementViewModel { Name = "Gold", X=10, Y=5, AtomicNumber = 79, ShortName = "Au", MassNumber = 196.966569f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Rg", new ElementViewModel { Name = "Roentgenium", X=10, Y=6, AtomicNumber = 111, ShortName = "Rg", MassNumber = 282f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"Zn", new ElementViewModel { Name = "Zinc", X=11, Y=3, AtomicNumber = 30, ShortName = "Zn", MassNumber = 65.38f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Cd", new ElementViewModel { Name = "Cadmium", X=11, Y=4, AtomicNumber = 48, ShortName = "Cd", MassNumber = 112.414f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Hg", new ElementViewModel { Name = "Mercury", X=11, Y=5, AtomicNumber = 80, ShortName = "Hg", MassNumber = 200.592f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},
            {"Cn", new ElementViewModel { Name = "Copernicium", X=11, Y=6, AtomicNumber = 112, ShortName = "Cn", MassNumber = 285f, BackgroundColor = CustomColors.ElementLightGreen, FontColor = Colors.White}},

            {"B", new ElementViewModel { Name = "Boron", X=12, Y=1, AtomicNumber = 5, ShortName = "B", MassNumber = 10.81f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White, IsGroupFirst = true, BlockName = StringResource.PBlockString}},
            {"Al", new ElementViewModel { Name = "Aluminium", X=12, Y=2, AtomicNumber = 13, ShortName = "Al", MassNumber = 26.9815385f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Ga", new ElementViewModel { Name = "Gallium", X=12, Y=3, AtomicNumber = 31, ShortName = "Ga", MassNumber = 69.723f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"In", new ElementViewModel { Name = "Indium", X=12, Y=4, AtomicNumber = 49, ShortName = "In", MassNumber = 114.818f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Tl", new ElementViewModel { Name = "Thallium", X=12, Y=5, AtomicNumber = 81, ShortName = "Tl", MassNumber = 204.38f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Nh", new ElementViewModel { Name = "Nihonium", X=12, Y=6, AtomicNumber = 113, ShortName = "Nh", MassNumber = 286f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},

            {"C", new ElementViewModel { Name = "Carbon", X=13, Y=1, AtomicNumber = 6, ShortName = "C", MassNumber = 12.011f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Si", new ElementViewModel { Name = "Silicon", X=13, Y=2, AtomicNumber = 14, ShortName = "Si", MassNumber = 28.085f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Ge", new ElementViewModel { Name = "Germanium", X=13, Y=3, AtomicNumber = 32, ShortName = "Ge", MassNumber = 72.630f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Sn", new ElementViewModel { Name = "Tin", X=13, Y=4, AtomicNumber = 50, ShortName = "Sn", MassNumber = 118.710f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Pb", new ElementViewModel { Name = "Lead", X=13, Y=5, AtomicNumber = 82, ShortName = "Pb", MassNumber = 207.2f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Fl", new ElementViewModel { Name = "Flerovium", X=13, Y=6, AtomicNumber = 114, ShortName = "Fl", MassNumber = 289f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},

            {"N", new ElementViewModel { Name = "Nitrogen", X=14, Y=1, AtomicNumber = 7, ShortName = "N", MassNumber = 14.007f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"P", new ElementViewModel { Name = "Phosphorus", X=14, Y=2, AtomicNumber = 15, ShortName = "P", MassNumber = 30.973761998f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"As", new ElementViewModel { Name = "Arsenic", X=14, Y=3, AtomicNumber = 33, ShortName = "As", MassNumber = 74.921595f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Sb", new ElementViewModel { Name = "Antimony", X=14, Y=4, AtomicNumber = 51, ShortName = "Sb", MassNumber = 121.760f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Bi", new ElementViewModel { Name = "Bismuth", X=14, Y=5, AtomicNumber = 83, ShortName = "Bi", MassNumber = 208.98040f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Mc", new ElementViewModel { Name = "Moscovium", X=14, Y=6, AtomicNumber = 115, ShortName = "Mc", MassNumber = 290f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},

            {"O", new ElementViewModel { Name = "Oxygen", X=15, Y=1, AtomicNumber = 8, ShortName = "O", MassNumber = 15.999f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"S", new ElementViewModel { Name = "Sulfur", X=15, Y=2, AtomicNumber = 16, ShortName = "S", MassNumber = 32.06f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Se", new ElementViewModel { Name = "Selenium", X=15, Y=3, AtomicNumber = 34, ShortName = "Se", MassNumber = 78.971f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Te", new ElementViewModel { Name = "Tellurium", X=15, Y=4, AtomicNumber = 52, ShortName = "Te", MassNumber = 127.60f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Po", new ElementViewModel { Name = "Polonium", X=15, Y=5, AtomicNumber = 84, ShortName = "Po", MassNumber = 209f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Lv", new ElementViewModel { Name = "Livermorium", X=15, Y=6, AtomicNumber = 116, ShortName = "Lv", MassNumber = 293f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},

            {"F", new ElementViewModel { Name = "Fluorine", X=16, Y=1, AtomicNumber = 9, ShortName = "F", MassNumber = 18.998403163f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White, IsGroupFirst = true}},
            {"Cl", new ElementViewModel { Name = "Chlorine", X=16, Y=2, AtomicNumber = 17, ShortName = "Cl", MassNumber = 35.45f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Br", new ElementViewModel { Name = "Bromine", X=16, Y=3, AtomicNumber = 35, ShortName = "Br", MassNumber = 79.904f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"I", new ElementViewModel { Name = "Iodine", X=16, Y=4, AtomicNumber = 53, ShortName = "I", MassNumber = 126.9044f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"At", new ElementViewModel { Name = "Astatine", X=16, Y=5, AtomicNumber = 85, ShortName = "At", MassNumber = 210f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Ts", new ElementViewModel { Name = "Tennessine", X=16, Y=6, AtomicNumber = 117, ShortName = "Ts", MassNumber = 294f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},

            {"He", new ElementViewModel { Name = "Helium", X=17, Y=0, AtomicNumber = 2, ShortName = "He", MassNumber = 4.002602f, BackgroundColor = Colors.White, FontColor = CustomColors.ElementDarkGreen, IsGroupFirst = true}},
            {"Ne", new ElementViewModel { Name = "Neon", X=17, Y=1, AtomicNumber = 10, ShortName = "Ne", MassNumber = 20.1797f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Ar", new ElementViewModel { Name = "Argon", X=17, Y=2, AtomicNumber = 18, ShortName = "Ar", MassNumber = 39.948f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Kr", new ElementViewModel { Name = "Krypton", X=17, Y=3, AtomicNumber = 36, ShortName = "Kr", MassNumber = 83.798f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Xe", new ElementViewModel { Name = "Xenon", X=17, Y=4, AtomicNumber = 54, ShortName = "Xe", MassNumber = 131.293f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Rn", new ElementViewModel { Name = "Radon", X=17, Y=5, AtomicNumber = 86, ShortName = "Rn", MassNumber = 222f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},
            {"Og", new ElementViewModel { Name = "Oganesson", X=17, Y=6, AtomicNumber = 118, ShortName = "Og", MassNumber = 294f, BackgroundColor = CustomColors.ElementDarkGreen, FontColor = Colors.White}},

            // Lanthanides and Actinides
            {"Ce", new ElementViewModel { Name = "Cerium", X=2, Y=7, AtomicNumber = 58, ShortName = "Ce", MassNumber = 140.12f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White, BlockName = StringResource.FBlockString}},
            {"Th", new ElementViewModel { Name = "Thorium", X=2, Y=8, AtomicNumber = 90, ShortName = "Th", MassNumber = 232.04f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Pr", new ElementViewModel { Name = "Praseodymium", X=3, Y=7, AtomicNumber = 59, ShortName = "Pr", MassNumber = 140.91f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Pa", new ElementViewModel { Name = "Protactinium", X=3, Y=8, AtomicNumber = 91, ShortName = "Pa", MassNumber = 231.04f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Nd", new ElementViewModel { Name = "Neodymium", X=4, Y=7, AtomicNumber = 60, ShortName = "Nd", MassNumber = 144.24f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"U", new ElementViewModel { Name = "Uranium", X=4, Y=8, AtomicNumber = 92, ShortName = "U", MassNumber = 238.03f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Pm", new ElementViewModel { Name = "Promethium", X=5, Y=7, AtomicNumber = 61, ShortName = "Pm", MassNumber = 145f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Np", new ElementViewModel { Name = "Neptunium", X=5, Y=8, AtomicNumber = 93, ShortName = "Pu", MassNumber = 237f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Sm", new ElementViewModel { Name = "Samarium", X=6, Y=7, AtomicNumber = 62, ShortName = "Sm", MassNumber = 150.36f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Pu", new ElementViewModel { Name = "Plutonium", X=6, Y=8, AtomicNumber = 94, ShortName = "Pu", MassNumber = 244f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Eu", new ElementViewModel { Name = "Europium", X=7, Y=7, AtomicNumber = 63, ShortName = "Eu", MassNumber = 151.96f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Am", new ElementViewModel { Name = "Americium", X=7, Y=8, AtomicNumber = 95, ShortName = "Am", MassNumber = 243f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Gd", new ElementViewModel { Name = "Gadolinium", X=8, Y=7, AtomicNumber = 64, ShortName = "Gd", MassNumber = 157.25f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Cm", new ElementViewModel { Name = "Curium", X=8, Y=8, AtomicNumber = 96, ShortName = "Cm", MassNumber = 247f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Tb", new ElementViewModel { Name = "Terbium", X=9, Y=7, AtomicNumber = 65, ShortName = "Tb", MassNumber = 158.93f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Bk", new ElementViewModel { Name = "Berkelium", X=9, Y=8, AtomicNumber = 97, ShortName = "Bk", MassNumber = 247f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Dy", new ElementViewModel { Name = "Dysprosium", X=10, Y=7, AtomicNumber = 66, ShortName = "Dy", MassNumber = 162.50f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Cf", new ElementViewModel { Name = "Californium", X=10, Y=8, AtomicNumber = 98, ShortName = "Cf", MassNumber = 251f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Ho", new ElementViewModel { Name = "Holmium", X=11, Y=7, AtomicNumber = 67, ShortName = "Ho", MassNumber = 164.93f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Es", new ElementViewModel { Name = "Einsteinium", X=11, Y=8, AtomicNumber = 99, ShortName = "Es", MassNumber = 252f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Er", new ElementViewModel { Name = "Erbium", X=12, Y=7, AtomicNumber = 68, ShortName = "Er", MassNumber = 167.28f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Fm", new ElementViewModel { Name = "Fermium", X=12, Y=8, AtomicNumber = 100, ShortName = "Fm", MassNumber = 257f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Tm", new ElementViewModel { Name = "Thulium", X=13, Y=7, AtomicNumber = 69, ShortName = "Tm", MassNumber = 168.93f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Md", new ElementViewModel { Name = "Mendelevium", X=13, Y=8, AtomicNumber = 101, ShortName = "Md", MassNumber = 258f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Yb", new ElementViewModel { Name = "Ytterbium", X=14, Y=7, AtomicNumber = 70, ShortName = "Yb", MassNumber = 173.05f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"No", new ElementViewModel { Name = "Nobelium", X=14, Y=8, AtomicNumber = 102, ShortName = "No", MassNumber = 259f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Lu", new ElementViewModel { Name = "Lutenium", X=15, Y=7, AtomicNumber = 71, ShortName = "Lu", MassNumber = 174.97f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
            {"Lr", new ElementViewModel { Name = "Lawrencium", X=15, Y=8, AtomicNumber = 103, ShortName = "Lr", MassNumber = 266f, BackgroundColor = CustomColors.ElementLightBlue, FontColor = Colors.White}},
        };
    }
}
