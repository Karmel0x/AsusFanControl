namespace AsusFanControlGUI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.labelRPM = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(12, 12);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(300, 45);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trackBar1_KeyUp);
            this.trackBar1.MouseCaptureChanged += new System.EventHandler(this.trackBar1_MouseCaptureChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current value:";
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(91, 60);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(10, 13);
            this.labelValue.TabIndex = 2;
            this.labelValue.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current RPM:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(22, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "↻";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelRPM
            // 
            this.labelRPM.AutoSize = true;
            this.labelRPM.Location = new System.Drawing.Point(117, 81);
            this.labelRPM.Name = "labelRPM";
            this.labelRPM.Size = new System.Drawing.Size(10, 13);
            this.labelRPM.TabIndex = 5;
            this.labelRPM.Text = "-";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 111);
            this.Controls.Add(this.labelRPM);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Asus Fan Control";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelRPM;
    }
}

