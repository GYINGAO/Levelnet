namespace LevelnetAdjustment.form {
  partial class ExcelView {
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.cboSheet = new System.Windows.Forms.ComboBox();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.cboSheet, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(857, 510);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // cboSheet
      // 
      this.cboSheet.FormattingEnabled = true;
      this.cboSheet.Location = new System.Drawing.Point(3, 3);
      this.cboSheet.Name = "cboSheet";
      this.cboSheet.Size = new System.Drawing.Size(224, 20);
      this.cboSheet.TabIndex = 0;
      this.cboSheet.SelectedIndexChanged += new System.EventHandler(this.cboSheet_SelectedIndexChanged);
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(3, 33);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowTemplate.Height = 23;
      this.dataGridView1.Size = new System.Drawing.Size(851, 474);
      this.dataGridView1.TabIndex = 1;

      // 
      // ExcelView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(857, 510);
      this.Controls.Add(this.tableLayoutPanel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ExcelView";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "ExcelView";
      this.Load += new System.EventHandler(this.ExcelView_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.ComboBox cboSheet;
    private System.Windows.Forms.DataGridView dataGridView1;
  }
}