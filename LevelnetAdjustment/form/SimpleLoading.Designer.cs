namespace SplashScreenDemo {
    partial class SimpleLoading {
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
            this.lbl_tips_son = new System.Windows.Forms.Label();
            this.lbl_tips = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_tips_son
            // 
            this.lbl_tips_son.AutoSize = true;
            this.lbl_tips_son.Location = new System.Drawing.Point(116, 56);
            this.lbl_tips_son.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_tips_son.Name = "lbl_tips_son";
            this.lbl_tips_son.Size = new System.Drawing.Size(113, 12);
            this.lbl_tips_son.TabIndex = 7;
            this.lbl_tips_son.Text = "Please Waitting...";
            // 
            // lbl_tips
            // 
            this.lbl_tips.AutoSize = true;
            this.lbl_tips.Location = new System.Drawing.Point(116, 31);
            this.lbl_tips.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_tips.Name = "lbl_tips";
            this.lbl_tips.Size = new System.Drawing.Size(107, 12);
            this.lbl_tips.TabIndex = 6;
            this.lbl_tips.Text = "加载中，请稍等...";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Image = global::LevelnetAdjustment.Properties.Resources.loading3;
            this.label1.Location = new System.Drawing.Point(29, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 69);
            this.label1.TabIndex = 5;
            // 
            // SimpleLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 95);
            this.Controls.Add(this.lbl_tips_son);
            this.Controls.Add(this.lbl_tips);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SimpleLoading";
            this.Text = "提示";
            this.Load += new System.EventHandler(this.SimpleLoading_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_tips_son;
        private System.Windows.Forms.Label lbl_tips;
        private System.Windows.Forms.Label label1;
    }
}