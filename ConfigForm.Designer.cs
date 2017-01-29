namespace Kraken
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.layer1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.letterbox = new System.Windows.Forms.ComboBox();
            this.blank2 = new System.Windows.Forms.Button();
            this.blank1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.layer2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.frameskip = new System.Windows.Forms.NumericUpDown();
            this.showfps = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.framerate = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.randomize = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.layer1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layer2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameskip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.framerate)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // layer1
            // 
            this.layer1.Location = new System.Drawing.Point(25, 37);
            this.layer1.Maximum = new decimal(new int[] {
            326,
            0,
            0,
            0});
            this.layer1.Name = "layer1";
            this.layer1.Size = new System.Drawing.Size(120, 20);
            this.layer1.TabIndex = 0;
            this.layer1.ValueChanged += new System.EventHandler(this.layer1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Background layer 1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.randomize);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.letterbox);
            this.groupBox1.Controls.Add(this.blank2);
            this.groupBox1.Controls.Add(this.blank1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.layer2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.frameskip);
            this.groupBox1.Controls.Add(this.showfps);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.framerate);
            this.groupBox1.Controls.Add(this.layer1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 293);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Aspect ratio";
            // 
            // letterbox
            // 
            this.letterbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.letterbox.FormattingEnabled = true;
            this.letterbox.Items.AddRange(new object[] {
            "Full screen (8:7)",
            "Wide letterbox (4:3)",
            "Medium letterbox (2:1)",
            "Narrow letterbox (8:3)"});
            this.letterbox.Location = new System.Drawing.Point(25, 138);
            this.letterbox.Name = "letterbox";
            this.letterbox.Size = new System.Drawing.Size(200, 21);
            this.letterbox.TabIndex = 11;
            this.letterbox.SelectedIndexChanged += new System.EventHandler(this.letterbox_SelectedIndexChanged);
            // 
            // blank2
            // 
            this.blank2.Location = new System.Drawing.Point(150, 85);
            this.blank2.Name = "blank2";
            this.blank2.Size = new System.Drawing.Size(75, 22);
            this.blank2.TabIndex = 10;
            this.blank2.Text = "Blank";
            this.blank2.UseVisualStyleBackColor = true;
            this.blank2.Click += new System.EventHandler(this.blank2_Click);
            // 
            // blank1
            // 
            this.blank1.Location = new System.Drawing.Point(150, 36);
            this.blank1.Name = "blank1";
            this.blank1.Size = new System.Drawing.Size(75, 22);
            this.blank1.TabIndex = 10;
            this.blank1.Text = "Blank";
            this.blank1.UseVisualStyleBackColor = true;
            this.blank1.Click += new System.EventHandler(this.blank1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Background layer 2";
            // 
            // layer2
            // 
            this.layer2.Location = new System.Drawing.Point(25, 86);
            this.layer2.Maximum = new decimal(new int[] {
            326,
            0,
            0,
            0});
            this.layer2.Name = "layer2";
            this.layer2.Size = new System.Drawing.Size(120, 20);
            this.layer2.TabIndex = 8;
            this.layer2.ValueChanged += new System.EventHandler(this.layer2_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 244);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Frameskip";
            // 
            // frameskip
            // 
            this.frameskip.Location = new System.Drawing.Point(25, 260);
            this.frameskip.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.frameskip.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameskip.Name = "frameskip";
            this.frameskip.Size = new System.Drawing.Size(120, 20);
            this.frameskip.TabIndex = 6;
            this.frameskip.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.frameskip.ValueChanged += new System.EventHandler(this.frameskip_ValueChanged);
            this.frameskip.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frameskip_KeyUp);
            // 
            // showfps
            // 
            this.showfps.AutoSize = true;
            this.showfps.Location = new System.Drawing.Point(25, 219);
            this.showfps.Name = "showfps";
            this.showfps.Size = new System.Drawing.Size(115, 17);
            this.showfps.TabIndex = 5;
            this.showfps.Text = "Show FPS counter";
            this.showfps.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(147, 196);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "fps";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Framerate";
            // 
            // framerate
            // 
            this.framerate.Location = new System.Drawing.Point(25, 191);
            this.framerate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.framerate.Name = "framerate";
            this.framerate.Size = new System.Drawing.Size(120, 20);
            this.framerate.TabIndex = 2;
            this.framerate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.framerate.ValueChanged += new System.EventHandler(this.framerate_ValueChanged);
            this.framerate.KeyUp += new System.Windows.Forms.KeyEventHandler(this.framerate_KeyUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Location = new System.Drawing.Point(290, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 293);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pictureBox1.Location = new System.Drawing.Point(16, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(320, 224);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(543, 389);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(99, 28);
            this.ButtonCancel.TabIndex = 4;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonOK
            // 
            this.ButtonOK.Location = new System.Drawing.Point(438, 389);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(99, 28);
            this.ButtonOK.TabIndex = 4;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Gray;
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(654, 71);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // randomize
            // 
            this.randomize.AutoSize = true;
            this.randomize.Location = new System.Drawing.Point(173, 64);
            this.randomize.Name = "randomize";
            this.randomize.Size = new System.Drawing.Size(79, 17);
            this.randomize.TabIndex = 13;
            this.randomize.Text = "Randomize";
            this.randomize.UseVisualStyleBackColor = true;
            this.randomize.CheckedChanged += new System.EventHandler(this.randomize_CheckedChanged);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 429);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kraken Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.layer1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layer2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameskip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.framerate)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown layer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown framerate;
        private System.Windows.Forms.CheckBox showfps;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown frameskip;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown layer2;
        private System.Windows.Forms.Button blank2;
        private System.Windows.Forms.Button blank1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox letterbox;
        private System.Windows.Forms.CheckBox randomize;
    }
}
