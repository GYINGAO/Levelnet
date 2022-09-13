using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using SplashScreenDemo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
    public delegate void TransfDelegate(Option options);
    public partial class ReadData : Form {
        public ClevelingAdjust ClAdj { get; set; }
        public Option Options { get; set; }
        public string ProjDir { get; set; }

        public bool IsFileChange { get; set; } = false;

        public event TransfDelegate TransfEvent;

        public ReadData(ClevelingAdjust clAdj, ProjectInfo project) {
            this.ClAdj = clAdj;
            this.Options = clAdj.Options;
            this.ProjDir = Path.Combine(project.Path, project.Name, "ImportFiles");
            InitializeComponent();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "DAT观测文件|*.dat;*.DAT|GSI-8观测文件|*.gsi;*.GSI|COSA观测文件|*.in1;*.IN1|已知点文件|*.txt",
                FilterIndex = 1,
                RestoreDirectory = true,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                foreach (var item in openFile.FileNames) {
                    int idx = dataGridView1.Rows.Add();
                    dataGridView1.Rows[idx].Cells["FileName"].Value = item;
                    dataGridView1.Rows[idx].Cells["IsSplit"].Value = false;
                }
                this.IsFileChange = true;
                dataGridView1.CurrentCell = null;
            }
        }

        /// <summary>
        /// 删除选中项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.dataGridView1.SelectedRows.Count != 0) {
                for (int i = dataGridView1.SelectedRows.Count; i > 0; i--) {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[i - 1].Index);
                }
            }
            this.IsFileChange = true;
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e) {
            dataGridView1.Rows.Clear();
            this.IsFileChange = true;
        }

        /// <summary>
        /// 键盘删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                this.删除ToolStripMenuItem_Click(sender, e);
            }
        }

        /// <summary>
        /// 窗体载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadData_Load(object sender, EventArgs e) {
            Options.ImportFiles.ForEach(t => {
                int idx = dataGridView1.Rows.Add();
                dataGridView1.Rows[idx].Cells["FileName"].Value = t.FileName;
                dataGridView1.Rows[idx].Cells["IsSplit"].Value = t.IsSplit;
            });
            dataGridView1.CurrentCell = null;

            switch (ClAdj.Options.PowerMethod) {
                case 0:
                    this.rbtn_dis.Checked = true;
                    break;
                case 1:
                    this.rbtn_num.Checked = true;
                    break;
                default:
                    break;
            }

            switch (ClAdj.Options.Level) {
                case 1:
                    this.rbtn1.Checked = true;
                    break;
                case 2:
                    this.rbtn2.Checked = true;
                    break;
                case 3:
                    this.rbtn3.Checked = true;
                    break;
                case 4:
                    this.rbtn4.Checked = true;
                    break;
                default:
                    break;
            }
            switch (ClAdj.Options.UnitRight) {
                case 0:
                    this.rbtn_before.Checked = true;
                    break;
                case 1:
                    this.rbtn_after.Checked = true;
                    break;
                default:
                    break;
            }

            this.tb_limit.Text = (ClAdj.Options.Limit * 100).ToString();
            this.textBox1.Visible = rbtn_before.Checked ? true : false;
            this.textBox1.Text = ClAdj.Options.Sigma.ToString();
        }


        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) {
            if (dataGridView1.Rows.Count == 0) {
                if (MessageBox.Show("没有选择文件，是否重新选择？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.OK) {
                    return;
                }
                else {
                    Options.Level = rbtn1.Checked ? 1 : rbtn2.Checked ? 2 : rbtn3.Checked ? 3 : 4;
                    Options.PowerMethod = rbtn_dis.Checked ? 0 : 1;
                    Options.Limit = double.Parse(tb_limit.Text) / 100;
                    Options.UnitRight = rbtn_before.Checked ? 0 : 1;
                    Options.Sigma = double.Parse(textBox1.Text);
                    TransfEvent(Options);
                    Close();
                    return;
                }
            }
            //没有改变文件，直接退出
            if (!IsFileChange) {
                Options.Level = rbtn1.Checked ? 1 : rbtn2.Checked ? 2 : rbtn3.Checked ? 3 : 4;
                Options.PowerMethod = rbtn_dis.Checked ? 0 : 1;
                Options.Limit = double.Parse(tb_limit.Text) / 100;
                Options.UnitRight = rbtn_before.Checked ? 0 : 1;
                Options.Sigma = double.Parse(textBox1.Text);
                TransfEvent(Options);
                Close();
                return;
            }
            ClAdj.ObservedDatas = new List<ObservedData>();
            ClAdj.KnownPoints = new List<PointData>();
            ClAdj.RawDatas = new List<RawData>();

            var KnownPoints = new List<PointData>();
            var ObservedDatas = new List<ObservedData>();
            var RawDatas = new List<RawData>();



            SimpleLoading loadingfrm = new SimpleLoading(this, "读取中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            try {
                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    string fileName = row.Cells["FileName"].Value.ToString();
                    bool isSplit = (bool)row.Cells["IsSplit"].Value;
                    //把文件复制到项目文件夹中
                    FileInfo fileInfo1 = new FileInfo(fileName);
                    string targetPath = Path.Combine(ProjDir, Path.GetFileName(fileName));
                    if (File.Exists(targetPath)) File.Delete(targetPath);
                    fileInfo1.CopyTo(targetPath);


                    switch (Path.GetExtension(fileName).ToLower()) {
                        case ".dat":
                            FileHelper.ReadDAT(fileName, RawDatas, ObservedDatas, isSplit);
                            break;
                        case ".gsi":
                            FileHelper.ReadGSI(fileName, RawDatas, ObservedDatas, KnownPoints, isSplit);
                            break;
                        case ".in1":
                            var tup = FileHelper.ReadOriginalFile(KnownPoints, ObservedDatas, fileName);
                            ClAdj.Options.Level = tup.Item1;
                            ClAdj.Options.PowerMethod = tup.Item2;
                            break;
                        case ".txt":
                            FileHelper.ReadKnPoints(fileName, ClAdj.KnownPoints);
                            break;
                        default:
                            break;
                    }
                }

                ClAdj.RawDatas = ClAdj.RawDatas != null ? Commom.Merge(ClAdj.RawDatas, RawDatas) : RawDatas;
                ClAdj.ObservedDatas = ClAdj.ObservedDatas != null ? Commom.Merge(ClAdj.ObservedDatas, ObservedDatas) : ObservedDatas;
                ClAdj.KnownPoints = ClAdj.KnownPoints != null ? Commom.Merge(ClAdj.KnownPoints, KnownPoints) : KnownPoints;
                ClAdj.ObservedDatasNoRep = Calc.RemoveDuplicates(ClAdj.ObservedDatas);


                Options.Level = rbtn1.Checked ? 1 : rbtn2.Checked ? 2 : rbtn3.Checked ? 3 : 4;
                Options.PowerMethod = rbtn_dis.Checked ? 0 : 1;
                Options.Limit = double.Parse(tb_limit.Text) / 100;
                Options.UnitRight = rbtn_before.Checked ? 0 : 1;
                Options.Sigma = double.Parse(textBox1.Text);

                UpdateList();
                loading.CloseWaitForm();
                TransfEvent(Options);
                if (MessageBox.Show("读取成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK) {
                    this.Close();
                }
            }
            catch (Exception ex) {
                loading.CloseWaitForm();
                throw ex;
            }
        }

        /// <summary>
        /// 控制是否显示单位权输入框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtn_before_CheckedChanged(object sender, EventArgs e) {
            if ((sender as RadioButton).Checked) {
                this.textBox1.Visible = true;
            }
            else {
                this.textBox1.Visible = false;
            }

        }

        /// <summary>
        /// 双击查看文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            //得到当前行
            var row = dataGridView1.CurrentRow;
            if (e.ColumnIndex != 0 || e.RowIndex < 0) {
                return;
            }
            if (row != null) {
                FileView fileView = new FileView(row.Cells["FileName"].Value.ToString()) {
                    ControlBox = true,
                    ShowInTaskbar = true,
                };
                fileView.ShowDialog();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex != 1) {
                return;
            }
            IsFileChange = true;
        }

        /// <summary>
        /// 根据表格更新List
        /// </summary>
        void UpdateList() {
            ClAdj.Options.ImportFiles = new List<InputFile>();
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                ClAdj.Options.ImportFiles.Add(new InputFile {
                    FileName = row.Cells["FileName"].Value.ToString(),
                    IsSplit = (bool)row.Cells["IsSplit"].Value
                });
            }
        }

        /// <summary>
        /// 自动编号，与数据无关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               dataGridView1.RowHeadersWidth - 4,
               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                  (e.RowIndex + 1).ToString(),
                   dataGridView1.RowHeadersDefaultCellStyle.Font,
                   rectangle,
                   dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                   TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e) {
            if (GetRowIndexAt(e.Y) == -1) {
                dataGridView1.CurrentCell = null;
            }
        }

        public int GetRowIndexAt(int mouseLocation_Y) {
            if (dataGridView1.FirstDisplayedScrollingRowIndex < 0) {
                return -1;
            }
            if (dataGridView1.ColumnHeadersVisible == true && mouseLocation_Y <= dataGridView1.ColumnHeadersHeight) {
                return -1;
            }
            int index = dataGridView1.FirstDisplayedScrollingRowIndex;
            int displayedCount = dataGridView1.DisplayedRowCount(true);
            for (int k = 1; k <= displayedCount;) {
                if (dataGridView1.Rows[index].Visible == true) {
                    Rectangle rect = dataGridView1.GetRowDisplayRectangle(index, true);  // 取该区域的显示部分区域   
                    if (rect.Top <= mouseLocation_Y && mouseLocation_Y < rect.Bottom) {
                        return index;
                    }
                    k++;
                }
                index++;
            }
            return -1;
        }

    }
}
