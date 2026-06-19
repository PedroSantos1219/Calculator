namespace Calculator.Engine
{
    // The four binary operators the calculator can have "pending" between
    // two operands. Kept as a small enum (rather than strings) so the engine
    // never has to validate a free-form operator symbol at runtime.
    public enum BinaryOperator
    {
        None,
        Add,
        Subtract,
        Multiply,
        Divide
    }
}
