﻿namespace AutoLoadTicket
{
    partial class frmVJ2
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
            this.wVJ = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wVJ
            // 
            this.wVJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wVJ.Location = new System.Drawing.Point(0, 0);
            this.wVJ.MinimumSize = new System.Drawing.Size(20, 20);
            this.wVJ.Name = "wVJ";
            this.wVJ.ScriptErrorsSuppressed = true;
            this.wVJ.Size = new System.Drawing.Size(800, 450);
            this.wVJ.TabIndex = 8;
            this.wVJ.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wVJ_DocumentCompleted);
            // 
            // frmVJ2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.wVJ);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmVJ2";
            this.Text = "VietJet";
            this.Load += new System.EventHandler(this.frmVJ2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wVJ;
    }
}