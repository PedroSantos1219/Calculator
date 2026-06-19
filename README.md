# Calculator

A WinForms desktop calculator written in C# (.NET 8). The engine is a
left-to-right state machine modelled on the classic four-function desktop
calculator: operators apply in chained order (no precedence), pressing `=`
repeatedly re-applies the last operation, and the active entry is the only
thing a digit press can mutate.

The form ships with a green/gray theme, a history panel, single-slot memory
register, and full keyboard input.

## Features

- Four binary operations (`+`, `−`, `×`, `÷`) with chained evaluation.
- Unary operations: `√x`, `x²`, `1/x`, `±`, and Windows-style `%`.
- Memory register with `MC`, `MR`, `MS`, `M+`, `M-`.
- History panel: every committed expression lands as a recallable entry —
  double-click to push a past result back into the entry slot.
- Keyboard input: digits (top row and numpad), operators, Enter / `=`,
  Backspace (trim entry), Delete (CE), Escape (C), `.` or `,` for the
  decimal mark.
- Decimal arithmetic throughout — no floating-point rounding except in
  `sqrt`, which round-trips through `double` (the only operation that
  needs to).
- Error messages render inline on the display: divide by zero, square root
  of a negative number, and overflow all surface as a friendly message
  without popping a modal.

## Layout

```
Calculadora.sln
└── Calculadora/
    ├── Calculadora.csproj
    ├── Program.cs
    ├── MainForm.cs                 — UI wiring (button clicks, keyboard)
    ├── MainForm.Designer.cs        — control layout and theming
    ├── Theme.cs                    — green/gray palette constants
    └── Engine/
        ├── BinaryOperator.cs       — Add / Subtract / Multiply / Divide
        ├── CalculationException.cs — surfaced as inline display errors
        ├── CalculatorEngine.cs     — the state machine
        ├── DisplayFormatter.cs     — culture-aware number rendering
        └── HistoryEntry.cs         — record for the history panel
```

The engine is UI-agnostic — `Calculator.Engine` references no WinForms
type, which makes the state machine easy to drive from a unit test
later.

## Running it

```
dotnet run --project Calculadora/Calculadora.csproj
```

Requires the .NET 8 SDK with the Windows Desktop workload installed.
