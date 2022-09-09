using LevelnetAdjustment.form;
using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using MathNet.Numerics.LinearAlgebra;
using SplashScreenDemo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment {
    public partial class MainForm : Form {
        // 文件路径
        private string filePath;
        public string FilePath {
            get => filePath;
            set {
                filePath = value;
                OutpathAdj = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value) + "约束网平差结果.ou3");
                OutpathAdjFree = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value) + "拟稳平差结果.ou4");
                OutpathClosure = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value) + "闭合差计算结果.ou1");
                OutpathGrossError = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value) + "粗差探测结果.ou2");
            }
        }
        // 输出文件格式
        public readonly string split = "----------------------------------------------------------------------------------------------";
        public readonly string space = "                             ";
        public string OutpathClosure { get; set; } = "";// 闭合差文件输出路径
        public string OutpathGrossError { get; set; } // 粗差探测结果
        public string OutpathAdj { get; set; } = "";// 平差文件输出路径
        public string OutpathAdjFree { get; set; } = "";// 平差文件输出路径
        public ClevelingAdjust ClAdj { get; set; }

        public static bool flag = true;

        public List<string> FileList { get; set; } = new List<string>(); //输入文件列表

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm() {
            InitializeComponent();
            ClAdj = new ClevelingAdjust();
        }


        private void MainForm_Load(object sender, EventArgs e) {
            tabControl1.TabPages.Clear();
            tabControl1.Visible = false;    // 没有元素的时候隐藏自己
        }


        #region 文件管理
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_read_Click(object sender, EventArgs e) {
            ReadData rd = new ReadData(FileList, ClAdj);
            rd.TransfEvent += frm_DataTransfEvent;
            rd.ShowDialog();
        }

        private void frm_DataTransfEvent(List<string> fileList) {
            this.FileList = fileList;
            for (int i = 0; i < fileList.Count; i++) {
                if (Path.GetExtension(fileList[i].ToLower()) != ".txt") {
                    FilePath = fileList[i];
                    break;
                }
            }

        }

        /// <summary>
        /// 创建新的观测文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewDropItem_Click(object sender, EventArgs e) {
            FileView fileView = new FileView("") {
                MdiParent = this,
                //WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                ShowInTaskbar = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None
            };
            fileView.Show();
            AddTabPage(fileView);  // 新建窗体同时新建一个标签
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearDropItem_Click(object sender, EventArgs e) {
            ClAdj = new ClevelingAdjust();
            FormCollection childCollection = Application.OpenForms;
            for (int i = childCollection.Count; i-- > 0;) {
                if (childCollection[i].Name != this.Name) childCollection[i].Close();
            }
            tabControl1.TabPages.Clear();
            tabControl1.Visible = false;    // 没有元素的时候隐藏自己
        }

        /// <summary>
        /// 打开任意文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_open_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "COSA观测文件|*.in1|DAT观测文件|*.dat;*.DAT|GSI-8观测文件|*.gsi;*.GSI|闭合差结果文件|*.ou1|" +
                "粗差探测结果文件|*.ou2|约束网平差结果文件|*.ou3|拟稳平差结果结果文件|*.ou4|所有文件(*.*)|*.*",
                FilterIndex = 1,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                foreach (var item in openFile.FileNames) {
                    FileView fv = new FileView(item) {
                        MdiParent = this,//WindowState = FormWindowState.Maximized,
                        ShowIcon = false,
                        ShowInTaskbar = false,
                        Dock = DockStyle.Fill,
                        FormBorderStyle = FormBorderStyle.None
                    };
                    fv.Show();
                    AddTabPage(fv);  // 新建窗体同时新建一个标签
                }
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
            if (File.Exists(OutpathAdj)) {
                if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    return;
                }
            }

            SimpleLoading loadingfrm = new SimpleLoading(this, "约束网平差中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            /*try {*/
            int i = ClAdj.LS_Adjustment();
            ClAdj.ExportConstraintNetworkResult(OutpathAdj, split, space);
            loading.CloseWaitForm();
            FileView fileView = new FileView(OutpathAdj) {
                MdiParent = this,
                //WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                ShowInTaskbar = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None
            };
            MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
            AddTabPage(fileView);  // 新建窗体同时新建一个标签
            /* }
             catch (Exception ex) {
                 loading.CloseWaitForm();
                 throw ex;
             }*/

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
            if (File.Exists(OutpathAdjFree)) {
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
                    /*try {*/
                    ClAdj.CalcApproximateHeight();
                    FileHelper.ReadStablePoint(openFile.FileName, ClAdj.StablePoints, ClAdj.UnknownPoints);
                    i = ClAdj.QuasiStable();
                    ClAdj.ExportFreeNetworkResult(split, space, OutpathAdjFree, "拟稳");
                    loading.CloseWaitForm();
                    /*      }
                          catch (Exception ex) {
                              loading.CloseWaitForm();
                              throw ex;
                          }*/
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
                    ClAdj.ExportFreeNetworkResult(split, space, OutpathAdjFree, "自由网");
                    loading.CloseWaitForm();
                }
                catch (Exception ex) {
                    loading.CloseWaitForm();
                    throw ex;
                }
            }

            FileView fileView = new FileView(OutpathAdjFree) {
                MdiParent = this,
                //WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                ShowInTaskbar = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None
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
            if (File.Exists(OutpathClosure)) {
                if (MessageBox.Show("闭合差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    return;
                }
            }

            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                ClAdj.CalcClosureError(OutpathClosure, split, space);
                loading.CloseWaitForm();
                FileView fileView = new FileView(OutpathClosure) {
                    MdiParent = this,
                    //WindowState = FormWindowState.Maximized,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    Dock = DockStyle.Fill,
                    FormBorderStyle = FormBorderStyle.None
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
                ClAdj.FindGrossError(split, space, OutpathGrossError);
                loading.CloseWaitForm();
                FileView fileView = new FileView(OutpathGrossError) {
                    MdiParent = this,
                    //WindowState = FormWindowState.Maximized,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    Dock = DockStyle.Fill,
                    FormBorderStyle = FormBorderStyle.None
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

        /// <summary>
        /// 打开设置窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionDropItem_Click(object sender, EventArgs e) {
            Setting setting = new Setting(ClAdj.Options);
            setting.TransfEvent += frm_SettingTransfEvent;
            setting.ShowDialog();
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
                FileName = FilePath != "" ? Path.GetFileName(Path.GetFileNameWithoutExtension(FilePath)) : "",
                InitialDirectory = FilePath != "" ? Path.GetDirectoryName(FilePath) : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                SimpleLoading loadingfrm = new SimpleLoading(this, "导出中，请稍等...");
                //将Loaing窗口，注入到 SplashScreenManager 来管理
                GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                loading.ShowLoading();
                try {
                    ExceHelperl.ExportHandbook(ClAdj.RawDatas, ClAdj.ObservedDatas, saveFileDialog.FileName);
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
                Filter = "COSA水准观测文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = Path.GetFileName(Path.GetFileNameWithoutExtension(FilePath)),
                InitialDirectory = Path.GetDirectoryName(FilePath),
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
                FileName = Path.GetFileName(Path.GetFileNameWithoutExtension(FilePath)),
                InitialDirectory = Path.GetDirectoryName(FilePath),
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

        private void toolStripMenuItem_close_Click(object sender, EventArgs e) {
            int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);  // 这里也需要通过弹出菜单的位置来得到当前是哪个项弹出的，注意菜单位置是针对屏幕左边的距离
            CloseTabPage(index);
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

        //事件处理方法
        void frm_SettingTransfEvent(Option option) {
            this.ClAdj.Options = option;
        }

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
            toolStripMenuItem_open_Click(sender, e);
        }

        private void 帮助LToolStripButton_Click(object sender, EventArgs e) {
            AboutDropItem_Click(sender, e);
        }
    }
}

