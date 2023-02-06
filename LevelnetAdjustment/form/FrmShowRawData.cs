using LevelnetAdjustment.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
  public partial class FrmShowRawData : Form {
    public List<RawData> RawDatas { get; set; }
    public FrmShowRawData(List<RawData> rawDatas) {
      RawDatas = rawDatas;
      InitializeComponent();
    }

    private void FrmRawData_Load(object sender, EventArgs e) {
      StringBuilder sb = new StringBuilder();
      int stationNum = 0;
      double total = 0;
      RawDatas.ForEach(r => {
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
    }
  }
}
