using LevelnetAdjustment.model;
using System;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
    public delegate void SetObsDataLimit(ObsDataLimit limit);
    public partial class FrmCheckObsData : Form {
        public ObsDataLimit Limit { get; set; }
        public event SetObsDataLimit TransfEvent;
        public FrmCheckObsData(ObsDataLimit limit) {
            Limit = limit;
            InitializeComponent();
        }

        private void FrmCheckObsData_Load(object sender, EventArgs e) {
            textBox1.Text = Limit.FBDis.ToString();
            textBox2.Text = Limit.FBDisSum.ToString();
            textBox3.Text = Limit.StafLow.ToString();
        }

        private void button1_Click(object sender, EventArgs e) {
            Limit.FBDis = double.Parse(textBox1.Text);
            Limit.FBDisSum = double.Parse(textBox2.Text);
            Limit.StafLow = double.Parse(textBox3.Text);
            Close();
            TransfEvent(Limit);
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
