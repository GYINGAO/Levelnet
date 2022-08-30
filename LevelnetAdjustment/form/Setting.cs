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
    //声明委托 和 事件
    public delegate void TransfDelegate(Option option);
    public partial class Setting : Form {
        public Option Options { get; set; }

        public event TransfDelegate TransfEvent;
        public Setting(Option _option) {
            this.Options = _option;
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e) {
            switch (Options.PowerMethod) {
                case 0:
                    this.rbtn_dis.Checked = true;
                    break;
                case 1:
                    this.rbtn_num.Checked = true;
                    break;
                default:
                    break;
            }

            switch (Options.Level) {
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

            switch (Options.AdjustMethod) {
                case 0:
                    this.rbtn_constraint.Checked = true;
                    break;
                case 1:
                    this.rbtn_quasi_stable.Checked = true;
                    break;
                default:
                    break;
            }

            this.tb_limit.Text = (Options.Limit * 100).ToString();
        }

        private void btn_cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e) {
            this.Options.Level = rbtn1.Checked ? 1 : rbtn2.Checked ? 2 : rbtn3.Checked ? 3 : 4;
            this.Options.PowerMethod = rbtn_dis.Checked ? 0 : 1;
            this.Options.Limit = double.Parse(tb_limit.Text) / 100;
            this.Options.AdjustMethod = rbtn_constraint.Checked ? 0 : 1;
            TransfEvent(Options);
            this.Close();
        }
    }
}
