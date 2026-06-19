using System;
using System.Windows.Forms;

namespace Calculator
{
    // Top-level shell for the application. Wiring between the UI controls and
    // the calculation engine lives in this file; the engine itself is kept
    // separate so it stays unit-testable without dragging WinForms along.
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
    }
}
