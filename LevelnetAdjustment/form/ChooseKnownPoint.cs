using LevelnetAdjustment.model;
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
    public event ChangeStable TransfChangeKnownPoint;
    public ChooseKnownPoint(List<PointData> points) {
      this.Points = points;
      InitializeComponent();
      //利用反射设置DataGridView的双缓冲
      Type dgvType = this.dataGridView1.GetType();
      PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
          BindingFlags.Instance | BindingFlags.NonPublic);
      pi.SetValue(this.dataGridView1, true, null);
    }

    private void ChooseKnownPoint_Load(object sender, EventArgs e) {
      this.dataGridView1.AutoGenerateColumns = false;
      this.dataGridView1.DataSource = new BindingList<PointData>(Points);
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
      Close();
    }

    private void button1_Click(object sender, EventArgs e) {
      TransfChangeKnownPoint(Points);
      Close();
    }

    private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e) {

    }
  }
}
