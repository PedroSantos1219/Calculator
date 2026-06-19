using System.Globalization;

namespace Calculator.Engine
{
    // Formats a decimal for the calculator display. Trims trailing zeros so
    // an integer result shows as "5" rather than "5.000000" (decimal keeps
    // the scale of the operation that produced it), and uses the current
    // culture's group separators so the value reads naturally for the user.
    public static class DisplayFormatter
    {
        public static string Format(decimal value)
        {
            // Normalising via ToString("G29") + parse strips the unused scale
            // bits decimal carries around — G29 is "the most digits decimal
            // can faithfully represent". After this round-trip the value
            // prints without trailing zeros.
            decimal normalised = decimal.Parse(
                value.ToString("G29", CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);

            // "N" gives thousands separators and respects the current culture
            // (so a Portuguese user sees "1 234,5", an English user sees
            // "1,234.5"). Capping decimals at 12 keeps the column width sane
            // without throwing away meaningful precision.
            return normalised.ToString("N12", CultureInfo.CurrentCulture)
                             .TrimEnd('0')
                             .TrimEnd(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray());
        }
    }
}
