namespace Calculator.Engine
{
    // A single completed calculation. Kept as a record so two entries with
    // the same Expression and Result compare equal — useful when a future
    // commit collapses duplicate consecutive entries.
    //
    // Expression holds the human-readable left-hand side ("12 + 7") and
    // Result holds the value that "=" produced. Splitting them lets the
    // history panel show both columns without the UI parsing strings.
    public sealed record HistoryEntry(string Expression, decimal Result);
}
