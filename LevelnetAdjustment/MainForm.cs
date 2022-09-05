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
                OutpathAdj = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value) + "平差结果.ou1");
                OutpathClosure = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value) + "闭合差计算结果.ou2");
                OutpathGrossError = Path.Combine(Path.GetDirectoryName(value), Path.GetFileNameWithoutExtension(value) + "粗差探测结果.ou2");
            }
        }
        // 输出文件格式
        public readonly string split = "---------------------------------------------------------------------------------";
        public readonly string space = "                             ";
        public string OutpathClosure { get; set; } = "";// 闭合差文件输出路径
        public string OutpathGrossError { get; set; } // 粗差探测结果
        public string OutpathAdj { get; set; } = "";// 平差文件输出路径
        public ClevelingAdjust ClAdj { get; set; }

        public static bool flag = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm() {
            InitializeComponent();
            ClAdj = new ClevelingAdjust();
        }



        /// <summary>
        /// 导入COSA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDropItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = false,
                Title = "打开",
                Filter = "COSA文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                FilterIndex = 1,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                if (string.IsNullOrEmpty(FilePath)) {
                    FilePath = openFile.FileName;
                }

                var KnownPoints = new List<PointData>();
                var ObservedDatas = new List<ObservedData>();
                var ObservedDatasNoRep = new List<ObservedData>();
                var tup = FileHelper.ReadOriginalFile(KnownPoints, ObservedDatas, ObservedDatasNoRep, openFile.FileName);
                ClAdj.Options.Level = tup.Item1;
                ClAdj.Options.PowerMethod = tup.Item2;
                ClAdj.KnownPoints = KnownPoints;
                ClAdj.ObservedDatas = ClAdj.ObservedDatas != null ? Commom.Merge(ClAdj.ObservedDatas, ObservedDatas) : ObservedDatas;
                ClAdj.ObservedDatasNoRep = ClAdj.ObservedDatasNoRep != null ? Commom.Merge(ClAdj.ObservedDatasNoRep, ObservedDatasNoRep) : ObservedDatasNoRep;
                FileView fileView = new FileView(new string[] { openFile.FileName }) {
                    MdiParent = this,
                };
                fileView.Show();
            }
            else {
                return;
            }
        }

        /// <summary>
        /// 平差按钮
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
            try {
                int i = ClAdj.LS_Adjustment();
                ClAdj.ExportConstraintNetworkResult(OutpathAdj, split, space);
                loading.CloseWaitForm();
                FileView fileView = new FileView(new string[] { OutpathAdj }) {
                    MdiParent = this,
                };
                MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fileView.Show();
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
            if (File.Exists(OutpathClosure)) {
                if (MessageBox.Show("闭合差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    return;
                }
            }

            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            /*  try {*/
            ClAdj.CalcClosureError(OutpathClosure, split, space);
            loading.CloseWaitForm();
            FileView fileView = new FileView(new string[] { OutpathClosure }) {
                MdiParent = this,
            };
            MessageBox.Show("闭合差计算完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
            /*  }
              catch (Exception ex) {
                  loading.CloseWaitForm();
                  throw ex;
              }*/
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
                FileView fileView = new FileView(new string[] { OutpathGrossError }) {
                    MdiParent = this,
                };
                MessageBox.Show("粗差探测完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fileView.Show();
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
            setting.TransfEvent += frm_TransfEvent;
            setting.ShowDialog();
        }

        //事件处理方法
        void frm_TransfEvent(Option option) {
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
        /// 创建新的观测文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewDropItem_Click(object sender, EventArgs e) {
            FileView fileView = new FileView(new string[] { "" }) {
                MdiParent = this,
            };
            fileView.Show();
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

        /// <summary>
        /// 读取水准仪原始数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RawDataDropItem_Click(object sender, EventArgs e) {
            var RawDatas = new List<RawData>();
            var ObservedDatas = new List<ObservedData>();
            var KnownPoints = new List<PointData>();
            var res = MessageBox.Show("是否按测段分割？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == DialogResult.Yes) {
                ClAdj.Options.IsSplit = true;
            }
            else if (res == DialogResult.No) {
                ClAdj.Options.IsSplit = false;
            }
            else {
                return;
            }
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "DAT观测文件|*.dat;*.DAT|GSI-8观测文件|*.gsi;*.GSI|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                FilePath = openFile.FileNames[0];
                SimpleLoading loadingfrm = new SimpleLoading(this, "读取中，请稍等...");
                //将Loaing窗口，注入到 SplashScreenManager 来管理
                GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                loading.ShowLoading();

                foreach (var item in openFile.FileNames) {
                    if (Path.GetExtension(item).ToLower() == ".dat") {
                        FileHelper.ReadDAT(item, RawDatas, ObservedDatas, ClAdj.Options.IsSplit);
                    }
                    else if (Path.GetExtension(item).ToLower() == ".gsi") {
                        FileHelper.ReadGSI(item, RawDatas, ObservedDatas, KnownPoints, ClAdj.Options.IsSplit);
                    }
                }
                ClAdj.RawDatas = ClAdj.RawDatas != null ? Commom.Merge(ClAdj.RawDatas, RawDatas) : RawDatas;
                ClAdj.ObservedDatas = ClAdj.ObservedDatas != null ? Commom.Merge(ClAdj.ObservedDatas, ObservedDatas) : ObservedDatas;
                ClAdj.KnownPoints = ClAdj.KnownPoints != null ? Commom.Merge(ClAdj.KnownPoints, KnownPoints) : KnownPoints;

                loading.CloseWaitForm();
                FileView fv = new FileView(openFile.FileNames) { MdiParent = this };
                fv.Show();
            }
        }



        /// <summary>
        /// 读取已知点数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KnDropItem_Click(object sender, EventArgs e) {
            var KnownPoints = new List<PointData>();
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "文本文件|*.txt;*.TXT|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                FileHelper.ReadGSI(openFile.FileName, KnownPoints);
            }
            ClAdj.KnownPoints = ClAdj.KnownPoints != null ? Commom.Merge(ClAdj.KnownPoints, KnownPoints) : KnownPoints;
        }

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

        /// <summary>
        /// 秩亏网平差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RankDefectNetworkDropItem_Click(object sender, EventArgs e) {
            if (ClAdj.ObservedDatas == null) {
                throw new Exception("请打开观测文件");
            }
            if (File.Exists(OutpathAdj)) {
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
                    ClAdj.CalcApproximateHeight();
                    FileHelper.ReadStablePoint(openFile.FileName, ClAdj.StablePoints, ClAdj.UnknownPoints);
                    SimpleLoading loadingfrm = new SimpleLoading(this, "拟稳平差中，请稍等...");
                    //将Loaing窗口，注入到 SplashScreenManager 来管理
                    GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
                    loading.ShowLoading();
                    try {
                        i = ClAdj.QuasiStable();
                        ClAdj.ExportFreeNetworkResult(split, space, OutpathAdj);
                        loading.CloseWaitForm();
                    }
                    catch (Exception ex) {
                        loading.CloseWaitForm();
                        throw ex;
                    }
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
                    ClAdj.ExportFreeNetworkResult(split, space, OutpathAdj);
                    loading.CloseWaitForm();
                }
                catch (Exception ex) {
                    loading.CloseWaitForm();
                    throw ex;
                }
            }

            FileView fileView = new FileView(new string[] { OutpathAdj }) {
                MdiParent = this,
            };
            MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }
    }
}

