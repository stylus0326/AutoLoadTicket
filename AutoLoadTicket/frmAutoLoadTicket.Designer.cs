namespace AutoLoadTicket
{
    partial class frmAutoLoadTicket
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
            this.chkVJ = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpVU = new System.Windows.Forms.DateTimePicker();
            this.btnVU = new System.Windows.Forms.Button();
            this.chkVU = new System.Windows.Forms.CheckBox();
            this.dtpVN = new System.Windows.Forms.DateTimePicker();
            this.btnVN = new System.Windows.Forms.Button();
            this.btnQH = new System.Windows.Forms.Button();
            this.btnVJ = new System.Windows.Forms.Button();
            this.dtpQH = new System.Windows.Forms.DateTimePicker();
            this.dtpVJ = new System.Windows.Forms.DateTimePicker();
            this.chkVN = new System.Windows.Forms.CheckBox();
            this.chkQH = new System.Windows.Forms.CheckBox();
            this.chkAuto = new System.Windows.Forms.CheckBox();
            this.VJ2 = new System.Windows.Forms.Button();
            this.VJ3 = new System.Windows.Forms.Button();
            this.TimerLayDuLieu = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkVJ
            // 
            this.chkVJ.AutoSize = true;
            this.chkVJ.Checked = true;
            this.chkVJ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVJ.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVJ.ForeColor = System.Drawing.Color.Red;
            this.chkVJ.Location = new System.Drawing.Point(6, 19);
            this.chkVJ.Name = "chkVJ";
            this.chkVJ.Size = new System.Drawing.Size(69, 19);
            this.chkVJ.TabIndex = 0;
            this.chkVJ.Text = "VietJet";
            this.chkVJ.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpVU);
            this.groupBox1.Controls.Add(this.btnVU);
            this.groupBox1.Controls.Add(this.chkVU);
            this.groupBox1.Controls.Add(this.dtpVN);
            this.groupBox1.Controls.Add(this.btnVN);
            this.groupBox1.Controls.Add(this.btnQH);
            this.groupBox1.Controls.Add(this.btnVJ);
            this.groupBox1.Controls.Add(this.dtpQH);
            this.groupBox1.Controls.Add(this.dtpVJ);
            this.groupBox1.Controls.Add(this.chkVN);
            this.groupBox1.Controls.Add(this.chkQH);
            this.groupBox1.Controls.Add(this.chkVJ);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 125);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lấy dữ liệu tự động";
            // 
            // dtpVU
            // 
            this.dtpVU.CustomFormat = "dd-MM-yyyy";
            this.dtpVU.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpVU.Location = new System.Drawing.Point(92, 97);
            this.dtpVU.Name = "dtpVU";
            this.dtpVU.Size = new System.Drawing.Size(80, 20);
            this.dtpVU.TabIndex = 8;
            // 
            // btnVU
            // 
            this.btnVU.Location = new System.Drawing.Point(178, 95);
            this.btnVU.Name = "btnVU";
            this.btnVU.Size = new System.Drawing.Size(48, 24);
            this.btnVU.TabIndex = 7;
            this.btnVU.Text = "Chạy";
            this.btnVU.UseVisualStyleBackColor = true;
            this.btnVU.Click += new System.EventHandler(this.btnVU_Click);
            // 
            // chkVU
            // 
            this.chkVU.AutoSize = true;
            this.chkVU.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVU.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.chkVU.Location = new System.Drawing.Point(6, 98);
            this.chkVU.Name = "chkVU";
            this.chkVU.Size = new System.Drawing.Size(89, 19);
            this.chkVU.TabIndex = 6;
            this.chkVU.Text = "VietTravel";
            this.chkVU.UseVisualStyleBackColor = true;
            // 
            // dtpVN
            // 
            this.dtpVN.CustomFormat = "dd-MM-yyyy";
            this.dtpVN.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpVN.Location = new System.Drawing.Point(92, 71);
            this.dtpVN.Name = "dtpVN";
            this.dtpVN.Size = new System.Drawing.Size(80, 20);
            this.dtpVN.TabIndex = 5;
            // 
            // btnVN
            // 
            this.btnVN.Location = new System.Drawing.Point(178, 69);
            this.btnVN.Name = "btnVN";
            this.btnVN.Size = new System.Drawing.Size(48, 24);
            this.btnVN.TabIndex = 4;
            this.btnVN.Text = "Chạy";
            this.btnVN.UseVisualStyleBackColor = true;
            this.btnVN.Click += new System.EventHandler(this.btnVN_Click);
            // 
            // btnQH
            // 
            this.btnQH.Location = new System.Drawing.Point(178, 42);
            this.btnQH.Name = "btnQH";
            this.btnQH.Size = new System.Drawing.Size(49, 24);
            this.btnQH.TabIndex = 2;
            this.btnQH.Text = "Chạy";
            this.btnQH.UseVisualStyleBackColor = true;
            this.btnQH.Click += new System.EventHandler(this.btnQH_Click);
            // 
            // btnVJ
            // 
            this.btnVJ.Location = new System.Drawing.Point(178, 15);
            this.btnVJ.Name = "btnVJ";
            this.btnVJ.Size = new System.Drawing.Size(49, 24);
            this.btnVJ.TabIndex = 2;
            this.btnVJ.Text = "Chạy";
            this.btnVJ.UseVisualStyleBackColor = true;
            this.btnVJ.Click += new System.EventHandler(this.btnVJ_Click);
            // 
            // dtpQH
            // 
            this.dtpQH.CustomFormat = "dd-MM-yyyy";
            this.dtpQH.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpQH.Location = new System.Drawing.Point(92, 44);
            this.dtpQH.Name = "dtpQH";
            this.dtpQH.Size = new System.Drawing.Size(80, 20);
            this.dtpQH.TabIndex = 1;
            // 
            // dtpVJ
            // 
            this.dtpVJ.CustomFormat = "dd-MM-yyyy";
            this.dtpVJ.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpVJ.Location = new System.Drawing.Point(92, 17);
            this.dtpVJ.Name = "dtpVJ";
            this.dtpVJ.Size = new System.Drawing.Size(80, 20);
            this.dtpVJ.TabIndex = 1;
            // 
            // chkVN
            // 
            this.chkVN.AutoSize = true;
            this.chkVN.Checked = true;
            this.chkVN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVN.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.chkVN.Location = new System.Drawing.Point(6, 72);
            this.chkVN.Name = "chkVN";
            this.chkVN.Size = new System.Drawing.Size(80, 19);
            this.chkVN.TabIndex = 0;
            this.chkVN.Text = "VietNam";
            this.chkVN.UseVisualStyleBackColor = true;
            // 
            // chkQH
            // 
            this.chkQH.AutoSize = true;
            this.chkQH.Checked = true;
            this.chkQH.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQH.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkQH.ForeColor = System.Drawing.Color.ForestGreen;
            this.chkQH.Location = new System.Drawing.Point(6, 44);
            this.chkQH.Name = "chkQH";
            this.chkQH.Size = new System.Drawing.Size(80, 19);
            this.chkQH.TabIndex = 0;
            this.chkQH.Text = "BamBoo";
            this.chkQH.UseVisualStyleBackColor = true;
            // 
            // chkAuto
            // 
            this.chkAuto.AutoSize = true;
            this.chkAuto.Location = new System.Drawing.Point(165, 136);
            this.chkAuto.Name = "chkAuto";
            this.chkAuto.Size = new System.Drawing.Size(67, 17);
            this.chkAuto.TabIndex = 2;
            this.chkAuto.Text = "Tự động";
            this.chkAuto.UseVisualStyleBackColor = true;
            this.chkAuto.CheckedChanged += new System.EventHandler(this.chkAuto_CheckedChanged);
            // 
            // VJ2
            // 
            this.VJ2.Location = new System.Drawing.Point(6, 131);
            this.VJ2.Name = "VJ2";
            this.VJ2.Size = new System.Drawing.Size(49, 24);
            this.VJ2.TabIndex = 3;
            this.VJ2.Text = "VJ2";
            this.VJ2.UseVisualStyleBackColor = true;
            this.VJ2.Click += new System.EventHandler(this.VJ2_Click);
            // 
            // VJ3
            // 
            this.VJ3.Location = new System.Drawing.Point(61, 131);
            this.VJ3.Name = "VJ3";
            this.VJ3.Size = new System.Drawing.Size(49, 24);
            this.VJ3.TabIndex = 3;
            this.VJ3.Text = "VJ3";
            this.VJ3.UseVisualStyleBackColor = true;
            this.VJ3.Click += new System.EventHandler(this.VJ3_Click);
            // 
            // TimerLayDuLieu
            // 
            this.TimerLayDuLieu.Interval = 1;
            this.TimerLayDuLieu.Tick += new System.EventHandler(this.TimerLayDuLieu_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(116, 131);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 24);
            this.button2.TabIndex = 5;
            this.button2.Text = "AGS";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // frmAutoLoadTicket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 161);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.chkAuto);
            this.Controls.Add(this.VJ3);
            this.Controls.Add(this.VJ2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAutoLoadTicket";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "[Auto]:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAutoLoadTicket_FormClosing);
            this.Load += new System.EventHandler(this.frmAutoLoadTicket_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkVJ;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnVN;
        private System.Windows.Forms.Button btnQH;
        private System.Windows.Forms.Button btnVJ;
        private System.Windows.Forms.DateTimePicker dtpQH;
        private System.Windows.Forms.DateTimePicker dtpVJ;
        private System.Windows.Forms.CheckBox chkVN;
        private System.Windows.Forms.CheckBox chkQH;
        private System.Windows.Forms.CheckBox chkAuto;
        private System.Windows.Forms.Button VJ2;
        private System.Windows.Forms.Button VJ3;
        private System.Windows.Forms.Timer TimerLayDuLieu;
        private System.Windows.Forms.DateTimePicker dtpVN;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DateTimePicker dtpVU;
        private System.Windows.Forms.Button btnVU;
        private System.Windows.Forms.CheckBox chkVU;
    }
}