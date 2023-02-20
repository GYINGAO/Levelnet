using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
  public partial class ChooseKnownPoint : Form {
    public List<PointData> Points { get; set; }
    public List<ObservedData> ObservedDatas { get; set; }
    public BindingSource BindingSourcePoint { get; set; }
    public event ChangeStable TransfChangeKnownPoint;
    public ChooseKnownPoint(List<PointData> points, List<ObservedData> observedDatas) {
      ObservedDatas = observedDatas;
      this.Points = points;
      BindingSourcePoint = new BindingSource() { DataSource = this.Points };
      InitializeComponent();
      //利用反射设置DataGridView的双缓冲
      Type dgvType = this.dataGridView1.GetType();
      PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
          BindingFlags.Instance | BindingFlags.NonPublic);
      pi.SetValue(this.dataGridView1, true, null);
    }

    private void ChooseKnownPoint_Load(object sender, EventArgs e) {
      this.dataGridView1.AutoGenerateColumns = false;
      /*   this.dataGridView1.DataSource = new BindingList<PointData>(Points);*/
      this.dataGridView1.DataSource = BindingSourcePoint;
    }

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

    private void button2_Click(object sender, EventArgs e) {
      TransfChangeKnownPoint(Points);
      Close();
    }

    private void button1_Click(object sender, EventArgs e) {
      Close();
    }

    private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e) {

    }

    private void 添加单个点ToolStripMenuItem_Click(object sender, EventArgs e) {
      FrmAddOneKnownPoint frm = new FrmAddOneKnownPoint(BindingSourcePoint, ObservedDatas, dataGridView1);
      frm.ShowDialog();
    }

    private void 导入文件ToolStripMenuItem_Click(object sender, EventArgs e) {
      OpenFileDialog openFile = new OpenFileDialog {
        Multiselect = false,
        Title = "打开",
        Filter = "已知点文件|*.txt",
        FilterIndex = 1,
        RestoreDirectory = true,
      };
      if (openFile.ShowDialog() == DialogResult.OK) {
        List<PointData> PointList = new List<PointData>();
        FileHelper.ReadKnPoints(openFile.FileName, PointList);
        FrmShowKnowPoint frm = new FrmShowKnowPoint(new List<PointData>(), PointList, BindingSourcePoint, ObservedDatas);
        frm.ShowDialog(this);
        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
      }
    }
  }
}
