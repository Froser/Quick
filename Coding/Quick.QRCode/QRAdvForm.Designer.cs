namespace Quick.QRCode
{
    partial class QRAdvForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QRAdvForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fg = new System.Windows.Forms.PictureBox();
            this.bg = new System.Windows.Forms.PictureBox();
            this.gc = new System.Windows.Forms.PictureBox();
            this.rLink = new System.Windows.Forms.RadioButton();
            this.textLogoURL = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.color = new System.Windows.Forms.ColorDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rNoLogo = new System.Windows.Forms.RadioButton();
            this.btnUpload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "前景颜色";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "背景颜色";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(316, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "渐变颜色";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Logo";
            // 
            // fg
            // 
            this.fg.BackColor = System.Drawing.Color.White;
            this.fg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fg.Location = new System.Drawing.Point(80, 17);
            this.fg.Name = "fg";
            this.fg.Size = new System.Drawing.Size(21, 18);
            this.fg.TabIndex = 4;
            this.fg.TabStop = false;
            this.fg.Click += new System.EventHandler(this.ChangeColor);
            // 
            // bg
            // 
            this.bg.BackColor = System.Drawing.Color.White;
            this.bg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bg.Location = new System.Drawing.Point(228, 17);
            this.bg.Name = "bg";
            this.bg.Size = new System.Drawing.Size(21, 18);
            this.bg.TabIndex = 5;
            this.bg.TabStop = false;
            this.bg.Click += new System.EventHandler(this.ChangeColor);
            // 
            // gc
            // 
            this.gc.BackColor = System.Drawing.Color.White;
            this.gc.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gc.Location = new System.Drawing.Point(376, 17);
            this.gc.Name = "gc";
            this.gc.Size = new System.Drawing.Size(21, 18);
            this.gc.TabIndex = 6;
            this.gc.TabStop = false;
            this.gc.Click += new System.EventHandler(this.ChangeColor);
            // 
            // rLink
            // 
            this.rLink.AutoSize = true;
            this.rLink.Location = new System.Drawing.Point(29, 79);
            this.rLink.Name = "rLink";
            this.rLink.Size = new System.Drawing.Size(71, 16);
            this.rLink.TabIndex = 0;
            this.rLink.Text = "使用外链";
            this.rLink.UseVisualStyleBackColor = true;
            // 
            // textLogoURL
            // 
            this.textLogoURL.Location = new System.Drawing.Point(166, 77);
            this.textLogoURL.Name = "textLogoURL";
            this.textLogoURL.Size = new System.Drawing.Size(263, 21);
            this.textLogoURL.TabIndex = 1;
            this.textLogoURL.Text = "http://";
            this.textLogoURL.TextChanged += new System.EventHandler(this.textLogoURL_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(261, 153);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "地址:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(354, 153);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rNoLogo
            // 
            this.rNoLogo.AutoSize = true;
            this.rNoLogo.Checked = true;
            this.rNoLogo.Location = new System.Drawing.Point(29, 113);
            this.rNoLogo.Name = "rNoLogo";
            this.rNoLogo.Size = new System.Drawing.Size(83, 16);
            this.rNoLogo.TabIndex = 2;
            this.rNoLogo.TabStop = true;
            this.rNoLogo.Text = "不使用Logo";
            this.rNoLogo.UseVisualStyleBackColor = true;
            // 
            // btnUpload
            // 
            this.btnUpload.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUpload.Location = new System.Drawing.Point(29, 153);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(138, 23);
            this.btnUpload.TabIndex = 3;
            this.btnUpload.Text = "上传图片到外链(&A)";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // QRAdvForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(461, 197);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.rNoLogo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.textLogoURL);
            this.Controls.Add(this.rLink);
            this.Controls.Add(this.gc);
            this.Controls.Add(this.bg);
            this.Controls.Add(this.fg);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QRAdvForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QRCode 生成选项";
            ((System.ComponentModel.ISupportInitialize)(this.fg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox fg;
        private System.Windows.Forms.PictureBox bg;
        private System.Windows.Forms.PictureBox gc;
        private System.Windows.Forms.RadioButton rLink;
        private System.Windows.Forms.TextBox textLogoURL;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ColorDialog color;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rNoLogo;
        private System.Windows.Forms.Button btnUpload;
    }
}