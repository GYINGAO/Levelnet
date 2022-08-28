namespace SplashScreenDemo {
    partial class MultiFuncLoading {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_tips = new System.Windows.Forms.Label();
            this.lbl_tips_son = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_jd = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_cur = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Image = global::LevelnetAdjustment.Properties.Resources.loading3;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 102);
            this.label1.TabIndex = 0;
            // 
            // lbl_tips
            // 
            this.lbl_tips.AutoSize = true;
            this.lbl_tips.Location = new System.Drawing.Point(153, 16);
            this.lbl_tips.Name = "lbl_tips";
            this.lbl_tips.Size = new System.Drawing.Size(161, 18);
            this.lbl_tips.TabIndex = 3;
            this.lbl_tips.Text = "加载中，请稍等...";
            // 
            // lbl_tips_son
            // 
            this.lbl_tips_son.AutoSize = true;
            this.lbl_tips_son.Location = new System.Drawing.Point(153, 54);
            this.lbl_tips_son.Name = "lbl_tips_son";
            this.lbl_tips_son.Size = new System.Drawing.Size(170, 18);
            this.lbl_tips_son.TabIndex = 4;
            this.lbl_tips_son.Text = "Please Waitting...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 88);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "执行进度：";
            // 
            // lbl_jd
            // 
            this.lbl_jd.AutoSize = true;
            this.lbl_jd.Location = new System.Drawing.Point(252, 88);
            this.lbl_jd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_jd.Name = "lbl_jd";
            this.lbl_jd.Size = new System.Drawing.Size(62, 18);
            this.lbl_jd.TabIndex = 6;
            this.lbl_jd.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 141);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 18);
            this.label4.TabIndex = 7;
            this.label4.Text = "当前执行：";
            // 
            // lbl_cur
            // 
            this.lbl_cur.Location = new System.Drawing.Point(124, 136);
            this.lbl_cur.Margin = new System.Windows.Forms.Padding(4);
            this.lbl_cur.Multiline = true;
            this.lbl_cur.Name = "lbl_cur";
            this.lbl_cur.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lbl_cur.Size = new System.Drawing.Size(484, 102);
            this.lbl_cur.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(496, 29);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 69);
            this.button1.TabIndex = 10;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MultiFuncLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 250);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbl_cur);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_jd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_tips_son);
            this.Controls.Add(this.lbl_tips);
            this.Controls.Add(this.label1);
            this.Name = "MultiFuncLoading";
            this.Text = "Loading";
            this.Load += new System.EventHandler(this.Loading_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_tips;
        private System.Windows.Forms.Label lbl_tips_son;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_jd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox lbl_cur;
        private System.Windows.Forms.Button button1;
    }
}