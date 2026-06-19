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

        // Pushes the latest engine state onto the visible controls. Called
        // after every input that could change what the user is looking at.
        // Centralising it removes the temptation to update half the display
        // from one handler and the other half from another.
        private void RefreshDisplay()
        {
            _displayLabel.Text = DisplayFormatter.Format(_engine.Display);
            _memoryIndicator.Visible = _engine.HasMemory;
        }
    }
}
