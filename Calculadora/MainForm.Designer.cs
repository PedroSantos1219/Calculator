namespace Calculator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label _signatureLabel = null!;

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

            this.ResumeLayout(false);
        }

        #endregion
    }
}
