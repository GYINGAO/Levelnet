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
            Options.LevelParams.LevelingGrade = this.comboBox1.SelectedIndex;
            Options.LevelParams.WangFan = double.Parse(this.textBox1.Text);
            Options.LevelParams.FuHe = double.Parse(this.textBox2.Text);
            Options.LevelParams.Huan = double.Parse(this.textBox3.Text);
            Options.LevelParams.CP3WangFan = double.Parse(this.textBox6.Text);
            Options.LevelParams.CP3Huan = double.Parse(this.textBox5.Text);
            Options.LevelParams.IsCP3 = this.checkBox2.Checked;
            Options.Sigma = double.Parse(this.textBox4.Text);
            Options.Limit = double.Parse(tb_limit.Text) / 100;
            Options.UnitRight = rbtn_before.Checked ? 0 : 1;

            TransfEvent(Options);
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
