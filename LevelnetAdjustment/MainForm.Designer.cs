
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_close = new System.Windows.Forms.ToolStripMenuItem();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_open = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_read = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AdjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClosureErrorDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrossErrorDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConstraintNetworkDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RankDefectNetworkDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.COSADropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisPower = new System.Windows.Forms.ToolStripMenuItem();
            this.StationPower = new System.Windows.Forms.ToolStripMenuItem();
            this.HandbookDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutDropItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.新建NToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.打开OToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.帮助LToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.使用说明ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_close});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // toolStripMenuItem_close
            // 
            this.toolStripMenuItem_close.Name = "toolStripMenuItem_close";
            this.toolStripMenuItem_close.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem_close.Text = "关闭";
            this.toolStripMenuItem_close.Click += new System.EventHandler(this.toolStripMenuItem_close_Click);
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewDropItem,
            this.toolStripMenuItem_open,
            this.toolStripMenuItem_read,
            this.ClearDropItem,
            this.toolStripSeparator7,
            this.ExitDropItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.FileToolStripMenuItem.Text = "文件";
            // 
            // NewDropItem
            // 
            this.NewDropItem.Name = "NewDropItem";
            this.NewDropItem.Size = new System.Drawing.Size(124, 22);
            this.NewDropItem.Text = "新建";
            this.NewDropItem.Click += new System.EventHandler(this.NewDropItem_Click);
            // 
            // toolStripMenuItem_open
            // 
            this.toolStripMenuItem_open.Name = "toolStripMenuItem_open";
            this.toolStripMenuItem_open.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem_open.Text = "打开";
            this.toolStripMenuItem_open.Click += new System.EventHandler(this.toolStripMenuItem_open_Click);
            // 
            // toolStripMenuItem_read
            // 
            this.toolStripMenuItem_read.Name = "toolStripMenuItem_read";
            this.toolStripMenuItem_read.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem_read.Text = "数据读取与设置";
            this.toolStripMenuItem_read.Click += new System.EventHandler(this.toolStripMenuItem_read_Click);
            // 
            // ClearDropItem
            // 
            this.ClearDropItem.Name = "ClearDropItem";
            this.ClearDropItem.Size = new System.Drawing.Size(124, 22);
            this.ClearDropItem.Text = "清空数据";
            this.ClearDropItem.Click += new System.EventHandler(this.ClearDropItem_Click);
            // 
            // ExitDropItem
            // 
            this.ExitDropItem.Name = "ExitDropItem";
            this.ExitDropItem.Size = new System.Drawing.Size(124, 22);
            this.ExitDropItem.Text = "退出";
            this.ExitDropItem.Click += new System.EventHandler(this.ExitDropItem_Click);
            // 
            // AdjToolStripMenuItem
            // 
            this.AdjToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClosureErrorDropItem,
            this.GrossErrorDropItem,
            this.ConstraintNetworkDropItem,
            this.RankDefectNetworkDropItem});
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
            // ReportToolStripMenuItem
            // 
            this.ReportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.COSADropItem,
            this.HandbookDropItem});
            this.ReportToolStripMenuItem.Name = "ReportToolStripMenuItem";
            this.ReportToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.ReportToolStripMenuItem.Text = "报表";
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
            this.AboutDropItem,
            this.使用说明ToolStripMenuItem});
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
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
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
            this.menuStrip1.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.menuStrip1_ItemAdded);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripButton,
            this.打开OToolStripButton,
            this.toolStripSeparator,
            this.帮助LToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1179, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // 新建NToolStripButton
            // 
            this.新建NToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.新建NToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("新建NToolStripButton.Image")));
            this.新建NToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新建NToolStripButton.Name = "新建NToolStripButton";
            this.新建NToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.新建NToolStripButton.Text = "新建(&N)";
            this.新建NToolStripButton.Click += new System.EventHandler(this.新建NToolStripButton_Click);
            // 
            // 打开OToolStripButton
            // 
            this.打开OToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打开OToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripButton.Image")));
            this.打开OToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripButton.Name = "打开OToolStripButton";
            this.打开OToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.打开OToolStripButton.Text = "打开(&O)";
            this.打开OToolStripButton.Click += new System.EventHandler(this.打开OToolStripButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // 帮助LToolStripButton
            // 
            this.帮助LToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.帮助LToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("帮助LToolStripButton.Image")));
            this.帮助LToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.帮助LToolStripButton.Name = "帮助LToolStripButton";
            this.帮助LToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.帮助LToolStripButton.Text = "帮助(&L)";
            this.帮助LToolStripButton.Click += new System.EventHandler(this.帮助LToolStripButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.ItemSize = new System.Drawing.Size(140, 20);
            this.tabControl1.Location = new System.Drawing.Point(0, 50);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(1179, 24);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 6;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDoubleClick_1);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1171, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(177, 6);
            // 
            // 使用说明ToolStripMenuItem
            // 
            this.使用说明ToolStripMenuItem.Name = "使用说明ToolStripMenuItem";
            this.使用说明ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.使用说明ToolStripMenuItem.Text = "使用说明";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1179, 685);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "水准网平差程序";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_close;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NewDropItem;
        private System.Windows.Forms.ToolStripMenuItem ClearDropItem;
        private System.Windows.Forms.ToolStripMenuItem ExitDropItem;
        private System.Windows.Forms.ToolStripMenuItem AdjToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClosureErrorDropItem;
        private System.Windows.Forms.ToolStripMenuItem GrossErrorDropItem;
        private System.Windows.Forms.ToolStripMenuItem ConstraintNetworkDropItem;
        private System.Windows.Forms.ToolStripMenuItem RankDefectNetworkDropItem;
        private System.Windows.Forms.ToolStripMenuItem ReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem COSADropItem;
        private System.Windows.Forms.ToolStripMenuItem DisPower;
        private System.Windows.Forms.ToolStripMenuItem StationPower;
        private System.Windows.Forms.ToolStripMenuItem HandbookDropItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutDropItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton 新建NToolStripButton;
        private System.Windows.Forms.ToolStripButton 打开OToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton 帮助LToolStripButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_open;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_read;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem 使用说明ToolStripMenuItem;
    }
}

