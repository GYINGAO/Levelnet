using LevelnetAdjustment.form;
using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using SplashScreenDemo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace LevelnetAdjustment {
    public partial class MainForm : Form {

        // 输出文件格式
        public readonly string split = new string('-', 80);
        public readonly string space = new string(' ', 30);

        public ClevelingAdjust ClAdj { get; set; }

        public ProjectInfo Project { get; set; }

        public static bool flag = true;



        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm() {
            InitializeComponent();
            this.DoubleBuffered = true;//设置本窗体

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

            //添加最近打开的项目
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            if (settings.Count == 0) {
                return;
            }
            ToolStripMenuItem terMenu;
            foreach (var key in ConfigurationManager.AppSettings.AllKeys) {
                terMenu = new ToolStripMenuItem {
                    Name = key,
                    Text = settings[key].Value
                };
                //注册事件
                terMenu.Click += new EventHandler(terMenu_Click);
                ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Add(terMenu);
            }

            this.BackgroundImage = Properties.Resources.backgroundimage;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            //部分按钮禁用
            ClosureErrorDropItem.Enabled = false;
            GrossErrorDropItem.Enabled = false;
            ConstraintNetworkDropItem.Enabled = false;
            RankDefectNetworkDropItem.Enabled = false;
            COSADropItem.Enabled = false;
            HandbookDropItem.Enabled = false;

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
            rd.ShowDialog();
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

        public void UpDateProject(ProjectInfo project) {
            this.ClAdj = new ClevelingAdjust();
            this.Project = project;
            UpDateMenu(Path.Combine(project.Path, project.Name));
            FormCollection childCollection = Application.OpenForms;
            for (int i = childCollection.Count; i-- > 0;) {
                if (childCollection[i].Name != this.Name) childCollection[i].Close();
            }
            tabControl1.TabPages.Clear();
            tabControl1.Visible = false;    // 没有元素的时候隐藏自己
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
            // 读取文件信息
            this.Project = JsonHelper.ReadJson(projname);
            this.ClAdj.Options = Project.Options;
            this.ClAdj.RawDatas = Project.RawDatas;
            this.ClAdj.ObservedDatas = Project.ObservedDatas;
            this.ClAdj.KnownPoints = Project.KnownPoints;
            this.ClAdj.StablePoints = Project.StablePoints;
            // 获取所有文件
            FileInfo[] files = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(projname), "ExportFiles")).GetFiles();
            foreach (var file in files) {
                FileView fv = new FileView(file.FullName) {
                    MdiParent = this,//WindowState = FormWindowState.Maximized,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    Dock = DockStyle.Fill,
                    FormBorderStyle = FormBorderStyle.None,
                };
                fv.Show();
                AddTabPage(fv);  // 新建窗体同时新建一个标签
            }
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

                FileView fileView = new FileView(Project.Options.OutputFiles.OutpathAdj) {
                    MdiParent = this,
                    //WindowState = FormWindowState.Maximized,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    Dock = DockStyle.Fill,
                    FormBorderStyle = FormBorderStyle.None,
                };
                MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fileView.Show();
                AddTabPage(fileView);  // 新建窗体同时新建一个标签
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
            FileView fileView = new FileView(Project.Options.OutputFiles.OutpathAdjFree) {
                MdiParent = this,
                //WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                ShowInTaskbar = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
            AddTabPage(fileView);  // 新建窗体同时新建一个标签
        }

        /// <summary>
        /// 计算闭合差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosureErrorDropItem_Click(object sender, EventArgs e) {
            if (File.Exists(ClAdj.Options.OutputFiles.OutpathClosure)) {
                if (MessageBox.Show("闭合差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
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

                FileView fileView = new FileView(Project.Options.OutputFiles.OutpathClosure) {
                    MdiParent = this,
                    //WindowState = FormWindowState.Maximized,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    Dock = DockStyle.Fill,
                    FormBorderStyle = FormBorderStyle.None,
                };
                MessageBox.Show("闭合差计算完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fileView.Show();
                AddTabPage(fileView);  // 新建窗体同时新建一个标签
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
            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();

            try {
                ClAdj.FindGrossError(split, space, Project.Options.OutputFiles.OutpathGrossError);
                loading.CloseWaitForm();

                FileView fileView = new FileView(Project.Options.OutputFiles.OutpathGrossError) {
                    MdiParent = this,
                    //WindowState = FormWindowState.Maximized,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    Dock = DockStyle.Fill,
                    FormBorderStyle = FormBorderStyle.None,
                };
                MessageBox.Show("粗差探测完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fileView.Show();
                AddTabPage(fileView);  // 新建窗体同时新建一个标签
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

            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "Excel 工作簿(*.xlsx)|*.xlsx|Excel 97-2003 工作簿(*.xls)|*.xls",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = "观测手簿",
                InitialDirectory = Path.Combine(Project.Path, Project.Name, "ExportFiles"),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                SimpleLoading loadingfrm = new SimpleLoading(this, "导出中，请稍等...");
                //将Loaing窗口，注入到 SplashScreenManager 来管理
                GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                loading.ShowLoading();
                try {
                    ExceHelperl.ExportHandbook(ClAdj.RawDatas, ClAdj.ObservedDatas, saveFileDialog.FileName, ClAdj.Options.FileList);
                    loading.CloseWaitForm();
                }
                catch (Exception ex) {
                    loading.CloseWaitForm();
                    throw ex;
                }

                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 导出COSA按距离定权 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisPower_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "COSA水准观测文件(*.in1)|*.in1",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = "按距离定权",
                InitialDirectory = Path.Combine(Project.Path, Project.Name, "ExportFiles"),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                FileHelper.ExportCOSA(ClAdj.ObservedDatas, ClAdj.KnownPoints, saveFileDialog.FileName);
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 导出COSA按测站数定权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationPower_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "COSA水准观测文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = "按测站数定权",
                InitialDirectory = Path.Combine(Project.Path, Project.Name, "ExportFiles"),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                FileHelper.ExportCOSAStationPower(ClAdj.ObservedDatas, ClAdj.KnownPoints, saveFileDialog.FileName);
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region 设置多标签页面
        /// <summary>
        /// 添加一个标签
        /// </summary>
        /// <param name="frm"></param>
        private void AddTabPage(Form frm) {
            TabPage tp = new TabPage();
            tp.Tag = frm;  // 当前标签控制的窗体对象记录在Tag属性中
            tp.Text = frm.Text;
            tp.ToolTipText = frm.Text;
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
            var helpFile = Path.Combine(Application.StartupPath, "help.chm");
            if (!File.Exists(helpFile)) {
                ByteHelper.WriteByteToFile(Properties.Resources.help, helpFile);
            }
            System.Diagnostics.Process.Start(helpFile);
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
        }





    }
}

