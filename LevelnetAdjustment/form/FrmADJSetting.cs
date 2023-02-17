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
using MathNet.Numerics;
using LevelnetAdjustment.utils;

namespace LevelnetAdjustment.form {
  public delegate void TransfDelegate(Option option);
  public partial class FrmADJSetting : Form {

    public Option Options { get; set; }
    public event TransfDelegate TransfEvent;
    public FrmADJSetting(Option options) {
      this.Options = options;
      InitializeComponent();
    }


    /// <summary>
    /// 控制是否显示单位权输入框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void rbtn_before_CheckedChanged(object sender, EventArgs e) {
      if ((sender as RadioButton).Checked) {
        this.textBox4.Visible = true;
      }
      else {
        this.textBox4.Visible = false;
      }
    }

    private void FrmADJSetting_Load(object sender, EventArgs e) {
      this.comboBox1.SelectedIndex = Options.LevelParams.LevelingGrade;
      this.textBox1.Text = Options.LevelParams.WangFan.ToString();
      this.textBox2.Text = Options.LevelParams.FuHe.ToString();
      this.textBox3.Text = Options.LevelParams.Huan.ToString();
      this.textBox6.Text = Options.LevelParams.CP3WangFan.ToString();
      this.textBox5.Text = Options.LevelParams.CP3Huan.ToString();
      this.checkBox2.Checked = Options.LevelParams.IsCP3;

      switch (Options.UnitRight) {
        case 0:
          this.rbtn_before.Checked = true;
          break;
        case 1:
          this.rbtn_after.Checked = true;
          break;
        default:
          break;
      }

      this.textBox4.Text = Options.Sigma.ToString();
      this.textBox4.Visible = rbtn_before.Checked ? true : false;
      tb_limit.Text = (Options.Limit * 100).ToString();
      groupBox1.Enabled = checkBox2.Checked;

      txt_xianzhu.Text = Options.Alpha.ToString();
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
      ComboBox comboBox = (ComboBox)sender;
      int idx = comboBox.SelectedIndex;
      switch (idx) {
        case 0:
          textBox1.Text = "1.8";
          textBox2.Text = "2";
          textBox3.Text = "-1";
          break;
        case 1:
          textBox1.Text = "4";
          textBox2.Text = "4";
          textBox3.Text = "4";
          break;
        case 2:
          textBox1.Text = "12";
          textBox2.Text = "12";
          textBox3.Text = "12";
          break;
        case 3:
          textBox1.Text = "20";
          textBox2.Text = "20";
          textBox3.Text = "20";
          break;
        case 4:
          textBox1.Text = "8";
          textBox2.Text = "8";
          textBox3.Text = "8";
          break;
        default:
          textBox1.Text = "8";
          textBox2.Text = "8";
          textBox3.Text = "8";
          break;
      }
    }

    private void button1_Click(object sender, EventArgs e) {
      Close();
    }

    private void button2_Click(object sender, EventArgs e) {
      Options.LevelParams.LevelingGrade = comboBox1.SelectedIndex;
      Options.LevelParams.WangFan = double.Parse(textBox1.Text);
      Options.LevelParams.FuHe = double.Parse(textBox2.Text);
      Options.LevelParams.Huan = double.Parse(textBox3.Text);
      Options.LevelParams.CP3WangFan = double.Parse(textBox6.Text);
      Options.LevelParams.CP3Huan = double.Parse(textBox5.Text);
      Options.LevelParams.IsCP3 = checkBox2.Checked;
      Options.Sigma = double.Parse(textBox4.Text);
      Options.Limit = double.Parse(tb_limit.Text) / 100;
      Options.UnitRight = rbtn_before.Checked ? 0 : 1;
      Options.Alpha = double.Parse(txt_xianzhu.Text);
      Options.AlphaLimit = double.Parse(lbl_xiancha.Text);

      TransfEvent(Options);
      Close();

    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e) {
      groupBox1.Enabled = checkBox2.Checked;
    }

    private void txt_xianzhu_TextChanged(object sender, EventArgs e) {
      if (!Commom.IsFloat(txt_xianzhu.Text)) {
        return;
      }
      try {
        Options.Alpha = double.Parse(txt_xianzhu.Text);
        Options.AlphaLimit = ExcelFunctions.NormSInv(1 - Options.Alpha / 2);
        lbl_xiancha.Text = Options.AlphaLimit.ToString();
      }
      catch (Exception) {
        return;
      }

    }
  }
}
