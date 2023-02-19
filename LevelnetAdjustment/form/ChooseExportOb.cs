﻿using LevelnetAdjustment.model;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
  public delegate void DelegateExportOb(List<ExportObserve> lists);
  public partial class ChooseExportOb : Form {

    public event DelegateExportOb TransfEvent;
    public List<ExportObserve> ObList { get; set; }
    public ChooseExportOb(List<ExportObserve> lists) {
      this.ObList = lists;
      InitializeComponent();
      //利用反射设置DataGridView的双缓冲
      Type dgvType = this.dataGridView1.GetType();
      PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
          BindingFlags.Instance | BindingFlags.NonPublic);
      pi.SetValue(this.dataGridView1, true, null);

    }

    private void ChooseExport_Load(object sender, EventArgs e) {
      this.dataGridView1.AutoGenerateColumns = false;
      this.dataGridView1.DataSource = new BindingList<ExportObserve>(ObList);
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

    private void button1_Click(object sender, EventArgs e) {
      TransfEvent(ObList);
      Close();
    }

    private void button2_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
