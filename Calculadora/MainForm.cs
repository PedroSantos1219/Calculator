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
            _equalsButton.Click += (_, _) => CommitEquals();
            _clearButton.Click += (_, _) => { _engine.Clear(); RefreshDisplay(); };
            _clearEntryButton.Click += (_, _) => { _engine.ClearEntry(); RefreshDisplay(); };
            _backspaceButton.Click += (_, _) => { _engine.Backspace(); RefreshDisplay(); };
            WireUnaries();
            WireMemoryButtons();

            _clearHistoryButton.Click += (_, _) =>
            {
                _engine.ClearHistory();
                RefreshDisplay();
            };

            // Double-clicking a history entry recalls its result. Pulling
            // it via the engine's Recall path keeps the rest of the state
            // (overwrite flag, decimal flag, digit count) consistent with
            // a freshly-computed value.
            _historyList.DoubleClick += (_, _) =>
            {
                if (_historyList.SelectedIndex < 0)
                {
                    return;
                }

                var entry = _engine.History[_historyList.SelectedIndex];
                _engine.RecallValue(entry.Result);
                RefreshDisplay();
            };

            this.KeyDown += MainForm_KeyDown;
            _decimalButton.Click += (_, _) =>
            {
                _engine.AppendDecimalPoint();
                RefreshDisplay();
            };
            RefreshDisplay();
        }

        private void CommitEquals()
        {
            try
            {
                _engine.Equals();
            }
            catch (CalculationException ex)
            {
                ShowEngineError(ex);
                return;
            }

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
        // Unary buttons each call a single engine method and route any
        // engine error through ShowEngineError, identical to the binary
        // path but parameterised on an Action so we don't need three
        // copies of the try/catch.
        private void WireUnaries()
        {
            _squareRootButton.Click += (_, _) => RunUnary(_engine.SquareRoot);
            _squareButton.Click += (_, _) => RunUnary(_engine.Square);
            _reciprocalButton.Click += (_, _) => RunUnary(_engine.Reciprocal);
            _signButton.Click += (_, _) => RunUnary(_engine.ToggleSign);
            _percentButton.Click += (_, _) => RunUnary(_engine.Percent);
        }

        private void RunUnary(Action operation)
        {
            try
            {
                operation();
            }
            catch (CalculationException ex)
            {
                ShowEngineError(ex);
                return;
            }

            RefreshDisplay();
        }

        // Memory buttons share the unary error path because M+ and M- can
        // overflow on the addition into the register. MC and MR can't throw
        // but go through the same gate for symmetry.
        private void WireMemoryButtons()
        {
            _memoryStoreButton.Click += (_, _) => RunUnary(_engine.MemoryStore);
            _memoryRecallButton.Click += (_, _) => RunUnary(_engine.MemoryRecall);
            _memoryClearButton.Click += (_, _) => RunUnary(_engine.MemoryClear);
            _memoryAddButton.Click += (_, _) => RunUnary(_engine.MemoryAdd);
            _memorySubtractButton.Click += (_, _) => RunUnary(_engine.MemorySubtract);
        }

        // Global keyboard handler — KeyPreview is on so we see the keystrokes
        // before the focused button does. Digits live here; operator and
        // control keys land in follow-up commits.
        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                _engine.AppendDigit(e.KeyCode - Keys.D0);
                RefreshDisplay();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                _engine.AppendDigit(e.KeyCode - Keys.NumPad0);
                RefreshDisplay();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Accept both ',' and '.' for the decimal mark. Some keyboards
            // send a comma on the numpad's decimal key, others send a dot —
            // the displayed mark follows the current culture regardless.
            if (e.KeyCode == Keys.OemPeriod
                || e.KeyCode == Keys.Decimal
                || e.KeyCode == Keys.OemComma)
            {
                _engine.AppendDecimalPoint();
                RefreshDisplay();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
        }

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
            RefreshHistoryList();
        }

        // Reflects the engine's history list onto the ListBox. We rebuild
        // from scratch instead of diffing — the list is short, the rebuild
        // cost is negligible, and any other approach is one place where
        // the UI and engine can drift apart silently.
        private void RefreshHistoryList()
        {
            _historyList.BeginUpdate();
            try
            {
                _historyList.Items.Clear();
                foreach (var entry in _engine.History)
                {
                    _historyList.Items.Add($"{entry.Expression} = {DisplayFormatter.Format(entry.Result)}");
                }

                if (_historyList.Items.Count > 0)
                {
                    // Keep the latest entry in view — feels more like a
                    // running log than a static list when results stream in.
                    _historyList.TopIndex = _historyList.Items.Count - 1;
                }
            }
            finally
            {
                _historyList.EndUpdate();
            }
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
