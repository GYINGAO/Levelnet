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
    List<PointData> KnownPoints = new List<PointData>();

    public FrmShowKnowPoint(List<PointData> knownPoints) {
      KnownPoints = knownPoints;
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      KnownPoints.Clear();
      Close();
    }

    private void button2_Click(object sender, EventArgs e) {
      Close();
      MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void FrmShowKnowPoint_Load(object sender, EventArgs e) {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"{"点名",-10}{"高程/m"}");
      KnownPoints.ForEach(t => sb.AppendLine($"{t.Number,-12}{t.Height}"));
      richTextBox1.Text = sb.ToString();
    }
  }
}
