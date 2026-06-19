using System;
using System.Collections.Generic;

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

        // Stores the most recent (operator, right-hand operand) pair so
        // pressing "=" repeatedly keeps applying the same step. After
        // 2 + 3 =, hitting "=" again should give 8, then 11, then 14...
        private decimal _repeatOperand;
        private BinaryOperator _repeatOperator;
        private bool _hasRepeat;

        // Single-slot memory register, the same model as M+ / M- / MR / MC
        // on a desk calculator. A separate flag tracks whether something has
        // ever been stored so the UI can dim MR/MC when there's nothing to
        // recall or clear.
        private decimal _memory;
        private bool _hasMemory;

        // Public view of the memory register so the UI can light up the
        // "M" indicator when something is stored.
        public bool HasMemory => _hasMemory;

        // Backing store for the calculation history. Exposed read-only so
        // the UI can data-bind to it without being able to mutate it
        // behind the engine's back.
        private readonly List<HistoryEntry> _history = new();
        public IReadOnlyList<HistoryEntry> History => _history.AsReadOnly();

        public CalculatorEngine()
        {
            _pendingOperator = BinaryOperator.None;
            _overwriteOnNextDigit = true;
        }

        // Value the UI should render right now.
        public decimal Display => _current;

        // True while there's an operator waiting on a right-hand operand.
        public bool HasPendingOperator => _pendingOperator != BinaryOperator.None;

        // Wipes the history list. Separated from Clear() because the user
        // can reasonably want to reset the working calculation without
        // throwing away what they've already computed.
        public void ClearHistory()
        {
            _history.Clear();
        }

        // Resets every piece of state — the "C" button.
        public void Clear()
        {
            _current = 0m;
            _accumulator = 0m;
            _pendingOperator = BinaryOperator.None;
            _repeatOperand = 0m;
            _repeatOperator = BinaryOperator.None;
            _hasRepeat = false;
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
        // operator but with a remembered last step re-applies that step, so
        // 2 + 3 = = = walks through 5, 8, 11.
        public void Equals()
        {
            decimal left;
            decimal right;
            BinaryOperator op;

            if (_pendingOperator != BinaryOperator.None)
            {
                left = _accumulator;
                right = _current;
                op = _pendingOperator;

                _repeatOperand = right;
                _repeatOperator = op;
                _hasRepeat = true;
            }
            else if (_hasRepeat)
            {
                left = _current;
                right = _repeatOperand;
                op = _repeatOperator;
            }
            else
            {
                return;
            }

            decimal result = Evaluate(left, right, op);

            _history.Add(new HistoryEntry(
                $"{DisplayFormatter.Format(left)} {SymbolFor(op)} {DisplayFormatter.Format(right)}",
                result));

            _accumulator = result;
            _current = result;
            _pendingOperator = BinaryOperator.None;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // Renders a binary operator with its on-screen symbol. The crossed
        // ×/÷ glyphs match the buttons better than ASCII */-, and keep the
        // history readable when the user scrolls back through it.
        private static string SymbolFor(BinaryOperator op) => op switch
        {
            BinaryOperator.Add => "+",
            BinaryOperator.Subtract => "−",
            BinaryOperator.Multiply => "×",
            BinaryOperator.Divide => "÷",
            _ => "?"
        };

        // Squares the current entry in place. Result becomes the new entry
        // so it can either feed into a pending operator or stand on its own.
        public void Square()
        {
            try
            {
                _current = checked(_current * _current);
            }
            catch (OverflowException ex)
            {
                throw new CalculationException("Overflow.", ex);
            }

            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // Windows-calculator-style percent. The behaviour is context-aware:
        //
        //   100 + 5 %  →  5     (5% of 100, then ready to be added/subtracted)
        //   100 - 5 %  →  5
        //   100 * 5 %  →  0.05  (treat % as "divide by 100" for mul/div)
        //   100 / 5 %  →  0.05
        //   5 %        →  0     (no context → degenerate case)
        //
        // The asymmetry between add/sub and mul/div mirrors the physical
        // calculator behaviour that users have built muscle memory around.
        public void Percent()
        {
            if (_pendingOperator == BinaryOperator.Add || _pendingOperator == BinaryOperator.Subtract)
            {
                _current = _accumulator * _current / 100m;
            }
            else if (_pendingOperator == BinaryOperator.Multiply || _pendingOperator == BinaryOperator.Divide)
            {
                _current = _current / 100m;
            }
            else
            {
                _current = 0m;
            }

            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // √x. Decimal has no native sqrt so we round-trip through double
        // and accept the precision loss for this single operation. The
        // alternative (a hand-rolled decimal Newton iteration) is not worth
        // the complexity for a four-function calculator.
        public void SquareRoot()
        {
            if (_current < 0m)
            {
                throw new CalculationException("Invalid input.");
            }

            double approximated = Math.Sqrt((double)_current);
            _current = (decimal)approximated;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // 1/x. Division by zero short-circuits to a calculation error so
        // the UI can surface it without crashing the app.
        public void Reciprocal()
        {
            if (_current == 0m)
            {
                throw new CalculationException("Cannot divide by zero.");
            }

            _current = 1m / _current;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // MS — store. Overwrites whatever was in memory with the current
        // entry. Decimal copies by value so this is genuinely independent
        // of subsequent edits to _current.
        public void MemoryStore()
        {
            _memory = _current;
            _hasMemory = true;
        }

        // MR — recall. Pushes the memory value into the entry slot and
        // treats it like a freshly-typed result, so the next digit press
        // overwrites it rather than appending.
        public void MemoryRecall()
        {
            if (!_hasMemory)
            {
                return;
            }

            _current = _memory;
            _overwriteOnNextDigit = true;
            _hasDecimalPoint = false;
            _digitsTyped = 0;
        }

        // MC — clear the memory register entirely.
        public void MemoryClear()
        {
            _memory = 0m;
            _hasMemory = false;
        }

        // M+. Adds the current entry to whatever is in memory (treating an
        // empty memory as 0). The first M+ also lights up HasMemory.
        public void MemoryAdd()
        {
            try
            {
                _memory = checked(_memory + _current);
            }
            catch (OverflowException ex)
            {
                throw new CalculationException("Overflow.", ex);
            }

            _hasMemory = true;
        }

        // M-. Subtracts the current entry from memory.
        public void MemorySubtract()
        {
            try
            {
                _memory = checked(_memory - _current);
            }
            catch (OverflowException ex)
            {
                throw new CalculationException("Overflow.", ex);
            }

            _hasMemory = true;
        }

        // Flips the sign of the current entry. Doesn't commit the value or
        // touch the pending operator — it's a display-only edit, the same
        // way the +/- key behaves on a physical calculator.
        public void ToggleSign()
        {
            _current = -_current;

            // Once a result is on screen, flipping the sign should still
            // produce something the user can keep typing onto, so clear the
            // overwrite flag — otherwise the next digit would wipe the
            // negated value back out.
            _overwriteOnNextDigit = false;
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
            if (op == BinaryOperator.Divide && right == 0m)
            {
                throw new CalculationException("Cannot divide by zero.");
            }

            try
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
            catch (OverflowException ex)
            {
                throw new CalculationException("Overflow.", ex);
            }
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
