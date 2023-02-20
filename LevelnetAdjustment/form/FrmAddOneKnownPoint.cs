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

namespace LevelnetAdjustment.form {
  public partial class FrmAddOneKnownPoint : Form {

    public List<ObservedData> ObservedDatas { get; set; }
    public BindingSource BindingSourcePoint { get; set; }
    public DataGridView MyDataGridView { get; set; }
    public FrmAddOneKnownPoint(BindingSource bindingSourcePoint, List<ObservedData> observedDatas, DataGridView dataGridView) {
      MyDataGridView = dataGridView;
      BindingSourcePoint = bindingSourcePoint;
      ObservedDatas = observedDatas;
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      PointData point = new PointData() { Number = textBox1.Text.Trim(), Height = Convert.ToDouble(textBox2.Text.Trim()), IsStable = false, Enable = true };
      // 检查点名是否重复
      PointData p = (BindingSourcePoint.DataSource as List<PointData>).Find(t => t.Number == point.Number);
      // 检查点名是否在观测文件中
      if (p != null) {
        MessageBox.Show("该点已存在，请检查", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      else {
        if (!ObservedDatas.Exists(l => l.Start == point.Number || l.End == point.Number)) {
          MessageBox.Show("该点在观测数据中不存在，请检查", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else {
          BindingSourcePoint.Add(point);
          MyDataGridView.FirstDisplayedScrollingRowIndex = MyDataGridView.Rows.Count - 1;
          Close();
        }
      }
    }

    private void button2_Click(object sender, EventArgs e) {
      Close();
    }

    private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
      if ((int)e.KeyChar == '.' && (textBox2.Text.Contains(".") || textBox2.Text == "")) {
        e.Handled = true;
      }
      if (textBox2.Text == "") {
        if ((int)e.KeyChar <= '0' || (int)e.KeyChar > '9') e.Handled = true;
      }
      else if (((int)e.KeyChar < '0' || (int)e.KeyChar > '9') && (int)e.KeyChar != 8 && (int)e.KeyChar != '.') {
        e.Handled = true;
      }
    }
  }
}
