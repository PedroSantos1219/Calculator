using System;

namespace Calculator.Engine
{
    // Domain-specific exception so the UI can distinguish a user-visible
    // calculation error (divide by zero, sqrt of negative, overflow) from
    // a real bug. Anything thrown from inside the engine that isn't one of
    // these should bubble up untouched — bugs deserve to be loud.
    public sealed class CalculationException : Exception
    {
        public CalculationException(string message) : base(message) { }

        public CalculationException(string message, Exception inner) : base(message, inner) { }
    }
}
