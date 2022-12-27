using ExcelDataReader;
using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LevelnetAdjustment.form {
  public partial class ExcelView : Form {
    public string FilePath { get; set; }
    public DataTableCollection TableCollection { get; set; }
    public ExcelView(string filePath) {
      InitializeComponent();
      FilePath = filePath;
      dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      dataGridView1.Dock = DockStyle.Fill;
    }

    private void ExcelView_Load(object sender, EventArgs e) {
      this.Text = Path.GetFileName(FilePath);
      dataGridView1.Rows.Clear();
      dataGridView1.Columns.Clear();
      using (var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read)) {
        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream)) {
          DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration() {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
          });
          TableCollection = result.Tables;
          cboSheet.Items.Clear();
          foreach (DataTable table in TableCollection)
            cboSheet.Items.Add(table.TableName);
          cboSheet.SelectedIndex = 0;
        }
      }
    }

    private void cboSheet_SelectedIndexChanged(object sender, EventArgs e) {
      DataTable dt = TableCollection[cboSheet.SelectedItem.ToString()];
      dataGridView1.DataSource = dt;
    }

  }
}
