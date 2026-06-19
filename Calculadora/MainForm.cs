using System;
using System.Windows.Forms;
using Calculator.Engine;

namespace Calculator
{
    // Top-level shell for the application. Wires the UI controls (declared
    // in MainForm.Designer.cs) to the calculation engine. The engine itself
    // is in Calculator.Engine and stays UI-agnostic — this file is the only
    // place WinForms talks to it.
    public partial class MainForm : Form
    {
        // One engine instance lives for the lifetime of the form. It owns
        // all calculator state (current entry, accumulator, memory,
        // history) so the form stays a thin presenter.
        private readonly CalculatorEngine _engine = new();

        public MainForm()
        {
            InitializeComponent();
            WireDigitButtons();
            WireBinaryOperators();
            _decimalButton.Click += (_, _) =>
            {
                _engine.AppendDecimalPoint();
                RefreshDisplay();
            };
            RefreshDisplay();
        }

        // Attach the same click handler to all ten digit buttons. Capturing
        // the digit value in the closure is cleaner than a giant switch on
        // sender — there are only ten of them, and the loop body makes the
        // mapping obvious at a glance.
        private void WireDigitButtons()
        {
            for (int digit = 0; digit <= 9; digit++)
            {
                int captured = digit;
                _digitButtons[digit].Click += (_, _) =>
                {
                    _engine.AppendDigit(captured);
                    RefreshDisplay();
                };
            }
        }

        // Binary operator buttons all funnel through a single handler so
        // the error-handling and refresh logic stays in one place.
        private void WireBinaryOperators()
        {
            _addButton.Click += (_, _) => ApplyOperator(BinaryOperator.Add);
            _subtractButton.Click += (_, _) => ApplyOperator(BinaryOperator.Subtract);
            _multiplyButton.Click += (_, _) => ApplyOperator(BinaryOperator.Multiply);
            _divideButton.Click += (_, _) => ApplyOperator(BinaryOperator.Divide);
        }

        private void ApplyOperator(BinaryOperator op)
        {
            try
            {
                _engine.ApplyBinaryOperator(op);
            }
            catch (CalculationException ex)
            {
                ShowEngineError(ex);
                return;
            }

            RefreshDisplay();
        }

        // Renders an engine error on the display and resets the engine so
        // the next press starts cleanly. Keeping the dialog out of this
        // path (the message lands inline in the display) means a stream of
        // bad inputs doesn't bury the user under modal popups.
        private void ShowEngineError(CalculationException error)
        {
            _displayLabel.Text = error.Message;
            _expressionLabel.Text = string.Empty;
            _engine.Clear();
        }

        // Pushes the latest engine state onto the visible controls. Called
        // after every input that could change what the user is looking at.
        // Centralising it removes the temptation to update half the display
        // from one handler and the other half from another.
        private void RefreshDisplay()
        {
            _displayLabel.Text = DisplayFormatter.Format(_engine.Display);
            _memoryIndicator.Visible = _engine.HasMemory;
            _expressionLabel.Text = BuildExpressionPreview();
        }

        // Builds the strip above the main display. While an operator is
        // pending we show "{accumulator} {symbol}" — once the user finishes
        // typing the right-hand operand and presses '=' the preview clears
        // (the history list holds the full completed expression instead).
        private string BuildExpressionPreview()
        {
            if (!_engine.HasPendingOperator)
            {
                return string.Empty;
            }

            string left = DisplayFormatter.Format(_engine.Accumulator);
            string symbol = CalculatorEngine.SymbolFor(_engine.PendingOperator);
            return $"{left} {symbol}";
        }
    }
}
