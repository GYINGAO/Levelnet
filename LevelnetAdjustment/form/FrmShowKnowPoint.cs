using LevelnetAdjustment.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
  public partial class FrmShowKnowPoint : Form {
    List<PointData> KnownPoints;
    List<PointData> AddPoints;
    BindingSource BindingSourcePoint;
    public BindingSource BindingPoint { get; set; }

    public FrmShowKnowPoint(List<PointData> knownPoints, List<PointData> addPoints = null, BindingSource bindingSource = null) {
      KnownPoints = knownPoints;
      AddPoints = addPoints;
      BindingSourcePoint = bindingSource;
      if (addPoints == null) {
        BindingPoint = new BindingSource() { DataSource = knownPoints };
      }
      else {
        BindingPoint = new BindingSource() { DataSource = addPoints };
      }
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      if (AddPoints == null) {
        KnownPoints = KnownPoints.FindAll(t => t.Enable);
      }
      else {
        foreach (PointData point in AddPoints) {
          if (point.Enable) {
            BindingSourcePoint.Add(point);
          }
        }

      }
      Close();
      MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button2_Click(object sender, EventArgs e) {
      if (AddPoints == null) {
        KnownPoints.Clear();
      }
      Close();
    }

    private void FrmShowKnowPoint_Load(object sender, EventArgs e) {
      this.dataGridView1.AutoGenerateColumns = false;
      this.dataGridView1.DataSource = BindingPoint;
    }

    private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e) {

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
  }
}
