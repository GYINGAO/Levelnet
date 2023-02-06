using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using SplashScreenDemo;
using System;
using System.Collections;
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

  public partial class FrmModifyPointName : Form {
    public delegate void ModifyPointaName(ManualModify manualModification);
    public List<RawData> Rds { get; set; }
    public List<PointData> Knownp { get; set; }
    public ManualModify ManualModification { get; set; }

    public string FilePath { get; set; }

    public event ModifyPointaName TransEvent;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="rds">观测数据</param>
    /// <param name="knownp">已知点</param>
    /// <param name="file">导出文件路径</param>
    /// <param name="pls">用户修改的点名</param>
    public FrmModifyPointName(List<RawData> rds, List<PointData> knownp, string file, ManualModify manualModification) {
      InitializeComponent();
      Rds = rds;
      ManualModification = manualModification;
      Knownp = knownp;
      FilePath = file;
    }

    private void FrmModifyPointName_Load(object sender, EventArgs e) {
      SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();

      //Console.WriteLine("123");
      panel1.AutoScroll = true;
      for (int i = 0; i < ManualModification.ChangedStationLine.Count; i++) {
        int txtY = 10 + 40 * i;
        int lblY = 15 + 40 * i;
        int idx = -1;
        for (int j = 0; j < ManualModification.ChangedStationLine[i].Count; j++) {
          idx++;
          int txtX = 10 + 100 * j;
          int lblX = 90 + 100 * j;
          TextBox text = new TextBox {
            Name = $"txt-{i}-{j}",
            Text = ManualModification.ChangedStationLine[i][j].ToString(),
            Location = new Point(txtX, txtY),
            Size = new Size(75, 21),
          };
          text.Enter += new EventHandler(textbox_enter);
          text.Leave += new EventHandler(textbox_leave);
          panel1.Controls.Add(text);

          if (idx != ManualModification.ChangedStationLine[i].Count - 1) {
            Label lbl = new Label {
              Name = $"lbl-{i}-{j}",
              Text = "→",
              Location = new Point(lblX, lblY),
              Size = new Size(17, 12),
            };
            panel1.Controls.Add(lbl);
          }
        }
      }
      button1_Click(sender, e);
      loading.CloseWaitForm();

    }

    private void textbox_enter(object sender, EventArgs e) {

    }
    private void textbox_leave(object sender, EventArgs e) {
      TextBox textBox = (TextBox)sender;
      string[] str = textBox.Name.Split('-');
      int i = int.Parse(str[1]);
      int j = int.Parse(str[2]);
      if (ManualModification.ChangedStationLine[i][j].ToString().Trim() != textBox.Text) {
        ChangedPoint point = new ChangedPoint {
          Value = textBox.Text.Trim(),
          ControlName = textBox.Name,
        };
        var idx = ManualModification.ChangedPoints.FindIndex(p => p.ControlName == textBox.Name);
        if (idx == -1) {
          ManualModification.ChangedPoints.Add(point);
        }
        else {
          ManualModification.ChangedPoints[idx].Value = textBox.Text;
        }
      }
      ManualModification.ChangedStationLine[i][j] = textBox.Text.Trim();
    }

    private void button1_Click(object sender, EventArgs e) {
      ManualModification.ChangedPoints.ForEach(p => {
        TextBox control = (TextBox)panel1.Controls[p.ControlName];
        control.ForeColor = Color.Black;
      });
      var list = ManualModification.ChangedPoints.getAllRepeated(z => new { z.Value }).ToList();
      list.ForEach(z => {
        TextBox control = (TextBox)panel1.Controls[z.ControlName];
        control.ForeColor = Color.Red;
      });

    }

    private void button2_Click(object sender, EventArgs e) {
      List<ObservedData> ods = new List<ObservedData>();
      List<ObservedData> ods_mid = new List<ObservedData>();

      int stationNum = -1;
      int num = 0;
      foreach (var item in Rds) {
        if (item.MidDis != 0) {
          ods_mid.Add(new ObservedData { Start = item.BackPoint, End = item.MidPoint, Distance = item.MidDis, HeightDiff = item.MidDiff, StationNum = 1 });
        }
        if (item.IsStart) {
          stationNum++;
          num = 0;
        }
        num++;
        ods.Add(new ObservedData {
          Start = ManualModification.ChangedStationLine[stationNum][num - 1].ToString(),
          End = ManualModification.ChangedStationLine[stationNum][num].ToString(),
          Distance = item.DisAve,
          HeightDiff = item.DiffAve,
          StationNum = 1
        });
      }

      FileStream fileStream = new FileStream(FilePath, FileMode.Create);
      StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
      if (Knownp != null && Knownp.Count > 0) {
        Knownp.ForEach(l => {
          streamWriter.WriteLine($"{l.Number},{l.Height}");
        });
      }
      Commom.Merge(ods, ods_mid).ForEach(l => {
        streamWriter.WriteLine($"{l.Start},{l.End},{Math.Round(l.HeightDiff, 5)},{Math.Round(l.Distance, 5)}");
      });
      streamWriter.Flush();
      streamWriter.Close();
      fileStream.Close();

      Close();
      TransEvent(ManualModification);

    }

    private void button3_Click(object sender, EventArgs e) {
      Close();
    }

    private void 自动修改转点名ToolStripMenuItem_Click(object sender, EventArgs e) {
      int idx = 1;
      for (int i = 0; i < ManualModification.ChangedStationLine.Count; i++) {
        for (int j = 0; j < ManualModification.ChangedStationLine[i].Count; j++) {
          if (ManualModification.ChangedStationLine[i][j].ToString().ToLower().Contains('z')) {
            string newName = $"Z{idx.ToString().PadLeft(3, '0')}";
            ManualModification.ChangedStationLine[i][j] = newName;
            ((TextBox)(panel1.Controls.Find($"txt-{i}-{j}", false))[0]).Text = newName;
            idx++;
          }
        }
      }
    }

    private void button4_Click(object sender, EventArgs e) {
      自动修改转点名ToolStripMenuItem_Click(sender, e);
    }
  }


  //静态扩展类
  public static class Extention {
    public static IEnumerable<T> getMoreThanOnceRepeated<T>(this IEnumerable<T> extList, Func<T, object> groupProps) where T : class { //返回第二个以后面的重复的元素集合
      return extList
          .GroupBy(groupProps)
          .SelectMany(z => z.Skip(1)); //跳过第一个重复的元素
    }
    public static IEnumerable<T> getAllRepeated<T>(this IEnumerable<T> extList, Func<T, object> groupProps) where T : class {
      //返回所有重复的元素集合
      return extList
          .GroupBy(groupProps)
          .Where(z => z.Count() > 1) //Filter only the distinct one
          .SelectMany(z => z);//All in where has to be retuned
    }
  }
}
