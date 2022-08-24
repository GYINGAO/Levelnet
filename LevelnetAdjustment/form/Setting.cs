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
    public delegate void TransfDelegate(int method, double limit);
    public partial class Setting : Form {
        public int _power { get; set; }
        public double _limit { get; set; }

        public event TransfDelegate TransfEvent;
        public Setting(int power, double limit) {
            this._power = power;
            this._limit = limit;
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e) {
            if (_power == 0) {
                this.rbtn_dis.Checked = true;
                this.rbtn_num.Checked = false;
            }
            else {
                this.rbtn_dis.Checked = false;
                this.rbtn_num.Checked = true;
            }
            this.tb_limit.Text = _limit.ToString();
        }

        private void btn_cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e) {
            if (rbtn_dis.Checked) {
                TransfEvent(0, double.Parse(tb_limit.Text));
            }
            else if (rbtn_num.Checked) {
                TransfEvent(1, double.Parse(tb_limit.Text));
            }
            this.Close();
        }
    }
}
