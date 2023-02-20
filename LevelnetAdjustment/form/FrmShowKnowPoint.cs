using LevelnetAdjustment.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace LevelnetAdjustment.form {
  public partial class FrmShowKnowPoint : Form {
    List<PointData> KnownPoints;
    List<PointData> AddPoints;
    BindingSource BindingSourcePoint;
    List<ObservedData> ObservedDatas;
    public BindingSource BindingPoint { get; set; }

    public FrmShowKnowPoint(List<PointData> knownPoints, List<PointData> addPoints = null, BindingSource bindingSource = null, List<ObservedData> observedDatas = null) {
      KnownPoints = knownPoints;
      AddPoints = addPoints;
      BindingSourcePoint = bindingSource;
      ObservedDatas = observedDatas;
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
        string repeatPointStrStart = "以下已知点重复：";

        string repeatPointStr = repeatPointStrStart;
        int repeatPointCount = 0;
        string notFoundStrStart = "\r\r以下已知点在观测数据中不存在：";
        string notFoundStr = notFoundStrStart;
        int notFoundCount = 0;
        for (int i = 0; i < AddPoints.Count; i++) {
          PointData point = AddPoints[i];
          if (point.Enable) {
            PointData p = (BindingSourcePoint.DataSource as List<PointData>).Find(t => t.Number == point.Number);
            // 点名重复
            if (p != null) {
              repeatPointCount++;
              repeatPointStr += $"\r{repeatPointCount,-10}{point.Number,-10}";
            }
            // 点名不重复
            else {
              // 点名不存在
              if (!ObservedDatas.Exists(l => l.Start == point.Number || l.End == point.Number)) {
                notFoundCount++;
                notFoundStr += $"\r{notFoundCount,-10}{point.Number,-10}";
              }
              // 点名存在
              else {
                BindingSourcePoint.Add(point);
              }
            }
          }
        }

        //显示错误信息
        string res = "";
        if (repeatPointStr != repeatPointStrStart) {
          res += repeatPointStr;
        }
        else {
          notFoundStr = notFoundStr.Substring(2);
        }
        if (notFoundStr != notFoundStrStart) {
          res += notFoundStr;
        }
        if (res != "") {
          MessageBox.Show($"{res}\r\r请检查！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
      Close();
      //MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
