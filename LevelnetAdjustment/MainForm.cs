﻿using LevelnetAdjustment.form;
using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using Newtonsoft.Json;
using SplashScreenDemo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace LevelnetAdjustment {
    public partial class MainForm : Form {

        // 输出文件格式
        public readonly string split = new string('-', 80);
        public readonly string space = new string(' ', 30);

        public ClevelingAdjust ClAdj { get; set; }

        public ProjectInfo Project { get; set; }

        public static bool flag = true;

        public bool IsImport { get; set; } = false;

        public string StartProj { get; set; }



        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm(string path) {
            InitializeComponent();
            this.DoubleBuffered = true;//设置本窗体
            this.StartProj = path;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();
        }
        private void MainForm_Load(object sender, EventArgs e) {
            tabControl1.TabPages.Clear();
            tabControl1.Visible = false;    // 没有元素的时候隐藏自己

            //修改其显示为当前时间
            this.toolStripStatusLabel3.Text = "系统当前时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            //对timer1进行相关设置
            this.timer1.Interval = 1000;
            this.timer1.Start();

            //设置菜单图标
            menuStrip1.RenderMode = ToolStripRenderMode.Professional;
            NewDropItem.Image = Properties.Resources._new;
            toolStripMenuItem_open.Image = Properties.Resources.open;
            toolStripMenuItem_read.Image = Properties.Resources.import2;
            ExitDropItem.Image = Properties.Resources.close2;
            COSADropItem.Image = Properties.Resources.TXT;
            HandbookDropItem.Image = Properties.Resources.excel_01;
            AboutDropItem.Image = Properties.Resources.about2;
            使用说明ToolStripMenuItem.Image = Properties.Resources.帮助中心编辑;


            //部分按钮禁用
            ClosureErrorDropItem.Enabled = false;
            GrossErrorDropItem.Enabled = false;
            ConstraintNetworkDropItem.Enabled = false;
            RankDefectNetworkDropItem.Enabled = false;
            COSADropItem.Enabled = false;
            HandbookDropItem.Enabled = false;
            toolStripMenuItem_read.Enabled = false;

            //添加最近打开的项目
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            if (settings.Count != 0) {
                ToolStripMenuItem terMenu;
                foreach (var key in ConfigurationManager.AppSettings.AllKeys) {
                    terMenu = new ToolStripMenuItem {
                        Name = key,
                        Text = settings[key].Value
                    };
                    //注册事件
                    terMenu.Click += new EventHandler(terMenu_Click);
                    ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Add(terMenu);
                };
            }

            this.BackgroundImage = Properties.Resources.backgroundimage;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            if (StartProj != "") {
                OpenProj(StartProj);
            }
        }

        /// <summary>
        /// 动态更新菜单栏
        /// </summary>
        /// <param name="value"></param>
        public void UpDateMenu(string value) {
            string key = ConfigHelper.AddAppSetting(value);
            if (key == "") {
                return;
            }
            ToolStripMenuItem terMenu = new ToolStripMenuItem {
                Name = key,
                Text = value
            };
            //注册事件
            terMenu.Click += new EventHandler(terMenu_Click);
            ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"])
                .DropDownItems.Add(terMenu);
        }

        /// <summary>
        /// 动态添加子菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void terMenu_Click(object sender, EventArgs e) {
            ToolStripMenuItem downItem = sender as ToolStripMenuItem;
            string path = Path.Combine(downItem.Text, Path.GetFileName(downItem.Text) + ".laproj");
            if (!File.Exists(path)) {
                MessageBox.Show("该项目已被删除！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfigHelper.DeleteAppSettings(downItem.Name);
                ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Remove(downItem);
                return;
            }

            OpenProj(path);
        }


        #region 文件管理
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_read_Click(object sender, EventArgs e) {
            ReadData rd = new ReadData(ClAdj, Project);
            rd.TransfEvent += ChangeImportState;
            rd.ShowDialog();
        }

        void ChangeImportState(Option options) {
            this.IsImport = true;
            this.ClAdj.Options = options;
            this.Project.Options = options;
        }

        /// <summary>
        /// 关闭所有的子窗体
        /// </summary>
        void ClearForms() {
            FormCollection childCollection = Application.OpenForms;
            for (int i = childCollection.Count; i-- > 0;) {
                if (childCollection[i].Name != this.Name) childCollection[i].Close();
            }
            tabControl1.TabPages.Clear();
            tabControl1.Visible = false;    // 没有元素的时候隐藏自己
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewDropItem_Click(object sender, EventArgs e) {
            CreatePrj prj = new CreatePrj(Project);
            prj.TransfEvent += UpDateProject;
            prj.ShowDialog();
        }

        /// <summary>
        /// 更新项目信息
        /// </summary>
        /// <param name="project"></param>
        public void UpDateProject(ProjectInfo project) {
            this.ClAdj = new ClevelingAdjust();
            this.Project = project;
            ClAdj.Options = project.Options;
            var projname = Path.Combine(project.Path, project.Name);
            UpDateMenu(projname);
            toolStripStatusLabel2.Text = "当前项目：" + projname;
            ClearForms();
        }

        /// <summary>
        /// 找到已打开文件tab索引
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int FindIndexFromTabControl(string value) {
            var pages = tabControl1.TabPages;
            for (int i = 0; i < pages.Count; i++) {
                if (pages[i].Text == Path.GetFileName(value)) {
                    //(tabControl1.TabPages[i].Tag as Form).Focus();
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 打开项目
        /// </summary>
        void OpenProj(string projname) {
            ClAdj = new ClevelingAdjust();
            UpDateMenu(Path.GetDirectoryName(projname));
            ClearForms();
            // 读取文件信息
            this.Project = JsonHelper.ReadJson(projname);
            this.ClAdj.Options = Project.Options;
            this.ClAdj.RawDatas = Project.RawDatas;
            this.ClAdj.ObservedDatas = Project.ObservedDatas;
            this.ClAdj.KnownPoints = Project.KnownPoints;
            this.ClAdj.StablePoints = Project.StablePoints;
            this.ClAdj.ObservedDatasNoRep = Calc.RemoveDuplicates(ClAdj.ObservedDatas);//去除重复边
            // 获取所有文件
            FileInfo[] files = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(projname), "ExportFiles")).GetFiles();
            foreach (var file in files) {
                if (!file.Extension.ToLower().Contains("ou")) {
                    continue;
                }
                AddTabPage(file.FullName);  // 新建窗体同时新建一个标签
            }
            toolStripStatusLabel2.Text = "当前项目：" + Path.GetDirectoryName(projname);
            MessageBox.Show("打开成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// 触发打开项目事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_choose_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = false,
                Title = "打开",
                Filter = "项目文件|*.laproj",
                FilterIndex = 1,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                OpenProj(openFile.FileName);
            }
        }

        /// <summary>
        /// 触发保存项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e) {
            if (Project == null) {
                return;
            }
            SaveProject();
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 保存项目
        /// </summary>
        private void SaveProject() {
            //没有导入文件不需要保存
            if (!IsImport) {
                return;
            }
            Project.StablePoints = ClAdj.StablePoints;
            Project.Options = ClAdj.Options;
            Project.RawDatas = ClAdj.RawDatas;
            Project.ObservedDatas = ClAdj.ObservedDatas;
            Project.KnownPoints = ClAdj.KnownPoints;
            JsonHelper.WriteJson(Project);
        }

        /// <summary>
        /// 清空最近打开的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_clear_Click(object sender, EventArgs e) {
            ConfigHelper.DelAllSettings();
            while (((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Count > 2) {
                ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.RemoveAt(((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Count - 1);
            }
        }

        #endregion

        #region 数据处理
        /// <summary>
        /// 约束网平差按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelnetDropItem_Click(object sender, EventArgs e) {
            if (ClAdj.ObservedDatas == null) {
                throw new Exception("请打开观测文件");
            }
            if (File.Exists(Project.Options.OutputFiles.OutpathAdj)) {
                if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {

                    AddTabPage(Project.Options.OutputFiles.OutpathAdj);  // 新建窗体同时新建一个标签
                    return;
                }
            }

            SimpleLoading loadingfrm = new SimpleLoading(this, "约束网平差中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                int i = ClAdj.LS_Adjustment();
                ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdj, split, space, "约束网");
                loading.CloseWaitForm();
                AddTabPage(Project.Options.OutputFiles.OutpathAdj);  // 新建窗体同时新建一个标签
                MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }

        }

        /// <summary>
        /// 秩亏网平差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RankDefectNetworkDropItem_Click(object sender, EventArgs e) {
            if (ClAdj.ObservedDatas == null) {
                throw new Exception("请打开观测文件");
            }
            if (File.Exists(Project.Options.OutputFiles.OutpathAdjFree)) {
                if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    AddTabPage(Project.Options.OutputFiles.OutpathAdjFree);  // 新建窗体同时新建一个标签
                    return;
                }
            }

            var i = 0;

            if (MessageBox.Show("是否导入拟稳点点号？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                // 拟稳平差
                OpenFileDialog openFile = new OpenFileDialog {
                    Multiselect = false,
                    Title = "打开",
                    Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*",
                    FilterIndex = 1,
                };
                if (openFile.ShowDialog() == DialogResult.OK) {
                    SimpleLoading loadingfrm = new SimpleLoading(this, "拟稳平差中，请稍等...");
                    //将Loaing窗口，注入到 SplashScreenManager 来管理
                    GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                    loading.ShowLoading();
                    try {
                        ClAdj.CalcApproximateHeight();
                        FileHelper.ReadStablePoint(openFile.FileName, ClAdj.StablePoints, ClAdj.UnknownPoints);
                        i = ClAdj.QuasiStable();
                        ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdjFree, split, space, "拟稳");
                        loading.CloseWaitForm();
                    }
                    catch (Exception ex) {
                        loading.CloseWaitForm();
                        throw ex;
                    }
                }
                else {
                    return;
                }
            }
            else {
                // 自由网平差
                SimpleLoading loadingfrm = new SimpleLoading(this, "自由网平差中，请稍等...");
                //将Loaing窗口，注入到 SplashScreenManager 来管理
                GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                loading.ShowLoading();
                try {
                    i = ClAdj.FreeNetAdjust();
                    ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdjFree, split, space, "自由网");
                    loading.CloseWaitForm();
                }
                catch (Exception ex) {
                    loading.CloseWaitForm();
                    throw ex;
                }
            }
            AddTabPage(Project.Options.OutputFiles.OutpathAdjFree);  // 新建窗体同时新建一个标签
            MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 计算闭合差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosureErrorDropItem_Click(object sender, EventArgs e) {
            if (File.Exists(Project.Options.OutputFiles.OutpathClosure)) {
                if (MessageBox.Show("闭合差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    AddTabPage(Project.Options.OutputFiles.OutpathClosure);  // 新建窗体同时新建一个标签
                    return;
                }
            }
            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                ClAdj.CalcClosureError(Project.Options.OutputFiles.OutpathClosure, split, space);
                loading.CloseWaitForm();

                AddTabPage(Project.Options.OutputFiles.OutpathClosure);  // 新建窗体同时新建一个标签
                MessageBox.Show("闭合差计算完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }

        }

        /// <summary>
        /// 粗差探测按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrossErrorDropItem_Click(object sender, EventArgs e) {
            if (File.Exists(Project.Options.OutputFiles.OutpathGrossError)) {
                if (MessageBox.Show("粗差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    AddTabPage(Project.Options.OutputFiles.OutpathGrossError);  // 新建窗体同时新建一个标签
                    return;
                }
            }
            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();

            try {
                ClAdj.FindGrossError(split, space, Project.Options.OutputFiles.OutpathGrossError);
                loading.CloseWaitForm();

                AddTabPage(Project.Options.OutputFiles.OutpathGrossError);  // 新建窗体同时新建一个标签
                MessageBox.Show("粗差探测完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }
        }

        #endregion

        #region 导出报表
        /// <summary>
        /// 导出观测手簿
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandbookDropItem_Click(object sender, EventArgs e) {
            if (!File.Exists(Project.Options.OutputFiles.Handbook)) {
                SimpleLoading loadingfrm = new SimpleLoading(this, "导出中，请稍等...");
                //将Loaing窗口，注入到 SplashScreenManager 来管理
                GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                loading.ShowLoading();
                try {
                    ExceHelperl.ExportHandbook(ClAdj.RawDatas, ClAdj.ObservedDatas, Project.Options.OutputFiles.Handbook, ClAdj.Options.ImportFiles);
                    loading.CloseWaitForm();
                    if (MessageBox.Show("导出成功，是否查看？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(Project.Options.OutputFiles.Handbook);
                }
                catch (Exception ex) {
                    loading.CloseWaitForm();
                    throw ex;
                }
            }
            else {
                System.Diagnostics.Process.Start(Project.Options.OutputFiles.Handbook);
            }
        }

        /// <summary>
        /// 导出COSA按距离定权 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisPower_Click(object sender, EventArgs e) {

            FileHelper.ExportCOSA(ClAdj.ObservedDatas, ClAdj.KnownPoints, Project.Options.OutputFiles.COSADis);
            MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AddTabPage(Project.Options.OutputFiles.COSADis);  // 新建窗体同时新建一个标签
        }

        /// <summary>
        /// 导出COSA按测站数定权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationPower_Click(object sender, EventArgs e) {
            FileHelper.ExportCOSAStationPower(ClAdj.ObservedDatas, ClAdj.KnownPoints, Project.Options.OutputFiles.COSASta);

            AddTabPage(Project.Options.OutputFiles.COSASta);  // 新建窗体同时新建一个标签
            MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        #endregion

        #region 设置多标签页面
        /// <summary>
        /// 添加一个标签
        /// </summary>
        /// <param name="frm"></param>
        private void AddTabPage(string filename) {
            var idx = FindIndexFromTabControl(filename);
            if (idx != -1) {
                CloseTabPage(idx);
            }
            FileView fv = new FileView(filename) {
                MdiParent = this,//WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                ShowInTaskbar = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            fv.Show();
            TabPage tp = new TabPage {
                Tag = fv,  // 当前标签控制的窗体对象记录在Tag属性中
                Text = fv.Text,
                ToolTipText = fv.Text
            };
            tabControl1.TabPages.Add(tp);
            tabControl1.SelectedIndex = tabControl1.TabCount - 1;  // 默认选中最后一个新建的标签
            if (!tabControl1.Visible) tabControl1.Visible = true;  // 如果自己是隐藏的则显示自己
        }

        private void menuStrip1_ItemAdded(object sender, ToolStripItemEventArgs e) {
            /*if (e.Item.Text.Length == 0 || e.Item.Text == "还原(&R)" || e.Item.Text == "最小化(&N)" || e.Item.Text == "关闭(&C)") {
                e.Item.Visible = false;
            }*/
        }

        /// <summary>
        /// 点击标签切换页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            if (tabControl1.SelectedIndex > -1)
                (tabControl1.TabPages[tabControl1.SelectedIndex].Tag as Form).Focus();
            else {
                this.BackgroundImage = Properties.Resources.backgroundimage;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        /// <summary>
        /// 删除一个标签
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void CloseTabPage(int selectedIndex) {
            (tabControl1.TabPages[selectedIndex].Tag as Form).Close();
            tabControl1.TabPages.RemoveAt(selectedIndex);
            if (tabControl1.TabPages.Count == 0) tabControl1.Visible = false;
        }

        private void tabControl1_MouseDoubleClick_1(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) // 只有左键双击才响应关闭
                CloseTabPage(tabControl1.SelectedIndex);
        }

        /// <summary>
        /// 关闭当前页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_close_Click(object sender, EventArgs e) {
            int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);  // 这里也需要通过弹出菜单的位置来得到当前是哪个项弹出的，注意菜单位置是针对屏幕左边的距离
            CloseTabPage(index);
        }
        /// <summary>
        /// 关闭其他页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_closeothers_Click(object sender, EventArgs e) {
            int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);
            for (int i = tabControl1.TabPages.Count - 1; i >= 1; i--) {
                CloseTabPage(i);
            }
            for (int i = tabControl1.TabPages.Count - 2; i >= 0; i--) {
                CloseTabPage(i);
            }

        }
        /// <summary>
        /// 关闭右侧页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_closeright_Click(object sender, EventArgs e) {
            int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);
            for (int i = tabControl1.TabPages.Count - 1; i > index; i--) {
                CloseTabPage(i);
            }
        }
        /// <summary>
        /// 关闭所有页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_closeall_Click(object sender, EventArgs e) {
            for (int i = tabControl1.TabPages.Count - 1; i >= 0; i--) {
                CloseTabPage(i);
            }
        }

        /// <summary>
        /// 在文件夹中打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 在文件夹中打开ToolStripMenuItem_Click(object sender, EventArgs e) {
            int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);
            string path = (tabControl1.TabPages[index].Tag as FileView).FilePath;
            Process.Start("Explorer", "/select," + path);
        }

        /// <summary>
        /// 从菜单弹出位置得到当前所在的标签索引
        /// </summary>
        /// <returns></returns>
        private int GetPageIndexWidthPoint(int pointX) {
            int x = 0;
            for (int i = 0; i < tabControl1.TabPages.Count; ++i) {
                if (pointX >= x && pointX <= x + tabControl1.ItemSize.Width)
                    return i;
                x += tabControl1.ItemSize.Width;
            }
            return tabControl1.TabPages.Count - 1;
        }
        #endregion


        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitDropItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        /// <summary>
        /// 确认关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (Project != null) {
                SaveProject();
            }
            DialogResult result = MessageBox.Show("您确定要关闭软件吗？", "退出提示",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.ExitThread();
            else {
                e.Cancel = true;
            }
        }


        /// <summary>
        /// 打开关于窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutDropItem_Click(object sender, EventArgs e) {
            About about = new About();
            about.ShowDialog();
        }

        private void 新建NToolStripButton_Click(object sender, EventArgs e) {
            NewDropItem_Click(sender, e);
        }

        private void 打开OToolStripButton_Click(object sender, EventArgs e) {
            toolStripMenuItem_choose_Click(sender, e);
        }

        private void 帮助LToolStripButton_Click(object sender, EventArgs e) {
            使用说明ToolStripMenuItem_Click(sender, e);
        }

        private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e) {
            var helpFile = Path.Combine(Application.StartupPath, "Readme.doc");
            if (!File.Exists(helpFile)) {
                ByteHelper.WriteByteToFile(Properties.Resources.Readme, helpFile);
            }
            Process.Start(helpFile);
        }
        private void timer1_Tick(object sender, EventArgs e) {
            this.toolStripStatusLabel3.Text = "系统当前时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e) {

            this.BackgroundImage = null;

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            if (ClAdj == null) {
                return;
            }
            if (ClAdj.ObservedDatas.Count == 0) {
                ClosureErrorDropItem.Enabled = false;
                GrossErrorDropItem.Enabled = false;
                ConstraintNetworkDropItem.Enabled = false;
                RankDefectNetworkDropItem.Enabled = false;
            }
            else {
                ClosureErrorDropItem.Enabled = true;
                GrossErrorDropItem.Enabled = true;
                ConstraintNetworkDropItem.Enabled = true;
                RankDefectNetworkDropItem.Enabled = true;
            }
            if (ClAdj.RawDatas.Count == 0) {
                COSADropItem.Enabled = false;
                HandbookDropItem.Enabled = false;
            }
            else {
                COSADropItem.Enabled = true;
                HandbookDropItem.Enabled = true;
            }

            if (Project == null) {
                toolStripMenuItem_read.Enabled = false;
            }
            else {
                toolStripMenuItem_read.Enabled = true;
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Link;
            }
            else {
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e) {
            string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (path.EndsWith(".laproj")) {
                OpenProj(path);
            }
        }





        string UpdateUrl = "http://localhost:7003/checkversion";//检测版本更新地址
        string ExeUrl = "http://localhost:7003/download";//下载EXE的地址
        string ExeName = "LNADJ";//程序名

        public void Updatenow() {
            try {
                Configuration config = ConfigurationManager.OpenExeConfiguration(@"" + ExeName + ".exe"); // 写的是应用程序的路径
            }
            catch {
                MessageBox.Show("当前目录找不到主程序" + ExeName);
                Close();
                return;
            }
            string retdata = HttpGet(UpdateUrl);

            LNADJ msg = JsonConvert.DeserializeObject<LNADJ>(retdata, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });

            if (msg.Update)//需要更新
            {
                DialogResult dr = MessageBox.Show("检测到新版本：" + msg.Version + "更新内容：" + msg.Remarks + "，当前版本：" + versionname + ",是否更新", "操作提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == dr) {
                    try {
                        System.Net.WebClient client = new System.Net.WebClient();
                        byte[] data = client.DownloadData(ExeUrl + updateBean.ID + "/" + ExeName + ".exe");//下载EXE程序

                        System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();//把之前的程序关闭
                        foreach (System.Diagnostics.Process p in ps) {
                            //MessageBox.Show(p.ProcessName);
                            if (p.ProcessName == ExeName || p.ProcessName == (ExeName + ".vshost")) {
                                p.Kill();
                                break;
                            }
                        }

                        string path = Application.StartupPath;
                        FileStream fs = new FileStream(path + "\\" + ExeName + ".exe", FileMode.Create);
                        //将byte数组写入文件中
                        fs.Write(data, 0, data.Length);
                        fs.Close();
                        MessageBox.Show("更新成功");

                        SaveConfig("VersionID", updateBean.ID.ToString());
                        SaveConfig("VersionName", updateBean.Version);
                    }
                    catch (Exception ex) {
                        MessageBox.Show("更新失败：" + ex.Message);
                    }
                }
            }
            Close();
            System.Diagnostics.Process.Start(Application.StartupPath + "\\" + ExeName + ".exe");

        }
        public string LoadConfig(string content) {
            Configuration config = ConfigurationManager.OpenExeConfiguration(@"" + ExeName + ".exe"); // 写的是应用程序的路径
            try {
                return config.AppSettings.Settings[content].Value;
            }
            catch {
                return "";
            }

        }
        public void SaveConfig(string content, string value) {
            Configuration config = ConfigurationManager.OpenExeConfiguration(@"" + ExeName + ".exe"); // 写的是应用程序的路径
            try {
                config.AppSettings.Settings[content].Value = value;
            }
            catch {
                config.AppSettings.Settings.Add(content, value);
            }
            config.Save(System.Configuration.ConfigurationSaveMode.Minimal);
        }
        public static string HttpGet(string url) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            try {
                response = request.GetResponse();
                if (response != null) {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception) {
                MessageBox.Show("请检查当前网络或者链接路径");
            }
            finally {
                request = null;
                response = null;
            }
            return responseStr;
        }
        public partial class LNADJ {
            public int ID { get; set; }
            public string Version { get; set; }
            public string Remarks { get; set; }
            public bool Update { get; set; }

        }







    }
}

