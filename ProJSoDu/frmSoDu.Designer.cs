namespace ProJSoDu
{
    partial class frmSoDu
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
            this.wVJS1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wVJS1
            // 
            this.wVJS1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wVJS1.Location = new System.Drawing.Point(0, 0);
            this.wVJS1.MinimumSize = new System.Drawing.Size(20, 20);
            this.wVJS1.Name = "wVJS1";
            this.wVJS1.ScriptErrorsSuppressed = true;
            this.wVJS1.Size = new System.Drawing.Size(800, 450);
            this.wVJS1.TabIndex = 7;
            this.wVJS1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wVJS1_DocumentCompleted);
            // 
            // frmSoDu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.wVJS1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSoDu";
            this.Text = "Số dư";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSoDu_FormClosing);
            this.Load += new System.EventHandler(this.frmSoDu_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wVJS1;
    }
}

