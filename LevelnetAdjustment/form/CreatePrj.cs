using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
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

namespace LevelnetAdjustment.form {
  public delegate void TransfDelegateProject(ProjectInfo project);
  public partial class CreatePrj : Form {
    public ProjectInfo Project { get; set; }

    public event TransfDelegateProject TransfEvent;
    public CreatePrj(ProjectInfo project) {
      this.Project = project;
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      FolderBrowserDialog folder = new FolderBrowserDialog() {
        ShowNewFolderButton = true,
        Description = "请选择项目路径",
      };
      if (folder.ShowDialog() == DialogResult.OK) {
        this.tbpath.Text = folder.SelectedPath;
      }
    }

    private void CreatePrj_Load(object sender, EventArgs e) {
      // 控制日期或时间的显示格式
      this.dtp_mt.CustomFormat = "yyyy-MM-dd HH:mm";
      this.dtp_pt.CustomFormat = "yyyy-MM-dd HH:mm";
      //使用自定义格式
      this.dtp_mt.Format = DateTimePickerFormat.Custom;
      this.dtp_pt.Format = DateTimePickerFormat.Custom;

      if (Project == null) {
        return;
      }
      this.tbpath.Text = Project.Path;
      this.tbname.Text = Project.Name;
      this.tbcomp.Text = Project.Company;
      this.tbmp.Text = Project.MeasurementPerson;
      this.dtp_mt.Value = Project.MeasurementTime;
      this.tb_pp.Text = Project.ProcessingPerson;
      this.dtp_pt.Value = Project.ProcessingTime;
    }

    private void btn_con_Click(object sender, EventArgs e) {
      if (tbpath.Text.Trim() == "") throw new Exception("请选择项目路径");
      if (tbname.Text.Trim() == "") throw new Exception("请输入项目名");
      if (!Directory.Exists(tbpath.Text.Trim())) throw new Exception("项目路径不合法");
      var basepath = Path.Combine(tbpath.Text.Trim(), tbname.Text.Trim());
      if (Directory.Exists(basepath)) {
        var msg = MessageBox.Show("存在相同的项目，是否覆盖？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (msg == DialogResult.Yes) {
          //删除目录及所有文件
          DirectoryInfo di = new DirectoryInfo(basepath);
          di.Delete(true);
          Directory.CreateDirectory(basepath);
          Directory.CreateDirectory(Path.Combine(basepath, "ImportFiles"));
          Directory.CreateDirectory(Path.Combine(basepath, "ExportFiles"));
        }
        else {
          return;
        }
      }
      else {
        Directory.CreateDirectory(basepath);
        Directory.CreateDirectory(Path.Combine(basepath, "ImportFiles"));
        Directory.CreateDirectory(Path.Combine(basepath, "ExportFiles"));
      }
      Project = new ProjectInfo {
        Path = tbpath.Text.Trim(),
        Name = tbname.Text.Trim(),
        Company = tbcomp.Text,
        MeasurementPerson = tbmp.Text.Trim(),
        MeasurementTime = dtp_mt.Value,
        ProcessingPerson = tb_pp.Text.Trim(),
        ProcessingTime = dtp_pt.Value
      };
      JsonHelper.WriteJson(Project);
      TransfEvent(Project);
      Close();
    }

    private void btn_cal_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
