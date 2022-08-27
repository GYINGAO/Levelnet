using LevelnetAdjustment.form;
using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using MathNet.Numerics.LinearAlgebra;
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
                ClAdj.Level = tup.Item1;
                ClAdj.PowerMethod = tup.Item2;
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
        /// 约束网平差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelnetDropItem_Click(object sender, EventArgs e) {
            if (File.Exists(OutpathAdj)) {
                if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    return;
                }
            }
            if (ClAdj.ObservedDatas == null) {
                throw new Exception("请打开观测文件");
            }
            var i = ClAdj.LS_Adjustment();
            ClAdj.ExportConstraintNetworkResult(OutpathAdj, split, space);

            FileView fileView = new FileView(new string[] { OutpathAdj }) {
                MdiParent = this,
            };
            MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
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
            ClAdj.CalcClosureError(OutpathClosure, split, space);
            FileView fileView = new FileView(new string[] { OutpathClosure }) {
                MdiParent = this,
            };
            MessageBox.Show("闭合差计算完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
        }

        /// <summary>
        /// 粗差探测按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrossErrorDropItem_Click(object sender, EventArgs e) {
            ClAdj.FindGrossError(split, space, OutpathGrossError);
            FileView fileView = new FileView(new string[] { OutpathGrossError }) {
                MdiParent = this,
            };
            MessageBox.Show("粗差探测完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
        }

        /// <summary>
        /// 打开设置窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionDropItem_Click(object sender, EventArgs e) {
            Setting setting = new Setting(ClAdj.PowerMethod, ClAdj.Limit, ClAdj.Level, ClAdj.AdjustmentMethod);
            setting.TransfEvent += frm_TransfEvent;
            setting.ShowDialog();
        }

        //事件处理方法
        void frm_TransfEvent(int method, double limit, int level, int adj) {
            this.ClAdj.PowerMethod = method;
            this.ClAdj.Limit = limit;
            this.ClAdj.Level = level;
            this.ClAdj.Coefficient = level;
            this.ClAdj.AdjustmentMethod = adj;
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
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "DAT观测文件|*.dat;*.DAT|GSI-8观测文件|*.gsi;*.GSI|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                if (string.IsNullOrEmpty(FilePath)) {
                    FilePath = openFile.FileNames[0];
                }
                foreach (var item in openFile.FileNames) {
                    if (Path.GetExtension(item).ToLower() == ".dat") {
                        FileHelper.ReadDAT(item, RawDatas, ObservedDatas);
                    }
                    else if (Path.GetExtension(item).ToLower() == ".gsi") {
                        FileHelper.ReadGSI(item, RawDatas, ObservedDatas, KnownPoints);
                    }
                }
                Commom.Merge(ClAdj.RawDatas, RawDatas);
                Commom.Merge(ClAdj.ObservedDatas, ObservedDatas);
                Commom.Merge(ClAdj.KnownPoints, KnownPoints);

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
            Commom.Merge(ClAdj.KnownPoints, KnownPoints);
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
                ExceHelperl.ExportHandbook(ClAdj.RawDatas, ClAdj.ObservedDatas, saveFileDialog.FileName);
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
            FilePath = null;
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
    }
}

