namespace Calculator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label _signatureLabel = null!;
        private System.Windows.Forms.Panel _displayPanel = null!;
        private System.Windows.Forms.Label _displayLabel = null!;

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

            this.ResumeLayout(false);
        }

        #endregion
    }
}
