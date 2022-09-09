using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using SplashScreenDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
    //声明委托 和 事件
    public delegate void TransfDelegate_2(List<string> fileList);
    public partial class ReadData : Form {
        public List<string> FileList { get; set; }
        public ClevelingAdjust ClAdj { get; set; }

        public event TransfDelegate_2 TransfEvent;

        public ReadData(List<string> fileList, ClevelingAdjust clAdj) {
            this.FileList = fileList;
            this.ClAdj = clAdj;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "COSA观测文件|*.in1|DAT观测文件|*.dat;*.DAT|GSI-8观测文件|*.gsi;*.GSI|已知点文件|*.txt|所有文件(*.*)|*.*",
                FilterIndex = 1,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                foreach (var item in openFile.FileNames) {
                    listBox1.Items.Add(item);
                }
                UpdateList();
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.listBox1.SelectedIndex != -1) {
                int count = this.listBox1.SelectedIndices.Count;
                //循环采用倒序，从最后一个开始删除，这样index就是正确的
                for (int i = 0; i < count; i++) {
                    int index = count - 1 - i;
                    int deleteIndex = this.listBox1.SelectedIndices[index];
                    this.listBox1.Items.RemoveAt(deleteIndex);
                }
                UpdateList();
            }
        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                this.删除ToolStripMenuItem_Click(sender, e);
            }
        }

        private void ReadData_Load(object sender, EventArgs e) {
            FileList.ForEach(t => listBox1.Items.Add(t));
        }

        void UpdateList() {
            FileList = new List<string>();
            foreach (var item in listBox1.Items) {
                FileList.Add(item.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (FileList.Count == 0) {
                throw new Exception("未选择文件");
            }

            ClAdj.ObservedDatas = new List<ObservedData>();
            ClAdj.KnownPoints = new List<PointData>();
            ClAdj.RawDatas = new List<RawData>();

            var KnownPoints = new List<PointData>();
            var ObservedDatas = new List<ObservedData>();
            var RawDatas = new List<RawData>();

            for (int i = 0; i < FileList.Count; i++) {
                string ext = Path.GetExtension(FileList[i]).ToLower();
                if (ext.Contains(".dat") && ext.Contains(".gsi")) {
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
                    break;
                }
            }

            SimpleLoading loadingfrm = new SimpleLoading(this, "读取中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                FileList.ForEach(t => {
                    switch (Path.GetExtension(t).ToLower()) {
                        case ".dat":
                            FileHelper.ReadDAT(t, RawDatas, ObservedDatas, ClAdj.Options.IsSplit);
                            break;
                        case ".gsi":
                            FileHelper.ReadGSI(t, RawDatas, ObservedDatas, KnownPoints, ClAdj.Options.IsSplit);
                            break;
                        case ".in1":
                            var tup = FileHelper.ReadOriginalFile(KnownPoints, ObservedDatas, t);
                            ClAdj.Options.Level = tup.Item1;
                            ClAdj.Options.PowerMethod = tup.Item2;
                            break;
                        case ".txt":
                            FileHelper.ReadKnPoints(t, ClAdj.KnownPoints);
                            break;
                        default:
                            break;
                    }
                });
                ClAdj.RawDatas = ClAdj.RawDatas != null ? Commom.Merge(ClAdj.RawDatas, RawDatas) : RawDatas;
                ClAdj.ObservedDatas = ClAdj.ObservedDatas != null ? Commom.Merge(ClAdj.ObservedDatas, ObservedDatas) : ObservedDatas;
                ClAdj.KnownPoints = ClAdj.KnownPoints != null ? Commom.Merge(ClAdj.KnownPoints, KnownPoints) : KnownPoints;
                ClAdj.ObservedDatasNoRep = Calc.RemoveDuplicates(ClAdj.ObservedDatas);
                loading.CloseWaitForm();
                TransfEvent(FileList);
                if (MessageBox.Show("读取成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK) {
                    this.Close();
                }
            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }
        }
    }
}
