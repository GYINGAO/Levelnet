using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LevelnetAdjustment.form {
  public partial class MultiPeriodComparison : Form {
    public MultiPeriodComparison() {
      InitializeComponent();
      //利用反射设置DataGridView的双缓冲
      Type dgvType = this.dataGridView1.GetType();
      PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
          BindingFlags.Instance | BindingFlags.NonPublic);
      pi.SetValue(this.dataGridView1, true, null);
      dataGridView1.AutoGenerateColumns = false;
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

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {
      if (e.RowIndex == -1) return;
      DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
      if (cell == null) return;
      if (cell is DataGridViewLinkCell && cell.FormattedValue.ToString() == "查看") {
        List<ChartData> chartDatas = new List<ChartData>();
        for (int i = 1; i < e.ColumnIndex; i++) {
          var y = dataGridView1.Rows[e.RowIndex].Cells[i];
          ChartData chartData = new ChartData() { X = dataGridView1.Columns[i].HeaderText.ToString(), Y = double.Parse(dataGridView1.Rows[e.RowIndex].Cells[i].Value.ToString()) };
          chartDatas.Add(chartData);
        }
        //新建窗体
        Form form = new Form() {
          Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(),
          ShowIcon = false,
          Size = new Size(chartDatas.Count * 200, 400),
          StartPosition = FormStartPosition.CenterParent,
        };
        //新建图表控件
        Chart chart = new Chart() {
          Dock = DockStyle.Fill,
          DataSource = chartDatas
        };

        chart.Series.Clear();
        ChartArea chartArea = new ChartArea();
        //是否显示网格
        chartArea.AxisX.MajorGrid.Enabled = false;
        chartArea.AxisY.MajorGrid.Enabled = true;
        //网格类型
        chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
        chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
        //设置背景颜色
        chartArea.BackColor = Color.Transparent;
        //设置显示范围
        chartArea.AxisY.Minimum = chartDatas.Min(t => t.Y) - 0.5;
        chartArea.AxisY.Maximum = chartDatas.Max(t => t.Y) + 0.5;

        //设置坐标偏移
        //chartArea.AxisX.IntervalOffset= 1.00D;

        chart.ChartAreas.Add(chartArea);

        Series series = new Series("Chart") {
          XValueMember = "X",
          YValueMembers = "Y",
          IsVisibleInLegend = true,
          ChartType = SeriesChartType.Line,
          XValueType = ChartValueType.String,
          Label = "#VAL",
          ToolTip = "#VALX：#VAL",
          ChartArea = chartArea.Name,
          BorderWidth = 3,
          MarkerBorderColor = Color.Red,
          MarkerColor = Color.Red,
          MarkerBorderWidth = 3,
          MarkerSize = 5,
          MarkerStyle = MarkerStyle.Circle

        };
        chart.Series.Add(series);
        form.Controls.Add(chart);

        form.ShowDialog();
      }
    }
  }

  public class ChartData {
    public string X { get; set; }
    public double Y { get; set; }

  }
}
