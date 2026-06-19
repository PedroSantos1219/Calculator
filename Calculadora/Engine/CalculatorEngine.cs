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

        // Evaluates the pending operation. Pressing "=" with no pending
        // operator is a no-op — there is nothing to commit.
        public void Equals()
        {
            if (_pendingOperator == BinaryOperator.None)
            {
                return;
            }

            decimal result = Evaluate(_accumulator, _current, _pendingOperator);

            _accumulator = result;
            _current = result;
            _pendingOperator = BinaryOperator.None;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // Records a binary operator press. If an operator was already pending
        // and the user typed a right-hand operand, evaluate it first so chained
        // expressions like 2 + 3 * 4 collapse into the running accumulator the
        // way the standard calculator does — left to right, no precedence.
        public void ApplyBinaryOperator(BinaryOperator op)
        {
            if (op == BinaryOperator.None)
            {
                throw new ArgumentException("None is not a valid operator press.", nameof(op));
            }

            if (_pendingOperator != BinaryOperator.None && !_overwriteOnNextDigit)
            {
                _accumulator = Evaluate(_accumulator, _current, _pendingOperator);
                _current = _accumulator;
            }
            else
            {
                _accumulator = _current;
            }

            _pendingOperator = op;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // The actual arithmetic. Kept private so callers go through the state
        // machine — direct evaluation outside that flow would skip the side
        // effects (overwrite flag, history, etc.) and produce inconsistent UI.
        private static decimal Evaluate(decimal left, decimal right, BinaryOperator op)
        {
            return op switch
            {
                BinaryOperator.Add => left + right,
                BinaryOperator.Subtract => left - right,
                BinaryOperator.Multiply => left * right,
                BinaryOperator.Divide => left / right,
                _ => throw new InvalidOperationException($"Unhandled operator: {op}")
            };
        }

        // Strips the right-most digit from the current entry. After "=" or
        // an operator press the display holds a committed result rather
        // than a user-typed value — backspacing into that would silently
        // destroy state, so this is a no-op in that case.
        public void Backspace()
        {
            if (_overwriteOnNextDigit)
            {
                return;
            }

            if (_hasDecimalPoint && CountFractionalDigits(_current) > 0)
            {
                int fractionalDigits = CountFractionalDigits(_current);
                decimal scale = Pow10(fractionalDigits);
                _current = Math.Truncate(_current * scale / 10m) / Pow10(fractionalDigits - 1);

                if (CountFractionalDigits(_current) == 0)
                {
                    // We just rubbed out the last fractional digit. Drop the
                    // decimal point too so a follow-up digit lands in the
                    // ones column instead of re-opening the fraction.
                    _hasDecimalPoint = false;
                }
            }
            else if (_hasDecimalPoint)
            {
                _hasDecimalPoint = false;
            }
            else
            {
                _current = Math.Truncate(_current / 10m);
            }

            if (_digitsTyped > 0)
            {
                _digitsTyped--;
            }
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
