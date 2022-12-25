using LevelnetAdjustment.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using LevelnetAdjustment.utils;

namespace LevelnetAdjustment.form {
    public delegate void ChangeStable(List<PointData> Points);
    public partial class ChooseStablePoint : Form {
        public List<PointData> Points { get; set; }
        public event ChangeStable TransfChangeStable;

        public ChooseStablePoint(List<PointData> points) {
            this.Points = points;
            InitializeComponent();
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void ChooseStablePoint_Load(object sender, EventArgs e) {
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = new BindingList<PointData>(Points);
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              dataGridView1.RowHeadersWidth - 4,
              e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                  (e.RowIndex + 1).ToString(),
                   dataGridView1.RowHeadersDefaultCellStyle.Font,
                   rectangle,
                   dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                   TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            TransfChangeStable(Points);
            this.Close();
        }
    }
}
