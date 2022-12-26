using LevelnetAdjustment.model;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.utils {
  public class FileHelper {




    /// <summary>
    /// 读取COSA文件
    /// </summary>
    /// <param name="knownPoints"></param>
    /// <param name="observedDatas"></param>
    /// <param name="fileName"></param>
    internal static void ReadOriginalFile(List<PointData> knownPoints, List<ObservedData> observedDatas, string fileName) {
      try {
        using (StreamReader sr = new StreamReader(fileName)) {
          string line;
          string[] dataArray;
          while (sr.Peek() > -1) {
            line = sr.ReadLine().Trim();
            dataArray = Regex.Split(line, "(?:\\s*[,|，]\\s*)", RegexOptions.IgnoreCase); // 正则匹配逗号（允许空格）
            if (dataArray.Length == 2) {
              PointData knownPoint = new PointData {
                Number = dataArray[0].TrimStart('0'),
                Height = double.Parse(dataArray[1])
              };
              knownPoints.Add(knownPoint);
            }
            else {
              if (observedDatas.Exists(p => p.End == dataArray[0] && p.Start == dataArray[1]) && observedDatas.Exists(p => p.Start == dataArray[0] && p.End == dataArray[1])) {
                continue;
              }
              ObservedData observedData = new ObservedData {
                Start = dataArray[0].TrimStart('0'),
                End = dataArray[1].TrimStart('0'),
                HeightDiff = double.Parse(dataArray[2]),
                Distance = double.Parse(dataArray[3])
              };
              if (dataArray.Length >= 5) {
                observedData.StationNum = int.Parse(dataArray[4]);

              }
              observedDatas.Add(observedData);
            }
          }
        }

      }
      catch (Exception) {
        throw new Exception("文件格式有误");
      }
    }

    /// <summary>
    /// 读取DAT
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    internal static void ReadDAT(List<RawData> dats, string filename) {
      string[] method1 = { "bffb", "bfbf", "fbbf", "abffb", "abfbf", "abf", "bf" };
      string[] method2 = { "bbff", "ffbb" };
      int ct = dats.Count;

      using (StreamReader sr = new StreamReader(filename)) {
        int stationIdx = 0;//测站索引,记录一个测站观测索引
        int stationNum = 0;//一个测段测站数
        int observeCount = 0;//每站观测次数
        string method = "";

        // string method;//观测方式 aBFFB aFBBF BFBF BBFF FBBF
        while (sr.Peek() > -1) {
          string line = sr.ReadLine().Trim();
          //跳过空行
          if (string.IsNullOrEmpty(line)) {
            continue;
          }
          string[] arr = Regex.Split(line, "(?:\\s*[|]\\s*)", RegexOptions.IgnoreCase);
          //测段开始
          if (line.Contains("Start-Line") || line.Contains("Start")) {
            method = Regex.Split(arr[2], "[\\s]+", RegexOptions.IgnoreCase)[1];
            stationIdx = 0;
            observeCount = method.Length > 2 ? 4 : 2;
            stationNum = 0;
            continue;
          }


          //整个测站重新测量
          if (line.Contains("Station repeated")) {
            //判断是否已经增加了一个测站，如果是，移除
            if (stationIdx != 0) {
              dats.RemoveAt(dats.Count - 1);//移除最后一站
              dats.Add(new RawData());
              stationIdx = 0;
            }
            continue;
          }
          //单次重新测量
          if (line.Contains("Measurement repeated")) {
            continue;
          }
          var str3 = arr[3].Trim();
          var str2 = arr[2].Trim();
          // 读取测量数据
          if (str3.Contains("Rb") || str3.Contains("Rf")) {
            // 不要的测量数据
            if (line.Contains("#####")) {
              continue;
            }
            stationIdx++;
            if (stationIdx == 1) {
              dats.Add(new RawData());
              stationNum++;
            }
            var arr3 = Regex.Split(str3, "[\\s]+", RegexOptions.IgnoreCase);
            var arr2 = Regex.Split(str2, "[\\s]+", RegexOptions.IgnoreCase);
            var arr4 = Regex.Split(arr[4].Trim(), "[\\s]+", RegexOptions.IgnoreCase);
            double c1;//高差单位系数，转换为m
            double c2;//距离单位系数,转换为km
            switch (arr3[2]) {
              case "km": c1 = 1000; break;
              case "mm": c1 = 0.001; break;
              default: c1 = 1; break;
            }
            switch (arr4[2]) {
              case "km": c2 = 1; break;
              case "mm": c2 = 0.000001; break;
              default: c2 = 0.001; break;
            }

            if (arr3[0] == "Rb") {
              if (dats[dats.Count - 1].BackDis1 == 0) {
                dats[dats.Count - 1].BackDiff1 = Convert.ToDouble(arr3[1]) * c1;
                dats[dats.Count - 1].BackDis1 = Convert.ToDouble(arr4[1]) * c2;
                dats[dats.Count - 1].BackPoint = arr2[1].TrimStart('0');
              }
              else {
                dats[dats.Count - 1].BackDiff2 = Convert.ToDouble(arr3[1]) * c1;
                dats[dats.Count - 1].BackDis2 = Convert.ToDouble(arr4[1]) * c2;
              }
            }
            else if (arr3[0] == "Rf") {
              if (dats[dats.Count - 1].FrontDis1 == 0) {
                dats[dats.Count - 1].FrontDiff1 = Convert.ToDouble(arr3[1]) * c1;
                dats[dats.Count - 1].FrontDis1 = Convert.ToDouble(arr4[1]) * c2;
                dats[dats.Count - 1].FrontPoint = arr2[1].TrimStart('0');
              }
              else {
                dats[dats.Count - 1].FrontDiff2 = Convert.ToDouble(arr3[1]) * c1;
                dats[dats.Count - 1].FrontDis2 = Convert.ToDouble(arr4[1]) * c2;
              }
            }

            if (stationIdx == observeCount) {
              dats[dats.Count - 1].Calc();
              stationIdx = 0;
            }
          }

          //测段结束
          if (line.Contains("End-Line") || line.Contains("End")) {
            dats[dats.Count - 1].Calc();
            dats[dats.Count - 1].IsEnd = true;
            dats[dats.Count - stationNum].IsStart = true;
            continue;
          }
        }
      }
      dats[ct].IsFileStart = true;
      dats[dats.Count - 1].IsFileFinish = true;
    }

    /// <summary>
    /// 读取GSI-8
    /// </summary>
    /// <param name="item"></param>
    /// <param name="rawDatas"></param>
    /// <param name="observedDatas"></param>
    /// 参考 https://www.docin.com/p-2467208828.html#:~:text=%E5%BE%95%E5%8D%A1DNA03%E7%94%B5,%E5%A4%9A%E4%B8%AA%E6%95%B0%E6%8D%AE%E5%9D%97%E7%BB%84%E6%88%90%E3%80%82
    ///  https://totalopenstation.readthedocs.io/en/stable/input_formats/if_leica_gsi.html
    internal static void ReadGSI(string filename, List<RawData> rds) {
      int ct = rds.Count;
      using (StreamReader sr = new StreamReader(filename)) {
        int stationIdx = 0;//测站索引,记录一个测站观测索引
        int stationNum = 0; //每个测段测站数
        int observeCount = 0;//每站观测次数
        while (sr.Peek() > -1) {
          string line = sr.ReadLine().Trim();
          if (string.IsNullOrEmpty(line)) {
            continue;
          }
          var arr = Regex.Split(line, "[\\s]+", RegexOptions.IgnoreCase);
          if (arr.Length == 1) {
            // 410003+?......4  4=aBFFB
            string n = arr[0].Substring(arr[0].Length - 1, 1); //取最后一位
            observeCount = Convert.ToInt32(n);
            // 这里判断测段结束
            if (stationNum >= 1) {
              rds[rds.Count - 1].IsEnd = true;
              rds[rds.Count - stationNum].IsStart = true;
              stationNum = 0;
            }
          }
          else if (arr.Length == 5) {


            // 331后视1 332前视1 335后视2 336前视2 32视距
            var pointCode = arr[2].Substring(0, 3);
            if (pointCode != "333" && (rds.Count == 0 || (stationIdx == 0 && rds[rds.Count - 1].BackDis1 != 0))) {
              rds.Add(new RawData());
            }
            switch (pointCode) {
              case "331":
                rds[rds.Count - 1].BackPoint = arr[0].Substring(arr[0].Length - 8, 8).TrimStart('0');
                rds[rds.Count - 1].BackDis1 = Convert2Double(arr[1].Substring(arr[1].Length - 9, 9)) / 1000; //转换为km
                rds[rds.Count - 1].BackDiff1 = Convert2Double(arr[2].Substring(arr[2].Length - 9, 9));
                stationIdx++;
                break;
              case "332":
                rds[rds.Count - 1].FrontPoint = arr[0].Substring(arr[0].Length - 8, 8).TrimStart('0'); ;
                rds[rds.Count - 1].FrontDis1 = Convert2Double(arr[1].Substring(arr[1].Length - 9, 9)) / 1000;
                rds[rds.Count - 1].FrontDiff1 = Convert2Double(arr[2].Substring(arr[2].Length - 9, 9));
                stationIdx++;
                break;
              case "335":
                rds[rds.Count - 1].BackDis2 = Convert2Double(arr[1].Substring(arr[1].Length - 9, 9)) / 1000;
                rds[rds.Count - 1].BackDiff2 = Convert2Double(arr[2].Substring(arr[2].Length - 9, 9));
                stationIdx++;
                break;
              case "336":
                rds[rds.Count - 1].FrontDis2 = Convert2Double(arr[1].Substring(arr[1].Length - 9, 9)) / 1000;
                rds[rds.Count - 1].FrontDiff2 = Convert2Double(arr[2].Substring(arr[2].Length - 9, 9));
                stationIdx++;
                break;
              case "333":
                rds[rds.Count - 1].MidPoint = arr[0].Substring(arr[0].Length - 8, 8).TrimStart('0');
                rds[rds.Count - 1].MidDis = Convert2Double(arr[1].Substring(arr[1].Length - 9, 9)) / 1000;
                rds[rds.Count - 1].MidNum = Convert2Double(arr[2].Substring(arr[2].Length - 9, 9));
                rds[rds.Count - 1].GetMid();
                break;
              default:
                break;
            }
            if (stationIdx == observeCount) {
              rds[rds.Count - 1].Calc();
              stationNum++;
              stationIdx = 0;
            }
          }
        }

        rds[rds.Count - 1].IsEnd = true;
        rds[rds.Count - stationNum].IsStart = true;
      }
      rds[ct].IsFileStart = true;
      rds[rds.Count - 1].IsFileFinish = true;

    }


    // 331.28+00097996 无小数点，保留了五位小数 +00097996=+0.97996m
    private static double Convert2Double(string str) {
      // 1 gon = 0.9°
      var n = Regex.Split(str, "[+]|[-]", RegexOptions.IgnoreCase)[1]; //00097996
      var m = double.Parse(n.TrimStart('0')); //97996
      var res = m / 10e4;
      return str.Contains("+") ? res : -res;
    }

    /// <summary>
    /// 写入txt
    /// </summary>
    /// <param name="str"></param>
    /// <param name="file"></param>
    internal static void WriteStrToTxt(string str, string file) {
      FileStream fileStream = new FileStream(file, FileMode.Create);
      StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
      streamWriter.Write(str);
      streamWriter.Flush();
      streamWriter.Close();
      fileStream.Close();
    }

    /// <summary>
    /// 导出cosa 按距离定权
    /// </summary>
    /// <param name="ods"></param>
    /// <param name="pds"></param>
    /// <param name="path"></param>
    internal static void ExportIN1(List<RawData> rds, List<PointData> pds, string zd, string path) {
      List<ObservedData> ods = Calc.Rd2Od(rds, zd);
      FileStream fileStream = new FileStream(path, FileMode.Create);
      StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
      if (pds != null && pds.Count > 0) {
        pds.ForEach(l => {
          streamWriter.WriteLine($"{l.Number},{l.Height}");
        });
      }
      ods.ForEach(l => {
        streamWriter.WriteLine($"{l.Start},{l.End},{Math.Round(l.HeightDiff, 5)},{Math.Round(l.Distance, 5)}");
      });
      streamWriter.Flush();
      streamWriter.Close();
      fileStream.Close();
    }

    public static void Matrix2Txt(double[,] matrix) {
      var file = @"C:\Users\GaoYing\Desktop\矩阵.txt";
      // 如果存在要保存的文件,则删除
      if (File.Exists(file)) {
        File.Delete(file);
      }
      FileStream fileStream = new FileStream(file, FileMode.Create);
      StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
      for (int i = 0; i < matrix.GetLength(0); i++) {
        for (int j = 0; j < matrix.GetLength(1); j++) {
          streamWriter.Write(matrix[i, j].ToString().PadRight(9) + "\t");
        }
        streamWriter.Write("\r\n");
      }
      streamWriter.Flush();
      streamWriter.Close();
      fileStream.Close();
    }

    /// <summary>
    /// 读取已知点数据
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="knownPoints"></param>
    internal static void ReadKnPoints(string fileName, List<PointData> knownPoints) {
      using (StreamReader sr = new StreamReader(fileName)) {
        while (sr.Peek() > -1) {
          string line = sr.ReadLine().Trim();
          if (string.IsNullOrEmpty(line)) {
            continue;
          }
          var arr = Regex.Split(line, "[\\s]+|[,]+|[，]+", RegexOptions.IgnoreCase);
          PointData pd = new PointData {
            Number = arr[0],
            Height = Convert.ToDouble(arr[1]),
          };
          knownPoints.Add(pd);
        }
      }
    }

    /// <summary>
    /// 读取拟稳点数据文件
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="points"></param>
    /// <param name="pds"></param>
    internal static void ReadStablePoint(string filename, List<string> points, List<PointData> pds) {
      using (StreamReader sr = new StreamReader(filename)) {
        while (sr.Peek() > -1) {
          string line = sr.ReadLine().Trim();
          if (string.IsNullOrEmpty(line)) {
            continue;
          }
          points.Add(line);
          try {
            pds.Find(p => p.Number == line).IsStable = true;
          }
          catch (Exception) {
            throw new Exception("出现数据文件中没有的点名");
          }
        }
      }
      if (points.Count > pds.Count || points.Count < 1) {
        throw new Exception("拟稳点数错误");
      }
    }

    /// <summary>
    /// 导出cosa 按测站数定权
    /// </summary>
    /// <param name="observedDatas"></param>
    /// <param name="pds"></param>
    /// <param name="fileName"></param>
    internal static void ExportCOSAStationPower(List<ObservedData> observedDatas, List<PointData> pds, string fileName) {
      FileStream fileStream = new FileStream(fileName, FileMode.Create);
      StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
      if (pds != null && pds.Count > 0) {
        pds.ForEach(l => {
          streamWriter.WriteLine($"{l.Number},{l.Height}");
        });
      }
      observedDatas.ForEach(l => {
        streamWriter.WriteLine($"{l.Start},{l.End},{Math.Round(l.HeightDiff, 5)},{Math.Round(l.Distance, 5)},{l.StationNum}");
      });
      streamWriter.Flush();
      streamWriter.Close();
      fileStream.Close();
    }

    public static void RegisterFileType(string typeName, string fileType, string fileContent, string app, string ico) {
      //工具启动路径
      string toolPath = app;

      string extension = typeName;

      //fileType = "自定义文件类型";

      //fileContent = "AAAA";
      //获取信息
      RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension);

      if (registryKey != null) {
        try {
          RegistryKey _Regkey = Registry.ClassesRoot.OpenSubKey("", true);

          RegistryKey _VRPkey = _Regkey.OpenSubKey(extension);
          if (_VRPkey != null) _Regkey.DeleteSubKeyTree(extension, true);
          if (_VRPkey != null) _Regkey.DeleteSubKeyTree("Exec");
        }
        catch (Exception e) {
          throw e;
        }
      }

      if (registryKey != null && registryKey.OpenSubKey("shell") != null && registryKey.OpenSubKey("shell").OpenSubKey("open") != null &&
          registryKey.OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command") != null) {
        var varSub = registryKey.OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command");
        var varValue = varSub.GetValue("");

        if (Equals(varValue, toolPath + " \"%1\"")) {
          return;
        }
      }

      //文件注册
      registryKey = Registry.ClassesRoot.CreateSubKey(extension);
      registryKey.SetValue("", fileType);
      registryKey.SetValue("Content Type", fileContent);
      //设置默认图标
      RegistryKey iconKey = registryKey.CreateSubKey("DefaultIcon");
      iconKey.SetValue("", Application.StartupPath + $"\\{ico}.ico");
      iconKey.Close();
      //设置默认打开程序路径
      registryKey = registryKey.CreateSubKey("shell\\open\\command");
      registryKey.SetValue("", toolPath + " \"%1\"");
      //关闭
      registryKey.Close();

      SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
    }

    [DllImport("shell32.dll")]
    public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    public static List<ExportHeight> readHeight(string path) {
      List<ExportHeight> heights = new List<ExportHeight>();
      using (StreamReader sr = new StreamReader(path)) {
        string line;
        string[] dataArray;
        sr.ReadLine();
        while (sr.Peek() > -1) {
          line = sr.ReadLine().Trim();
          dataArray = Regex.Split(line, "[\\s]+", RegexOptions.IgnoreCase); // 正则匹配空格
          ExportHeight height = new ExportHeight() {
            PointName = dataArray[1],
            Height = double.Parse(dataArray[2]),
          };
          heights.Add(height);
        }
        return heights;
      }
    }
  }
}
