using System.Drawing;

namespace Calculator
{
    // Central palette for the green/gray visual language. Pulled out of the
    // designer so a future re-theme touches one file instead of every
    // button setup block. Colors are picked from a single hue ramp (cool
    // grays) with two green accents — a muted forest for operators and a
    // brighter shade for the equals key.
    internal static class Theme
    {
        // Surfaces — backgrounds and the display panel.
        public static readonly Color FormBackground = Color.FromArgb(42, 49, 56);
        public static readonly Color DisplayBackground = Color.FromArgb(30, 37, 43);
        public static readonly Color HistoryBackground = Color.FromArgb(36, 43, 50);

        // Text colors. PrimaryText is the on-display readout, MutedText is
        // the expression preview / labels that should sit behind the value.
        public static readonly Color PrimaryText = Color.FromArgb(241, 245, 249);
        public static readonly Color MutedText = Color.FromArgb(148, 163, 184);

        // Buttons.
        public static readonly Color DigitButton = Color.FromArgb(74, 85, 94);
        public static readonly Color FunctionButton = Color.FromArgb(58, 67, 74);
        public static readonly Color MemoryButton = Color.FromArgb(58, 67, 74);
        public static readonly Color OperatorButton = Color.FromArgb(22, 163, 74);
        public static readonly Color EqualsButton = Color.FromArgb(34, 197, 94);

        // Border drawn around flat buttons. Almost transparent so it reads
        // as a subtle separator rather than a hard outline.
        public static readonly Color ButtonBorder = Color.FromArgb(28, 33, 38);

        // Hover and press tints. Same hue as the resting color, shifted
        // a touch lighter / darker.
        public static readonly Color DigitButtonHover = Color.FromArgb(94, 105, 114);
        public static readonly Color OperatorButtonHover = Color.FromArgb(34, 197, 94);
        public static readonly Color EqualsButtonHover = Color.FromArgb(74, 222, 128);
    }
}
