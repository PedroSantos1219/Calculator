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

        public CalculatorEngine()
        {
            _pendingOperator = BinaryOperator.None;
            _overwriteOnNextDigit = true;
        }

        // Value the UI should render right now.
        public decimal Display => _current;

        // True while there's an operator waiting on a right-hand operand.
        public bool HasPendingOperator => _pendingOperator != BinaryOperator.None;
    }
}
