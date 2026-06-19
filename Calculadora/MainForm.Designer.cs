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
            this.Controls.Add(this._signatureLabel);

            // Display panel — wraps the value and (later) the expression
            // preview so we can paint a single background colour behind both.
            this._displayPanel = new System.Windows.Forms.Panel();
            this._displayPanel.Location = new System.Drawing.Point(10, 32);
            this._displayPanel.Size = new System.Drawing.Size(370, 110);
            this._displayPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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

            this.ResumeLayout(false);
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
