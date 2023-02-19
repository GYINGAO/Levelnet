namespace LevelnetAdjustment.form {
  partial class FrmShowKnowPoint {
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.button2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.PointName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Height = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.IsImport = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(315, 451);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.button2, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.button1, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 404);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(309, 44);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // button2
      // 
      this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button2.Location = new System.Drawing.Point(169, 3);
      this.button2.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(125, 38);
      this.button2.TabIndex = 1;
      this.button2.Text = "取消";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // button1
      // 
      this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button1.Location = new System.Drawing.Point(15, 3);
      this.button1.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
      this.button1.MaximumSize = new System.Drawing.Size(124, 38);
      this.button1.MinimumSize = new System.Drawing.Size(124, 38);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(124, 38);
      this.button1.TabIndex = 0;
      this.button1.Text = "导入";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToOrderColumns = true;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PointName,
            this.Height,
            this.IsImport});
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(3, 3);
      this.dataGridView1.Name = "dataGridView1";
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
      this.dataGridView1.RowTemplate.Height = 23;
      this.dataGridView1.Size = new System.Drawing.Size(309, 395);
      this.dataGridView1.TabIndex = 1;
      this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
      this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
      // 
      // PointName
      // 
      this.PointName.DataPropertyName = "Number";
      this.PointName.HeaderText = "点名";
      this.PointName.Name = "PointName";
      // 
      // Height
      // 
      this.Height.DataPropertyName = "Height";
      dataGridViewCellStyle2.Format = "N5";
      dataGridViewCellStyle2.NullValue = "-1";
      this.Height.DefaultCellStyle = dataGridViewCellStyle2;
      this.Height.HeaderText = "高程";
      this.Height.Name = "Height";
      // 
      // IsImport
      // 
      this.IsImport.DataPropertyName = "Enable";
      this.IsImport.HeaderText = "是否导入";
      this.IsImport.Name = "IsImport";
      this.IsImport.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      // 
      // FrmShowKnowPoint
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(315, 451);
      this.Controls.Add(this.tableLayoutPanel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrmShowKnowPoint";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "已知点信息";
      this.Load += new System.EventHandler(this.FrmShowKnowPoint_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.DataGridViewTextBoxColumn PointName;
    private System.Windows.Forms.DataGridViewTextBoxColumn Height;
    private System.Windows.Forms.DataGridViewCheckBoxColumn IsImport;
  }
}