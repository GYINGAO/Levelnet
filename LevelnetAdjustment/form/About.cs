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
    public partial class About : Form {
        public About() {
            InitializeComponent();
        }


        private void About_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Escape) {
                this.Close();
            }
        }
    }
}
