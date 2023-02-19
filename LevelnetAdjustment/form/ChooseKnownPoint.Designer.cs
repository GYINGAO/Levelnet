
namespace LevelnetAdjustment.form {
  partial class ChooseKnownPoint {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseKnownPoint));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.添加单个点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.导入文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Height = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Enable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.menuStrip1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加单个点ToolStripMenuItem,
            this.导入文件ToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(350, 25);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // 添加单个点ToolStripMenuItem
      // 
      this.添加单个点ToolStripMenuItem.Name = "添加单个点ToolStripMenuItem";
      this.添加单个点ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
      this.添加单个点ToolStripMenuItem.Text = "添加单个点";
      this.添加单个点ToolStripMenuItem.Click += new System.EventHandler(this.添加单个点ToolStripMenuItem_Click);
      // 
      // 导入文件ToolStripMenuItem
      // 
      this.导入文件ToolStripMenuItem.Name = "导入文件ToolStripMenuItem";
      this.导入文件ToolStripMenuItem.Size = new System.Drawing.Size(104, 21);
      this.导入文件ToolStripMenuItem.Text = "导入已知点文件";
      this.导入文件ToolStripMenuItem.Click += new System.EventHandler(this.导入文件ToolStripMenuItem_Click);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(350, 456);
      this.tableLayoutPanel1.TabIndex = 3;
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
            this.Number,
            this.Height,
            this.Enable});
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(3, 3);
      this.dataGridView1.Name = "dataGridView1";
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
      this.dataGridView1.RowTemplate.Height = 23;
      this.dataGridView1.Size = new System.Drawing.Size(344, 400);
      this.dataGridView1.TabIndex = 12;
      this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
      this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
      // 
      // Number
      // 
      this.Number.DataPropertyName = "Number";
      this.Number.HeaderText = "已知点名";
      this.Number.Name = "Number";
      // 
      // Height
      // 
      this.Height.DataPropertyName = "Height";
      dataGridViewCellStyle2.Format = "N5";
      dataGridViewCellStyle2.NullValue = null;
      this.Height.DefaultCellStyle = dataGridViewCellStyle2;
      this.Height.HeaderText = "高程";
      this.Height.Name = "Height";
      // 
      // Enable
      // 
      this.Enable.DataPropertyName = "Enable";
      this.Enable.HeaderText = "选择已知点";
      this.Enable.Name = "Enable";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.button1, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.button2, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 409);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(344, 44);
      this.tableLayoutPanel2.TabIndex = 13;
      // 
      // button1
      // 
      this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button1.Location = new System.Drawing.Point(192, 3);
      this.button1.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(132, 38);
      this.button1.TabIndex = 0;
      this.button1.Text = "取消";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button2.Location = new System.Drawing.Point(20, 3);
      this.button2.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(132, 38);
      this.button2.TabIndex = 1;
      this.button2.Text = "确定";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // ChooseKnownPoint
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(350, 481);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Controls.Add(this.menuStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip1;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(300, 300);
      this.Name = "ChooseKnownPoint";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "选择已知点";
      this.Load += new System.EventHandler(this.ChooseKnownPoint_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.DataGridViewTextBoxColumn Number;
    private System.Windows.Forms.DataGridViewTextBoxColumn Height;
    private System.Windows.Forms.DataGridViewCheckBoxColumn Enable;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.ToolStripMenuItem 添加单个点ToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem 导入文件ToolStripMenuItem;
  }
}