using LevelnetAdjustment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplashScreenDemo {
    public partial class SimpleLoading : Form {
        //保存父窗口信息，主要用于居中显示加载窗体
        private Form partentForm = null;
        private string tips = "";
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        Timer timer = new Timer();
        int seconds = 0;
        public SimpleLoading(Form partentForm, string tips) {
            InitializeComponent();
            this.partentForm = partentForm;
            this.tips = tips;
        }
        private void SimpleLoading_Load(object sender, EventArgs e) {
            //设置一些Loading窗体信息
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ControlBox = false;
            this.lbl_tips.Text = tips;
            // 下面的方法用来使得Loading窗体居中父窗体显示
            int parentForm_Position_x = this.partentForm.Location.X;
            int parentForm_Position_y = this.partentForm.Location.Y;
            int parentForm_Width = this.partentForm.Width;
            int parentForm_Height = this.partentForm.Height;

            int start_x = (int)(parentForm_Position_x + (parentForm_Width - this.Width) / 2);
            int start_y = (int)(parentForm_Position_y + (parentForm_Height - this.Height) / 2);
            this.Location = new System.Drawing.Point(start_x, start_y);

            // 开始计时器
            timer.Interval = 1000;
            timer.Tick += new EventHandler(this.timer_Tick);
            timer.Start();

            //在主窗体加载后开始计时
            //sw.Start();
        }

        private void timer_Tick(object sender, EventArgs e) {
            //TimeSpan ts = sw.Elapsed;
            seconds++;
            /*TimeSpan ts = TimeSpan.FromSeconds(seconds);
            lbl_tips_son.Text = $"Please Waitting...{ts.Days * 86400 + ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds}s";*/
            lbl_tips_son.Text = $"Please Waitting...{seconds}s";
        }

        private void button1_Click(object sender, EventArgs e) {
            MainForm.flag = false;
        }
    }
}
