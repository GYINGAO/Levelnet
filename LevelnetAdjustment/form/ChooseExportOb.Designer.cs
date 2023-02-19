namespace LevelnetAdjustment.form {
  partial class ChooseExportOb {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.StartPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.EndPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Dis = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.HeightDiff = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.IsExport = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(444, 481);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToResizeColumns = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StartPoint,
            this.EndPoint,
            this.Dis,
            this.HeightDiff,
            this.IsExport});
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(3, 3);
      this.dataGridView1.Name = "dataGridView1";
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
      this.dataGridView1.RowTemplate.Height = 23;
      this.dataGridView1.Size = new System.Drawing.Size(438, 425);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
      // 
      // StartPoint
      // 
      this.StartPoint.DataPropertyName = "StartPoint";
      this.StartPoint.HeaderText = "起点";
      this.StartPoint.Name = "StartPoint";
      // 
      // EndPoint
      // 
      this.EndPoint.DataPropertyName = "EndPoint";
      this.EndPoint.HeaderText = "终点";
      this.EndPoint.Name = "EndPoint";
      // 
      // Dis
      // 
      this.Dis.DataPropertyName = "Dis";
      dataGridViewCellStyle1.Format = "N4";
      dataGridViewCellStyle1.NullValue = "-1";
      this.Dis.DefaultCellStyle = dataGridViewCellStyle1;
      this.Dis.HeaderText = "距离/km";
      this.Dis.Name = "Dis";
      // 
      // HeightDiff
      // 
      this.HeightDiff.DataPropertyName = "HeightDiff";
      dataGridViewCellStyle2.Format = "N5";
      dataGridViewCellStyle2.NullValue = "-1";
      this.HeightDiff.DefaultCellStyle = dataGridViewCellStyle2;
      this.HeightDiff.HeaderText = "高差/m";
      this.HeightDiff.Name = "HeightDiff";
      // 
      // IsExport
      // 
      this.IsExport.DataPropertyName = "IsExport";
      this.IsExport.HeaderText = "是否导出";
      this.IsExport.Name = "IsExport";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.button1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.button2, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 434);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(438, 44);
      this.tableLayoutPanel2.TabIndex = 1;
      // 
      // button1
      // 
      this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button1.Location = new System.Drawing.Point(20, 3);
      this.button1.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(179, 38);
      this.button1.TabIndex = 0;
      this.button1.Text = "确定";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button2.Location = new System.Drawing.Point(239, 3);
      this.button2.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(179, 38);
      this.button2.TabIndex = 1;
      this.button2.Text = "取消";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // ChooseExportOb
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(444, 481);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "ChooseExportOb";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "选择";
      this.Load += new System.EventHandler(this.ChooseExport_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    public System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.DataGridViewTextBoxColumn StartPoint;
    private System.Windows.Forms.DataGridViewTextBoxColumn EndPoint;
    private System.Windows.Forms.DataGridViewTextBoxColumn Dis;
    private System.Windows.Forms.DataGridViewTextBoxColumn HeightDiff;
    private System.Windows.Forms.DataGridViewCheckBoxColumn IsExport;
  }
}