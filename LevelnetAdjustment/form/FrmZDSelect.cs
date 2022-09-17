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

    public delegate void SelectZD(string zd);

    public partial class FrmZDSelect : Form {
        public event SelectZD TransfEvevn;
        public FrmZDSelect() {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e) {
            Close();
            if (radioButton1.Checked) {
                TransfEvevn("num");
            }
            else {
                TransfEvevn(comboBox1.Text);
            }         
        }

        private void FrmZDSelect_Load(object sender, EventArgs e) {
            radioButton2.Checked = true;
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
