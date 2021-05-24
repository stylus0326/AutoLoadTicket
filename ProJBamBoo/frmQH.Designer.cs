namespace ProJBamBoo
{
    partial class frmQH
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
            this.components = new System.ComponentModel.Container();
            this.wQH = new System.Windows.Forms.WebBrowser();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // wQH
            // 
            this.wQH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wQH.Location = new System.Drawing.Point(0, 0);
            this.wQH.MinimumSize = new System.Drawing.Size(20, 20);
            this.wQH.Name = "wQH";
            this.wQH.ScriptErrorsSuppressed = true;
            this.wQH.Size = new System.Drawing.Size(800, 450);
            this.wQH.TabIndex = 7;
            this.wQH.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wQH_DocumentCompleted);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // frmQH
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.wQH);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmQH";
            this.Text = "BamBoo";
            this.Load += new System.EventHandler(this.frmQH_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wQH;
        private System.Windows.Forms.Timer timer;
    }
}