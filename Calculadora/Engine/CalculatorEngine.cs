using System;

namespace Calculator.Engine
{
    // Stateful calculator core. Models the same "running accumulator" semantics
    // used by the standard Windows desktop calculator: operators evaluate
    // left-to-right (no precedence), pressing "=" repeatedly re-applies the
    // last operator with the last operand, and a fresh digit press after "="
    // starts a new computation.
    //
    // Numbers are kept as decimal everywhere they can be. It dodges the
    // 0.1 + 0.2 = 0.30000000000000004 trap users get angry about. Operations
    // that genuinely need real-number math (only square root today) round-trip
    // through double; the precision loss there is documented at the call site.
    public sealed class CalculatorEngine
    {
        // Anything past this many digits won't fit nicely on the display and
        // is almost certainly the result of holding a key down. Cap once at
        // the engine level so every input path benefits.
        private const int MaxDigitsAllowed = 16;

        // Operand the user is currently editing (or the latest result).
        private decimal _current;

        // Last committed value — the left-hand side of any pending operation.
        private decimal _accumulator;

        // What we'll apply when the next "=" or operator comes in.
        private BinaryOperator _pendingOperator;

        // After "=" or after an operator key the next digit press should
        // overwrite the display rather than append to it.
        private bool _overwriteOnNextDigit;

        // Tracks whether the current entry already contains a decimal point
        // so we can ignore a second press without scanning the value.
        private bool _hasDecimalPoint;

        // How many digits the user has actually typed for the current entry.
        // Used both to enforce MaxDigitsAllowed and to know where the next
        // fractional digit lands.
        private int _digitsTyped;

        public CalculatorEngine()
        {
            _pendingOperator = BinaryOperator.None;
            _overwriteOnNextDigit = true;
        }

        // Value the UI should render right now.
        public decimal Display => _current;

        // True while there's an operator waiting on a right-hand operand.
        public bool HasPendingOperator => _pendingOperator != BinaryOperator.None;

        // Resets every piece of state — the "C" button.
        public void Clear()
        {
            _current = 0m;
            _accumulator = 0m;
            _pendingOperator = BinaryOperator.None;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // Clears only the value the user is typing — the "CE" button. Leaves
        // the pending operator and accumulator intact so 5 + CE 7 = still
        // yields 12.
        public void ClearEntry()
        {
            _current = 0m;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // Appends a digit (0-9) to whatever the user is typing. Pressing a
        // digit right after "=" or an operator starts a fresh number rather
        // than tacking onto the previous result, which would surprise anyone.
        public void AppendDigit(int digit)
        {
            if (digit < 0 || digit > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(digit));
            }

            if (_overwriteOnNextDigit)
            {
                _current = 0m;
                _hasDecimalPoint = false;
                _digitsTyped = 0;
                _overwriteOnNextDigit = false;
            }

            if (_digitsTyped >= MaxDigitsAllowed)
            {
                return;
            }

            if (_hasDecimalPoint)
            {
                // The new digit lands in the next fractional slot. Build the
                // increment via decimal arithmetic so we don't lose precision
                // by round-tripping through string.
                int fractionalDigits = CountFractionalDigits(_current) + 1;
                decimal scale = Pow10(fractionalDigits);
                decimal sign = _current < 0m ? -1m : 1m;
                _current += sign * digit / scale;
            }
            else
            {
                _current = _current * 10m + (_current < 0m ? -digit : digit);
            }

            _digitsTyped++;
        }

        // Turns the decimal point on for the current entry. A second press
        // is a no-op — we track the flag explicitly so we don't have to scan
        // the value's string form on every keystroke.
        public void AppendDecimalPoint()
        {
            if (_overwriteOnNextDigit)
            {
                _current = 0m;
                _digitsTyped = 0;
                _overwriteOnNextDigit = false;
            }

            _hasDecimalPoint = true;
        }

        // Counts decimal digits to the right of the point. Decimal's Scale
        // property returns it directly but includes trailing zeros from the
        // representation, which is exactly what we want when picking the
        // next fractional slot.
        private static int CountFractionalDigits(decimal value)
        {
            return (decimal.GetBits(value)[3] >> 16) & 0x7F;
        }

        // Tiny helper: 10^n as a decimal. Loop instead of Math.Pow because
        // Math.Pow rounds and would feed precision loss back into _current.
        private static decimal Pow10(int exponent)
        {
            decimal result = 1m;
            for (int i = 0; i < exponent; i++)
            {
                result *= 10m;
            }
            return result;
        }
    }
}
