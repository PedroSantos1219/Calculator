namespace Calculator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label _signatureLabel = null!;
        private System.Windows.Forms.Panel _displayPanel = null!;
        private System.Windows.Forms.Label _displayLabel = null!;
        private System.Windows.Forms.Label _expressionLabel = null!;
        private System.Windows.Forms.Label _memoryIndicator = null!;
        private System.Windows.Forms.Button _memoryClearButton = null!;
        private System.Windows.Forms.Button _memoryRecallButton = null!;
        private System.Windows.Forms.Button _memoryStoreButton = null!;
        private System.Windows.Forms.Button _memoryAddButton = null!;
        private System.Windows.Forms.Button _memorySubtractButton = null!;

        private System.Windows.Forms.Button _percentButton = null!;
        private System.Windows.Forms.Button _squareRootButton = null!;
        private System.Windows.Forms.Button _squareButton = null!;
        private System.Windows.Forms.Button _reciprocalButton = null!;

        private System.Windows.Forms.Button _clearEntryButton = null!;
        private System.Windows.Forms.Button _clearButton = null!;
        private System.Windows.Forms.Button _backspaceButton = null!;
        private System.Windows.Forms.Button _divideButton = null!;

        private System.Windows.Forms.Button[] _digitButtons = new System.Windows.Forms.Button[10];
        private System.Windows.Forms.Button _multiplyButton = null!;
        private System.Windows.Forms.Button _subtractButton = null!;
        private System.Windows.Forms.Button _addButton = null!;
        private System.Windows.Forms.Button _signButton = null!;
        private System.Windows.Forms.Button _decimalButton = null!;
        private System.Windows.Forms.Button _equalsButton = null!;

        private System.Windows.Forms.Label _historyTitleLabel = null!;
        private System.Windows.Forms.ListBox _historyList = null!;
        private System.Windows.Forms.Button _clearHistoryButton = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            // Layout sizing — the body holds the header + display + button
            // grid in a single column; the history panel docks to the right
            // so users on a narrow window can resize and still get the core
            // calculator at full height.
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 620);
            this.MinimumSize = new System.Drawing.Size(640, 620);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.BackColor = Theme.FormBackground;
            this.Text = "Calculator";

            // Signature strip across the top. Kept as a single label rather
            // than baked into the title bar so the credit is visible no
            // matter how the window chrome is themed by the OS.
            this._signatureLabel = new System.Windows.Forms.Label();
            this._signatureLabel.Text = "Made by Pedro Santos";
            this._signatureLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this._signatureLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._signatureLabel.Location = new System.Drawing.Point(0, 8);
            this._signatureLabel.Size = new System.Drawing.Size(640, 20);
            this._signatureLabel.Anchor = System.Windows.Forms.AnchorStyles.Top
                                        | System.Windows.Forms.AnchorStyles.Left
                                        | System.Windows.Forms.AnchorStyles.Right;
            this._signatureLabel.ForeColor = Theme.MutedText;
            this.Controls.Add(this._signatureLabel);

            // Display panel — wraps the value and (later) the expression
            // preview so we can paint a single background colour behind both.
            this._displayPanel = new System.Windows.Forms.Panel();
            this._displayPanel.Location = new System.Drawing.Point(10, 32);
            this._displayPanel.Size = new System.Drawing.Size(370, 110);
            this._displayPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._displayPanel.BackColor = Theme.DisplayBackground;
            this.Controls.Add(this._displayPanel);

            // Main display — the big right-aligned readout. Mono-derived
            // Segoe UI Semibold makes digits the same width without losing
            // the typographic warmth of the proportional default.
            this._displayLabel = new System.Windows.Forms.Label();
            this._displayLabel.Text = "0";
            this._displayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._displayLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 32F, System.Drawing.FontStyle.Bold);
            this._displayLabel.Location = new System.Drawing.Point(8, 36);
            this._displayLabel.Size = new System.Drawing.Size(354, 66);
            this._displayLabel.AutoEllipsis = true;
            this._displayLabel.ForeColor = Theme.PrimaryText;
            this._displayPanel.Controls.Add(this._displayLabel);

            // Expression preview — the small grey strip that shows what the
            // engine is about to evaluate (e.g. "12 +"). Lives above the main
            // display so the eye drops from intent to result.
            this._expressionLabel = new System.Windows.Forms.Label();
            this._expressionLabel.Text = string.Empty;
            this._expressionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._expressionLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this._expressionLabel.Location = new System.Drawing.Point(8, 6);
            this._expressionLabel.Size = new System.Drawing.Size(354, 28);
            this._expressionLabel.AutoEllipsis = true;
            this._displayPanel.Controls.Add(this._expressionLabel);

            // Memory indicator — tiny "M" badge in the top-left. Hidden by
            // default; the wiring layer makes it visible once HasMemory.
            this._memoryIndicator = new System.Windows.Forms.Label();
            this._memoryIndicator.Text = "M";
            this._memoryIndicator.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this._memoryIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._memoryIndicator.Location = new System.Drawing.Point(8, 8);
            this._memoryIndicator.Size = new System.Drawing.Size(22, 22);
            this._memoryIndicator.Visible = false;
            this._displayPanel.Controls.Add(this._memoryIndicator);

            // Memory toolbar — five short, slim buttons that sit between the
            // display and the main grid. Visually distinct from the main
            // grid (shorter height, lighter weight) so the eye skips past
            // them when scanning for digits.
            this._memoryClearButton = MakeMemoryButton("MC", 0);
            this._memoryRecallButton = MakeMemoryButton("MR", 1);
            this._memoryStoreButton = MakeMemoryButton("MS", 2);
            this._memoryAddButton = MakeMemoryButton("M+", 3);
            this._memorySubtractButton = MakeMemoryButton("M−", 4);

            // Function row — unary operations that act on the current entry.
            // Sits above the digit grid so the user's hand drops naturally
            // onto the digits without crossing function keys.
            this._percentButton = MakeGridButton("%", 0, 0);
            this._squareRootButton = MakeGridButton("√", 1, 0);
            this._squareButton = MakeGridButton("x²", 2, 0);
            this._reciprocalButton = MakeGridButton("¹⁄ₓ", 3, 0);

            // Edit / control row. CE/C/⌫ sit next to the divide so the user
            // can rub out a mistake without their hand jumping to a far
            // corner of the keypad.
            this._clearEntryButton = MakeGridButton("CE", 0, 1);
            this._clearButton = MakeGridButton("C", 1, 1);
            this._backspaceButton = MakeGridButton("⌫", 2, 1);
            this._divideButton = MakeGridButton("÷", 3, 1);

            // Digit row 7 8 9 ×. Telephone-keypad ordering: 7-9 on top so
            // the eye picks them up first when looking at a result and
            // reading downward. The operator stays on the right column
            // throughout the grid.
            this._digitButtons[7] = MakeGridButton("7", 0, 2);
            this._digitButtons[8] = MakeGridButton("8", 1, 2);
            this._digitButtons[9] = MakeGridButton("9", 2, 2);
            this._multiplyButton = MakeGridButton("×", 3, 2);

            // Digit row 4 5 6 with subtract.
            this._digitButtons[4] = MakeGridButton("4", 0, 3);
            this._digitButtons[5] = MakeGridButton("5", 1, 3);
            this._digitButtons[6] = MakeGridButton("6", 2, 3);
            this._subtractButton = MakeGridButton("−", 3, 3);

            // Digit row 1 2 3 with add.
            this._digitButtons[1] = MakeGridButton("1", 0, 4);
            this._digitButtons[2] = MakeGridButton("2", 1, 4);
            this._digitButtons[3] = MakeGridButton("3", 2, 4);
            this._addButton = MakeGridButton("+", 3, 4);

            // Last row — sign toggle, zero, decimal point, equals. Zero
            // gets the same width as the other digits rather than a double
            // cell; users hit it accurately enough without the wider key
            // and a uniform grid reads cleaner.
            this._signButton = MakeGridButton("±", 0, 5);
            this._digitButtons[0] = MakeGridButton("0", 1, 5);
            this._decimalButton = MakeGridButton(",", 2, 5);
            this._equalsButton = MakeGridButton("=", 3, 5);
            this._equalsButton.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);

            // History panel — title strip, scrollable list, and a "Clear"
            // button at the bottom. A ListBox is plenty here; a DataGridView
            // would carry the right two-column shape but it's overkill for
            // a calculator's recall list, and the extra chrome would crowd
            // a 235px column.
            this._historyTitleLabel = new System.Windows.Forms.Label();
            this._historyTitleLabel.Text = "History";
            this._historyTitleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this._historyTitleLabel.Location = new System.Drawing.Point(395, 32);
            this._historyTitleLabel.Size = new System.Drawing.Size(235, 22);
            this.Controls.Add(this._historyTitleLabel);

            this._historyList = new System.Windows.Forms.ListBox();
            this._historyList.Location = new System.Drawing.Point(395, 56);
            this._historyList.Size = new System.Drawing.Size(235, 494);
            this._historyList.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._historyList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._historyList.IntegralHeight = false;
            this.Controls.Add(this._historyList);

            this._clearHistoryButton = new System.Windows.Forms.Button();
            this._clearHistoryButton.Text = "Clear history";
            this._clearHistoryButton.Location = new System.Drawing.Point(395, 555);
            this._clearHistoryButton.Size = new System.Drawing.Size(235, 32);
            this._clearHistoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Controls.Add(this._clearHistoryButton);

            this.ResumeLayout(false);
        }

        // Positions a button on the main 4-column / 6-row grid. Cells are
        // 85x60 with 10px horizontal and 8px vertical gaps. Centralising the
        // arithmetic means a future grid resize is a one-line change.
        private System.Windows.Forms.Button MakeGridButton(string text, int column, int row)
        {
            var button = new System.Windows.Forms.Button();
            button.Text = text;
            button.Font = new System.Drawing.Font("Segoe UI", 14F);
            button.Location = new System.Drawing.Point(10 + column * 95, 190 + row * 68);
            button.Size = new System.Drawing.Size(85, 60);
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Controls.Add(button);
            return button;
        }

        // Helper for the memory toolbar so the five slots stay in sync — if
        // we ever want to recolour or resize the row we change one method,
        // not five button blocks.
        private System.Windows.Forms.Button MakeMemoryButton(string text, int slot)
        {
            var button = new System.Windows.Forms.Button();
            button.Text = text;
            button.Font = new System.Drawing.Font("Segoe UI", 9F);
            button.Location = new System.Drawing.Point(10 + slot * 75, 148);
            button.Size = new System.Drawing.Size(70, 32);
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Controls.Add(button);
            return button;
        }

        #endregion
    }
}
