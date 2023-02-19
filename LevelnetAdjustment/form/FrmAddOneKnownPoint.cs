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

  public delegate void DelegateSendPoint(PointData point);


  public partial class FrmAddOneKnownPoint : Form {

    public PointData Point { get; set; } = new PointData() { IsStable = false, Enable = true };

    public event DelegateSendPoint AddOnePoint;
    public FrmAddOneKnownPoint() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      Point.Number = textBox1.Text.Trim();
      Point.Height = Convert.ToDouble(textBox2.Text.Trim());
      AddOnePoint(Point);
      Close();
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
