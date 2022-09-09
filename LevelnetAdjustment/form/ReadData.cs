using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
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
    public partial class ReadData : Form {
        public List<string> FileList { get; set; }
        public ClevelingAdjust ClAdj { get; set; }

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
            FileList.ForEach(t => {
                switch (Path.GetExtension(t).ToLower()) {
                    case ".dat":
                        FileHelper.ReadDAT(t, ClAdj.RawDatas, ClAdj.ObservedDatas, ClAdj.Options.IsSplit);
                        break;
                    case ".gsi":
                        FileHelper.ReadGSI(t, ClAdj.RawDatas, ClAdj.ObservedDatas, ClAdj.KnownPoints, ClAdj.Options.IsSplit);
                        break;
                    case ".in1":
                        var tup = FileHelper.ReadOriginalFile(ClAdj.KnownPoints, ClAdj.ObservedDatas, t);
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
        }
    }
}
