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
    public delegate void TransfDelegate(int method, double limit, int level, int adj);
    public partial class Setting : Form {
        public int _power { get; set; }
        public double _limit { get; set; }
        public int _level { get; set; }

        public int _adjustmentMethod { get; set; } //平差方法 0约束网平差

        public event TransfDelegate TransfEvent;
        public Setting(int power, double limit, int level, int adj) {
            this._power = power;
            this._limit = limit;
            this._level = level;
            this._adjustmentMethod = adj;
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e) {
            switch (_power) {
                case 0:
                    this.rbtn_dis.Checked = true;
                    break;
                case 1:
                    this.rbtn_num.Checked = true;
                    break;
                default:
                    break;
            }

            switch (_level) {
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

            switch (_adjustmentMethod) {
                case 0:
                    this.rbtn_constraint.Checked = true;
                    break;
                case 1:
                    this.rbtn_quasi_stable.Checked = true;
                    break;
                default:
                    break;
            }

            this.tb_limit.Text = (_limit * 100).ToString();
        }

        private void btn_cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e) {
            this._level = rbtn1.Checked ? 1 : rbtn2.Checked ? 2 : rbtn3.Checked ? 3 : 4;
            this._power = rbtn_dis.Checked ? 0 : 1;
            this._limit = double.Parse(tb_limit.Text) / 100;
            this._adjustmentMethod = rbtn_constraint.Checked ? 0 : 1;
            TransfEvent(_power, _limit, _level, _adjustmentMethod);
            this.Close();
        }
    }
}
