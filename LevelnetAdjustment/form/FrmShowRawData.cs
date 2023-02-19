using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using SplashScreenDemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
  public partial class FrmShowRawData : Form {
    public ClevelingAdjust ClAdj { get; set; }
    public List<RawData> AddRawDatas { get; set; } = new List<RawData>();
    public ProjectInfo Project { get; set; }
    public string[] FileNames { get; set; }
    public int maxPointNameLength { get; set; } = 0;
    public FrmShowRawData(ClevelingAdjust cladj, string[] fileNames, ProjectInfo project) {
      ClAdj = cladj;
      InitializeComponent();
      FileNames = fileNames;
      Project = project;
    }

    private void FrmRawData_Load(object sender, EventArgs e) {
      SimpleLoading loadingfrm = new SimpleLoading(this, "读取中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();
      try {

        //for (int i = ClAdj.Options.ImportFiles.Count - 1; i >= 0; i--) {
        //  string ext = Path.GetExtension(ClAdj.Options.ImportFiles[i].FileName).ToLower();
        //  if (ext.Contains("dat") || ext.Contains("gsi")) {
        //    ClAdj.Options.ImportFiles.RemoveAt(i);
        //  }
        //}

        //读取数据
        foreach (var item in FileNames) {
          if (ClAdj.Options.ImportFiles.Exists(t => t.FileName == Path.GetFileName(item))) {
            continue;
          }
          int length = 0;
          switch (Path.GetExtension(item).ToLower()) {
            case ".dat":
              length = FileHelper.ReadDAT(AddRawDatas, item);
              maxPointNameLength = length > maxPointNameLength ? length : maxPointNameLength;
              break;
            case ".gsi":
              FileHelper.ReadGSI(item, AddRawDatas);
              break;
            default:
              break;
          }
        }

        if (AddRawDatas.Count == 0) {
          loading.CloseWaitForm();
          MessageBox.Show("数据重复", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          Close();
          return;
        }

        //显示数据
        StringBuilder sb = new StringBuilder();
        int stationNum = 0;
        double total = 0;
        AddRawDatas.ForEach(r => {
          stationNum++;
          if (r.IsStart) {
            sb.AppendLine($"测站   后视    前视      后尺读数1   后视距1   前尺读数1   前视距1   后尺读数2   后视距2   前尺读数2   前视距2  前后视距差 累计前后视距差");
            sb.AppendLine("Start-Line");
          }
          double dis = r.DisDiffAve * 1000;
          total += dis;
          string backdiff1 = r.BackDiff1.ToString("#0.00000");
          string backdiff2 = r.BackDiff2.ToString("#0.00000");
          string frontdiff1 = r.FrontDiff1.ToString("#0.00000");
          string frontdiff2 = r.FrontDiff2.ToString("#0.00000");
          string disdiff = dis.ToString("#0.000");
          string totaldis = total.ToString("#0.000");
          sb.AppendLine($"{stationNum,-5}{r.BackPoint,-10}{r.FrontPoint,-8}{backdiff1,-12}{r.BackDis1 * 1000,-10:#0.000}{frontdiff1,-12}{r.FrontDis1 * 1000,-10:#0.000}{backdiff2,-12}{r.BackDis2 * 1000,-10:#0.000}{frontdiff2,-12}{r.FrontDis2 * 1000,-10:#0.000}{disdiff,-10}{totaldis,-10}");
          if (r.IsEnd) {
            sb.AppendLine("End-Line");
            sb.AppendLine("");
            total = 0;
          }
        });
        richTextBox1.Text = sb.ToString();

        loading.CloseWaitForm();
      }
      catch (Exception ex) {
        loading.CloseWaitForm();
        throw ex;
      }
    }

    private void button1_Click(object sender, EventArgs e) {
      AddRawDatas.ForEach(t => {
        if (t.DataType == DataTypeEnum.GSI) {
          int b = t.BackPoint.Length - maxPointNameLength;
          if (b > 0) {
            t.BackPoint = t.BackPoint.Substring(b, t.BackPoint.Length - 1);
          }
          int f = t.FrontPoint.Length - maxPointNameLength;
          if (f > 0) {
            t.FrontPoint = t.FrontPoint.Substring(f, t.FrontPoint.Length - 1);
          }
        }
      });
      ClAdj.RawDatas = Commom.Merge(ClAdj.RawDatas, AddRawDatas);
      foreach (var item in FileNames) {
        if (ClAdj.Options.ImportFiles.Exists(t => t.FileName == Path.GetFileName(item))) {
          continue;
        }
        //把文件复制到项目文件夹中
        FileInfo fileInfo1 = new FileInfo(item);
        string targetPath = Path.Combine(Project.Path, Project.Name, "ImportFiles", Path.GetFileName(item));
        if (File.Exists(targetPath)) File.Delete(targetPath);
        fileInfo1.CopyTo(targetPath);
        ClAdj.Options.ImportFiles.Add(new InputFile {
          FilePath = item,
          FileName = Path.GetFileName(item)
        });
      }
      Close();
      MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button2_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
