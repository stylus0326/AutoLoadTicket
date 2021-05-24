namespace VietNamProJ
{
    partial class frmVN
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wVN = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wVN
            // 
            this.wVN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wVN.Location = new System.Drawing.Point(0, 0);
            this.wVN.MinimumSize = new System.Drawing.Size(20, 20);
            this.wVN.Name = "wVN";
            this.wVN.ScriptErrorsSuppressed = true;
            this.wVN.Size = new System.Drawing.Size(800, 450);
            this.wVN.TabIndex = 9;
            // 
            // frmVN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.wVN);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmVN";
            this.Text = "VietNam";
            this.Load += new System.EventHandler(this.frmVN_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wVN;
    }
}