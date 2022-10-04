
using LevelnetAdjustment.form;
using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using Newtonsoft.Json;
using SplashScreenDemo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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

        public string StartProj { get; set; } = "";

        public bool DirectExit { get; set; } = false;



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
            ExitDropItem.Image = Properties.Resources.close2;
            生成平差文件ToolStripMenuItem.Image = Properties.Resources.TXT;
            生成观测手簿ToolStripMenuItem.Image = Properties.Resources.excel_01;
            AboutDropItem.Image = Properties.Resources.about2;
            使用说明ToolStripMenuItem.Image = Properties.Resources.帮助中心编辑;
            设置处理参数ToolStripMenuItem.Image = Properties.Resources.设置;
            导入观测文件ToolStripMenuItem.Image = Properties.Resources.import11;
            导入已知点ToolStripMenuItem.Image = Properties.Resources.坐标;
            观测数据检核ToolStripMenuItem.Image = Properties.Resources.辅助检查;
            选择平差文件ToolStripMenuItem.Image = Properties.Resources.选择文件;
            往返测高差较差ToolStripMenuItem.Image = Properties.Resources.比较图;
            ClosureErrorDropItem.Image = Properties.Resources.班级圈;
            GrossErrorDropItem.Image = Properties.Resources.搜索;
            ConstraintNetworkDropItem.Image = Properties.Resources.约束;
            RankDefectNetworkDropItem.Image = Properties.Resources.自由;
            检查更新ToolStripMenuItem.Image = Properties.Resources.更新2;



            //部分按钮禁用
            水准仪数据预处理ToolStripMenuItem.Enabled = false;
            AdjToolStripMenuItem.Enabled = false;


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
            Update();
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
        /// 设置与选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>





        /// <summary>
        /// 关闭所有的子窗体
        /// </summary>
        void ClearForms() {
            FormCollection childCollection = Application.OpenForms;
            for (int i = childCollection.Count; i-- > 0;) {
                if (childCollection[i].Name != Name) childCollection[i].Close();
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
            水准仪数据预处理ToolStripMenuItem.Enabled = true;
            AdjToolStripMenuItem.Enabled = true;

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
            if (Project != null) {
                if (toolStripStatusLabel2.Text == "当前项目：" + Path.GetDirectoryName(projname)) {
                    MessageBox.Show("打开了相同的项目！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // 打开项目保存上一个项目
                else {
                    SaveProject();
                }
            }
            ClAdj = new ClevelingAdjust();
            UpDateMenu(Path.GetDirectoryName(projname));
            ClearForms();
            // 读取文件信息
            this.Project = JsonHelper.ReadJson(projname);
            this.ClAdj.Options = Project.Options;
            this.ClAdj.RawDatas = Project.RawDatas;
            this.ClAdj.ObservedDatas = Project.ObservedDatas;
            this.ClAdj.KnownPoints = Project.KnownPoints;
            this.ClAdj.UnknownPoints = Project.UnknownPoints;
            this.ClAdj.Calc_Params();
            ClAdj.AllPoints = Commom.Merge(ClAdj.KnownPointEable, ClAdj.UnknownPoints);
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
            水准仪数据预处理ToolStripMenuItem.Enabled = true;
            AdjToolStripMenuItem.Enabled = true;
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
            /* if (!IsImport) {
                 return;
             }*/
            Project.UnknownPoints = ClAdj.UnknownPoints;
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
            if (ClAdj.ObservedDatasNoRep == null) {
                throw new Exception("请选择平差文件");
            }
            if (File.Exists(Project.Options.OutputFiles.OutpathAdj)) {
                if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {

                    AddTabPage(Project.Options.OutputFiles.OutpathAdj);  // 新建窗体同时新建一个标签
                    return;
                }
            }

            ChooseKnownPoint chooseStablePoint = new ChooseKnownPoint(ClAdj.KnownPoints);
            chooseStablePoint.TransfChangeKnownPoint += CalcLS;
            chooseStablePoint.ShowDialog();
        }

        void CalcLS(List<PointData> Points) {
            ClAdj.KnownPoints = Points;
            ClAdj.CalcApproximateHeight(true);
            ClAdj.AllPoints = Commom.Merge(ClAdj.KnownPointEable, ClAdj.UnknownPoints);
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
            if (ClAdj.ObservedDatasNoRep == null) {
                throw new Exception("请选择平差文件");
            }
            if (File.Exists(Project.Options.OutputFiles.OutpathAdjFree)) {
                if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    AddTabPage(Project.Options.OutputFiles.OutpathAdjFree);  // 新建窗体同时新建一个标签
                    return;
                }
            }
            ClAdj.CalcApproximateHeight(false);
            ChooseStablePoint chooseStablePoint = new ChooseStablePoint(ClAdj.UnknownPoints);
            chooseStablePoint.TransfChangeStable += CalcStable;
            chooseStablePoint.ShowDialog();
        }

        private void CalcStable(List<PointData> Points) {
            ClAdj.UnknownPoints = Points;
            ClAdj.AllPoints = Commom.Merge(ClAdj.KnownPointEable, Points);
            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                var i = 0;
                // 有拟稳点
                if (ClAdj.UnknownPoints.FindIndex(p => p.IsStable == true) != -1) {
                    i = ClAdj.QuasiStable();
                    ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdjFree, split, space, "拟稳");
                }
                // 无拟稳点
                else {
                    i = ClAdj.FreeNetAdjust();
                    ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdjFree, split, space, "自由网");
                }

                loading.CloseWaitForm();
                AddTabPage(Project.Options.OutputFiles.OutpathAdjFree);  // 新建窗体同时新建一个标签
                MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }
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

            ChooseKnownPoint chooseStablePoint = new ChooseKnownPoint(ClAdj.KnownPoints);
            chooseStablePoint.TransfChangeKnownPoint += CalcClosureError;
            chooseStablePoint.ShowDialog();
        }

        void CalcClosureError(List<PointData> Points) {
            ClAdj.KnownPoints = Points;
            ClAdj.CalcApproximateHeight(true);
            ClAdj.AllPoints = Commom.Merge(ClAdj.KnownPointEable, ClAdj.UnknownPoints);
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
            // 直接退出
            if (DirectExit) {
                return;
            }
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
            //没有新建项目
            if (ClAdj == null) {
                水准仪数据预处理ToolStripMenuItem.Enabled = false;
                AdjToolStripMenuItem.Enabled = false;
                return;
            }
            else {
                水准仪数据预处理ToolStripMenuItem.Enabled = true;
                AdjToolStripMenuItem.Enabled = true;
            }
            // 没有导入观测文件
            if (ClAdj.RawDatas?.Count == 0) {
                观测数据检核ToolStripMenuItem.Enabled = false;
                生成平差文件ToolStripMenuItem.Enabled = false;
                生成观测手簿ToolStripMenuItem.Enabled = false;
            }
            else {
                观测数据检核ToolStripMenuItem.Enabled = true;
                生成平差文件ToolStripMenuItem.Enabled = true;
                生成观测手簿ToolStripMenuItem.Enabled = true;
            }

            //没有导入平差文件
            if (ClAdj.ObservedDatas?.Count == 0) {
                往返测高差较差ToolStripMenuItem.Enabled = false;
                ClosureErrorDropItem.Enabled = false;
                GrossErrorDropItem.Enabled = false;
                ConstraintNetworkDropItem.Enabled = false;
                RankDefectNetworkDropItem.Enabled = false;
            }
            else {
                往返测高差较差ToolStripMenuItem.Enabled = true;
                ClosureErrorDropItem.Enabled = true;
                GrossErrorDropItem.Enabled = true;
                ConstraintNetworkDropItem.Enabled = true;
                RankDefectNetworkDropItem.Enabled = true;
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

        new void Update() {
            var Version = Application.ProductVersion.ToString();
            string checkURL = "http://43.142.49.203:7001/check?Version=" + Version;//检测版本更新地址

            try {

                string getJson = HttpHelper.Get(checkURL);
                Response res = JsonConvert.DeserializeObject<Response>(getJson);
                if (res.Update) {
                    DialogResult dr = MessageBox.Show("检测到新版本：" + res.LatestVersion + "\r\n当前版本：" + res.CurrentVersion + "\r\n更新内容：\r\n" + res.Remark + "\r\n\r\n是否更新?", "更新提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes) {
                        //string downloadURL = "http://43.142.49.203:7001/public/" + res.AppName;//下载EXE的地址
                        string downloadURL = "http://43.142.49.203:7001/download";//下载EXE的地址
                        Process.Start(downloadURL);
                        DirectExit = true;
                        this.Close();
                    }

                }
            }
            catch (Exception ex) { throw ex; }
        }


        public partial class Response {
            public string CurrentVersion { get; set; } = "";
            public string LatestVersion { get; set; } = "";
            public string Remark { get; set; } = "";
            public bool Update { get; set; }
            public string AppName { get; set; } = "";

        }

        private void 检查更新ToolStripMenuItem_Click(object sender, EventArgs e) {
            Update();
        }

        private void 生成观测手簿ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (File.Exists(Project.Options.OutputFiles.Handbook)) {
                if (MessageBox.Show("观测手簿已存在，是否重新导出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    Process.Start(Project.Options.OutputFiles.Handbook);
                    return;
                }
            }
            SimpleLoading loadingfrm = new SimpleLoading(this, "导出中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                ExceHelperl.ExportHandbook(ClAdj.RawDatas, Project.Options.OutputFiles.Handbook, ClAdj.Options.ImportFiles);
                loading.CloseWaitForm();
                if (MessageBox.Show("导出成功，是否查看？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    Process.Start(Project.Options.OutputFiles.Handbook);
            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }
        }

        private void 生成平差文件ToolStripMenuItem_Click(object sender, EventArgs e) {
            FrmZDSelect frmZDSelect = new FrmZDSelect();
            frmZDSelect.TransfEvevn += ChangeZD;
            frmZDSelect.ShowDialog();
        }
        void ChangeZD(string zd) {
            FileHelper.ExportIN1(ClAdj.RawDatas, ClAdj.KnownPoints, zd, Project.Options.OutputFiles.COSADis);
            MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AddTabPage(Project.Options.OutputFiles.COSADis);  // 新建窗体同时新建一个标签
        }
        private void 导入观测文件ToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "Trimble DINI|*.dat;*.DAT|Leica DNA|*.gsi;*.GSI|All files|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                SimpleLoading loadingfrm = new SimpleLoading(this, "读取中，请稍等...");
                //将Loaing窗口，注入到 SplashScreenManager 来管理
                GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                loading.ShowLoading();
                for (int i = ClAdj.Options.ImportFiles.Count - 1; i >= 0; i--) {
                    string ext = Path.GetExtension(ClAdj.Options.ImportFiles[i].FileName).ToLower();
                    if (ext.Contains("dat") || ext.Contains("gsi")) {
                        ClAdj.Options.ImportFiles.RemoveAt(i);
                    }
                }
                try {
                    var RawDatas = new List<RawData>();
                    foreach (var item in openFile.FileNames) {
                        /* if (ClAdj.Options.ImportFiles.Exists(t => t.FileName == Path.GetFileName(item))) {
                             continue;
                         }*/
                        //把文件复制到项目文件夹中
                        FileInfo fileInfo1 = new FileInfo(item);
                        string targetPath = Path.Combine(Project.Path, Project.Name, "ImportFiles", Path.GetFileName(item));
                        if (File.Exists(targetPath)) File.Delete(targetPath);
                        fileInfo1.CopyTo(targetPath);
                        ClAdj.Options.ImportFiles.Add(new InputFile {
                            FilePath = item,
                            FileName = Path.GetFileName(item)
                        });
                        switch (Path.GetExtension(item).ToLower()) {
                            case ".dat":
                                FileHelper.ReadDAT(RawDatas, item);
                                break;
                            case ".gsi":
                                FileHelper.ReadGSI(item, RawDatas);
                                break;
                            default:
                                break;
                        }
                    }
                    // ClAdj.RawDatas = ClAdj?.RawDatas.Count != 0 ? Commom.Merge(ClAdj.RawDatas, RawDatas) : RawDatas;
                    ClAdj.RawDatas = RawDatas;
                    loading.CloseWaitForm();
                }
                catch (Exception ex) {
                    loading.CloseWaitForm();
                    throw ex;
                }
            }


        }

        private void 选择平差文件ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (ClAdj.ObservedDatasNoRep.Count != 0) {
                if (MessageBox.Show("是否重新导入？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                    return;
                }
            }
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = false,
                Title = "打开",
                Filter = "in1 file|*.in1;*.IN1",
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = Path.Combine(Project.Path, Project.Name, "ExportFiles")
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                var knownPoints = new List<PointData>();
                var observedDatas = new List<ObservedData>();
                FileHelper.ReadOriginalFile(knownPoints, observedDatas, openFile.FileName);
                if (knownPoints.Count == 0) {
                    throw new Exception("缺少已知点，请检查");
                }
                ClAdj.KnownPoints = knownPoints;
                ClAdj.ObservedDatas = observedDatas;
                ClAdj.Calc_Params();
                ClAdj.AllPoints = Commom.Merge(ClAdj.KnownPointEable, ClAdj.UnknownPoints);
                string msg = "";
                int j = 0;
                for (int i = 0; i < ClAdj.KnownPoints.Count; i++) {
                    if (ClAdj.ObservedDatasNoRep.Exists(l => l.Start == ClAdj.KnownPoints[i].Number || l.End == ClAdj.KnownPoints[i].Number)) {
                        continue;
                    }
                    else {
                        j++;
                        msg += $"{j}、点号：{ClAdj.KnownPoints[i].Number}，高程：{ClAdj.KnownPoints[i].Height}\r\n";
                        ClAdj.KnownPoints[i].Enable = false;
                    }
                }
                if (msg != "") {
                    MessageBox.Show($"以下已知点在观测文件中未找到!\r\n{msg}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClAdj.Calc_Params();
                }
                else {
                    MessageBox.Show("读取成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void 设置处理参数ToolStripMenuItem_Click(object sender, EventArgs e) {
            FrmADJSetting rd = new FrmADJSetting(ClAdj.Options);
            rd.TransfEvent += ChangeLevelParams;
            rd.ShowDialog();
        }
        void ChangeLevelParams(Option option) {
            this.ClAdj.Options = option;
            this.Project.Options = option;
        }

        private void 导入已知点ToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = false,
                Title = "打开",
                Filter = "已知点文件|*.txt",
                FilterIndex = 1,
                RestoreDirectory = true,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                ClAdj.KnownPoints = new List<PointData>();
                FileHelper.ReadKnPoints(openFile.FileName, ClAdj.KnownPoints);
            }
        }

        private void 往返测高差较差ToolStripMenuItem_Click(object sender, EventArgs e) {
            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(split);
                sb.AppendLine(space + "往返测高差较差统计结果");
                sb.AppendLine(split);
                sb.AppendLine($"{"序号",-5}{"起点",-8}{"终点",-7}{"往测距离/km",-9}{"返测距离/km",-9}{"往测高差/m",-9}{"返测高差/m",-8}{"较差/mm",-8}{"限差/mm",-10}");
                double limit = ClAdj.Options.LevelParams.IsCP3 ? ClAdj.Options.LevelParams.CP3WangFan : ClAdj.Options.LevelParams.WangFan;
                int i = 0;
                double m = 0.0;
                ClAdj.ObservedDataWFs.ForEach(l => {
                    i++;
                    m += Math.Pow(l.HeightDiff_Diff, 2) / ((l.Distance_W + l.Distance_F) / 2);
                    sb.AppendLine($"{i,-7}{l.Start,-10}{l.End,-11}{l.Distance_W,-12:#0.000}{l.Distance_F,-13:#0.000}{l.HeightDiff_W,-12:#0.00000}{l.HeightDiff_F,-13:#0.00000}{l.HeightDiff_Diff,-9:#0.00}{limit,-11:#0.00}");
                });
                m = Math.Sqrt(m / (4 * i));
                sb.AppendLine(split);
                sb.AppendLine($"{space}每公里水准测量的高差偶然中误差：{m,-6:#0.000}(mm)");
                sb.AppendLine(split);
                FileHelper.WriteStrToTxt(sb.ToString(), Project.Options.OutputFiles.WFDiff);
                AddTabPage(Project.Options.OutputFiles.WFDiff);
                loading.CloseWaitForm();
            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }


        }
        private void 观测数据检核ToolStripMenuItem_Click(object sender, EventArgs e) {
            FrmCheckObsData frm = new FrmCheckObsData(ClAdj.Options.ObsDataLimits);
            frm.TransfEvent += SetObserverDataLimit;
            frm.ShowDialog();
        }
        void SetObserverDataLimit(ObsDataLimit limit) {
            ClAdj.Options.ObsDataLimits = limit;
            Project.Options.ObsDataLimits = limit;
            StringBuilder sb = new StringBuilder();
            int stationNum = 0;
            double total = 0;
            ClAdj.RawDatas.ForEach(r => {
                stationNum++;
                if (r.IsStart) {
                    sb.AppendLine($"测站   后视    前视     后尺读数1   后视距1   前尺读数1   前视距1   后尺读数2   后视距2   前尺读数2   前视距2  前后视距差 累计前后视距差");
                    sb.AppendLine("Start-Line");
                }
                double dis = r.DisDiffAve * 1000;
                total += dis;
                string backdiff1 = r.BackDiff1 < ClAdj.Options.ObsDataLimits.StafLow ? r.BackDiff1.ToString("#0.00000") + "!!!" : r.BackDiff1.ToString("#0.00000");
                string backdiff2 = r.BackDiff2 < ClAdj.Options.ObsDataLimits.StafLow ? r.BackDiff2.ToString("#0.00000") + "!!!" : r.BackDiff2.ToString("#0.00000");
                string frontdiff1 = r.FrontDiff1 < ClAdj.Options.ObsDataLimits.StafLow ? r.FrontDiff1.ToString("#0.00000") + "!!!" : r.FrontDiff1.ToString("#0.00000");
                string frontdiff2 = r.FrontDiff2 < ClAdj.Options.ObsDataLimits.StafLow ? r.FrontDiff2.ToString("#0.00000") + "!!!" : r.FrontDiff2.ToString("#0.00000");
                string disdiff = Math.Abs(dis) > ClAdj.Options.ObsDataLimits.FBDis ? dis.ToString("#0.000") + "!!!" : dis.ToString("#0.000");
                string totaldis = Math.Abs(total) > ClAdj.Options.ObsDataLimits.FBDisSum ? total.ToString("#0.000") + "!!!" : total.ToString("#0.000");
                sb.AppendLine($"{stationNum,-5}{r.BackPoint,-10}{r.FrontPoint,-10}{backdiff1,-12}{r.BackDis1 * 1000,-10:#0.000}{frontdiff1,-12}{r.FrontDis1 * 1000,-10:#0.000}{backdiff2,-12}{r.BackDis2 * 1000,-10:#0.000}{frontdiff2,-12}{r.FrontDis2 * 1000,-10:#0.000}{disdiff,-10}{totaldis,-10}");
                if (r.IsEnd) {
                    sb.AppendLine("End-Line");
                    sb.AppendLine("");
                    total = 0;
                }
            });
            FileHelper.WriteStrToTxt(sb.ToString(), Project.Options.OutputFiles.CheakRawData);
            AddTabPage(Project.Options.OutputFiles.CheakRawData);
        }
    }
}

