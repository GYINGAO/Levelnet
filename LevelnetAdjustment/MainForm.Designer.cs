
namespace LevelnetAdjustment {
    partial class MainForm {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RawDataDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.KnDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AdjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClosureErrorDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrossErrorDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConstraintNetworkDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RankDefectNetworkDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResultDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OriginalDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.COSADropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisPower = new System.Windows.Forms.ToolStripMenuItem();
            this.StationPower = new System.Windows.Forms.ToolStripMenuItem();
            this.HandbookDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.AdjToolStripMenuItem,
            this.ReportToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1179, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewDropItem,
            this.OpenDropItem,
            this.RawDataDropItem,
            this.KnDropItem,
            this.ClearDropItem,
            this.ExitDropItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.FileToolStripMenuItem.Text = "文件";
            // 
            // NewDropItem
            // 
            this.NewDropItem.Name = "NewDropItem";
            this.NewDropItem.Size = new System.Drawing.Size(180, 22);
            this.NewDropItem.Text = "新建";
            this.NewDropItem.Click += new System.EventHandler(this.NewDropItem_Click);
            // 
            // OpenDropItem
            // 
            this.OpenDropItem.Name = "OpenDropItem";
            this.OpenDropItem.Size = new System.Drawing.Size(180, 22);
            this.OpenDropItem.Text = "读取高差观测文件";
            this.OpenDropItem.Click += new System.EventHandler(this.OpenDropItem_Click);
            // 
            // RawDataDropItem
            // 
            this.RawDataDropItem.Name = "RawDataDropItem";
            this.RawDataDropItem.Size = new System.Drawing.Size(180, 22);
            this.RawDataDropItem.Text = "读取原始文件";
            this.RawDataDropItem.Click += new System.EventHandler(this.RawDataDropItem_Click);
            // 
            // KnDropItem
            // 
            this.KnDropItem.Name = "KnDropItem";
            this.KnDropItem.Size = new System.Drawing.Size(180, 22);
            this.KnDropItem.Text = "读取已知点数据";
            this.KnDropItem.Click += new System.EventHandler(this.KnDropItem_Click);
            // 
            // ClearDropItem
            // 
            this.ClearDropItem.Name = "ClearDropItem";
            this.ClearDropItem.Size = new System.Drawing.Size(180, 22);
            this.ClearDropItem.Text = "清空数据";
            this.ClearDropItem.Click += new System.EventHandler(this.ClearDropItem_Click);
            // 
            // ExitDropItem
            // 
            this.ExitDropItem.Name = "ExitDropItem";
            this.ExitDropItem.Size = new System.Drawing.Size(180, 22);
            this.ExitDropItem.Text = "退出";
            this.ExitDropItem.Click += new System.EventHandler(this.ExitDropItem_Click);
            // 
            // AdjToolStripMenuItem
            // 
            this.AdjToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClosureErrorDropItem,
            this.GrossErrorDropItem,
            this.ConstraintNetworkDropItem,
            this.RankDefectNetworkDropItem,
            this.OptionDropItem});
            this.AdjToolStripMenuItem.Name = "AdjToolStripMenuItem";
            this.AdjToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.AdjToolStripMenuItem.Text = "数据处理";
            // 
            // ClosureErrorDropItem
            // 
            this.ClosureErrorDropItem.Name = "ClosureErrorDropItem";
            this.ClosureErrorDropItem.Size = new System.Drawing.Size(136, 22);
            this.ClosureErrorDropItem.Text = "闭合差计算";
            this.ClosureErrorDropItem.Click += new System.EventHandler(this.ClosureErrorDropItem_Click);
            // 
            // GrossErrorDropItem
            // 
            this.GrossErrorDropItem.Name = "GrossErrorDropItem";
            this.GrossErrorDropItem.Size = new System.Drawing.Size(136, 22);
            this.GrossErrorDropItem.Text = "粗差探测";
            this.GrossErrorDropItem.Click += new System.EventHandler(this.GrossErrorDropItem_Click);
            // 
            // ConstraintNetworkDropItem
            // 
            this.ConstraintNetworkDropItem.Name = "ConstraintNetworkDropItem";
            this.ConstraintNetworkDropItem.Size = new System.Drawing.Size(136, 22);
            this.ConstraintNetworkDropItem.Text = "约束网平差";
            this.ConstraintNetworkDropItem.Click += new System.EventHandler(this.LevelnetDropItem_Click);
            // 
            // RankDefectNetworkDropItem
            // 
            this.RankDefectNetworkDropItem.Name = "RankDefectNetworkDropItem";
            this.RankDefectNetworkDropItem.Size = new System.Drawing.Size(136, 22);
            this.RankDefectNetworkDropItem.Text = "拟稳平差";
            this.RankDefectNetworkDropItem.Click += new System.EventHandler(this.RankDefectNetworkDropItem_Click);
            // 
            // OptionDropItem
            // 
            this.OptionDropItem.Name = "OptionDropItem";
            this.OptionDropItem.Size = new System.Drawing.Size(136, 22);
            this.OptionDropItem.Text = "设置与选项";
            this.OptionDropItem.Click += new System.EventHandler(this.OptionDropItem_Click);
            // 
            // ReportToolStripMenuItem
            // 
            this.ReportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ResultDropItem,
            this.OriginalDropItem,
            this.COSADropItem,
            this.HandbookDropItem});
            this.ReportToolStripMenuItem.Name = "ReportToolStripMenuItem";
            this.ReportToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.ReportToolStripMenuItem.Text = "报表";
            // 
            // ResultDropItem
            // 
            this.ResultDropItem.Name = "ResultDropItem";
            this.ResultDropItem.Size = new System.Drawing.Size(157, 22);
            this.ResultDropItem.Text = "平差结果";
            // 
            // OriginalDropItem
            // 
            this.OriginalDropItem.Name = "OriginalDropItem";
            this.OriginalDropItem.Size = new System.Drawing.Size(157, 22);
            this.OriginalDropItem.Text = "原始观测数据";
            // 
            // COSADropItem
            // 
            this.COSADropItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DisPower,
            this.StationPower});
            this.COSADropItem.Name = "COSADropItem";
            this.COSADropItem.Size = new System.Drawing.Size(157, 22);
            this.COSADropItem.Text = "COSA数据格式";
            // 
            // DisPower
            // 
            this.DisPower.Name = "DisPower";
            this.DisPower.Size = new System.Drawing.Size(148, 22);
            this.DisPower.Text = "按距离定权";
            this.DisPower.Click += new System.EventHandler(this.DisPower_Click);
            // 
            // StationPower
            // 
            this.StationPower.Name = "StationPower";
            this.StationPower.Size = new System.Drawing.Size(148, 22);
            this.StationPower.Text = "按测站数定权";
            this.StationPower.Click += new System.EventHandler(this.StationPower_Click);
            // 
            // HandbookDropItem
            // 
            this.HandbookDropItem.Name = "HandbookDropItem";
            this.HandbookDropItem.Size = new System.Drawing.Size(157, 22);
            this.HandbookDropItem.Text = "观测手簿";
            this.HandbookDropItem.Click += new System.EventHandler(this.HandbookDropItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutDropItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.HelpToolStripMenuItem.Text = "帮助";
            // 
            // AboutDropItem
            // 
            this.AboutDropItem.Name = "AboutDropItem";
            this.AboutDropItem.Size = new System.Drawing.Size(100, 22);
            this.AboutDropItem.Text = "关于";
            this.AboutDropItem.Click += new System.EventHandler(this.AboutDropItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1179, 685);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "水准网平差程序";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NewDropItem;
        private System.Windows.Forms.ToolStripMenuItem OpenDropItem;
        private System.Windows.Forms.ToolStripMenuItem ExitDropItem;
        private System.Windows.Forms.ToolStripMenuItem AdjToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConstraintNetworkDropItem;
        private System.Windows.Forms.ToolStripMenuItem OptionDropItem;
        private System.Windows.Forms.ToolStripMenuItem ReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ResultDropItem;
        private System.Windows.Forms.ToolStripMenuItem OriginalDropItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClosureErrorDropItem;
        private System.Windows.Forms.ToolStripMenuItem AboutDropItem;
        private System.Windows.Forms.ToolStripMenuItem RawDataDropItem;
        private System.Windows.Forms.ToolStripMenuItem COSADropItem;
        private System.Windows.Forms.ToolStripMenuItem KnDropItem;
        private System.Windows.Forms.ToolStripMenuItem ClearDropItem;
        private System.Windows.Forms.ToolStripMenuItem HandbookDropItem;
        private System.Windows.Forms.ToolStripMenuItem DisPower;
        private System.Windows.Forms.ToolStripMenuItem StationPower;
        private System.Windows.Forms.ToolStripMenuItem GrossErrorDropItem;
        private System.Windows.Forms.ToolStripMenuItem RankDefectNetworkDropItem;
    }
}

