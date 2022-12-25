using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
  public class ProjectInfo {
    private string path;
    public string Path {
      get {
        return path;
      }
      set {
        path = value;
        if (Name != null) {
          var basePath = System.IO.Path.Combine(value, Name, "ExportFiles");
          Options.OutputFiles = new OutputFile {
            OutpathAdj = System.IO.Path.Combine(basePath, "约束网平差结果.ou3"),
            OutpathAdjFree = System.IO.Path.Combine(basePath, "拟稳平差结果.ou4"),
            OutpathClosure = System.IO.Path.Combine(basePath, "闭合差计算结果.ou1"),
            OutpathGrossError = System.IO.Path.Combine(basePath, "粗差探测结果.ou2"),
            WFDiff = System.IO.Path.Combine(basePath, "往返测高差较差.ou5"),
            CheakRawData = System.IO.Path.Combine(basePath, "观测数据检核.ou6"),
            COSADis = System.IO.Path.Combine(basePath, "平差文件.in1"),
            Handbook = System.IO.Path.Combine(basePath, "观测手簿.xls"),
            ExportHeightPath = System.IO.Path.Combine(basePath, "高程平差成果.txt"),
            ExportObPath = System.IO.Path.Combine(basePath, "高差平差成果.txt"),
          };
        }
      }
    }//项目路径
    private string name;
    public string Name {
      get {
        return name;
      }
      set {
        name = value;
        if (Path != null) {
          var basePath = System.IO.Path.Combine(Path, value, "ExportFiles");
          Options.OutputFiles = new OutputFile {
            OutpathAdj = System.IO.Path.Combine(basePath, "约束网平差结果.ou3"),
            OutpathAdjFree = System.IO.Path.Combine(basePath, "拟稳平差结果.ou4"),
            OutpathClosure = System.IO.Path.Combine(basePath, "闭合差计算结果.ou1"),
            OutpathGrossError = System.IO.Path.Combine(basePath, "粗差探测结果.ou2"),
            WFDiff = System.IO.Path.Combine(basePath, "往返测高差较差.ou5"),
            CheakRawData = System.IO.Path.Combine(basePath, "观测数据检核.ou6"),
            COSADis = System.IO.Path.Combine(basePath, "平差文件.in1"),
            Handbook = System.IO.Path.Combine(basePath, "观测手簿.xls"),
            ExportHeightPath = System.IO.Path.Combine(basePath, "高程平差成果.txt"),
            ExportObPath = System.IO.Path.Combine(basePath, "高差平差成果.txt"),
          };
        }
      }
    }//项目名
    public string Company { get; set; } = "";//单位
    public DateTime MeasurementTime { get; set; } = DateTime.Now;//测量时间
    public string MeasurementPerson { get; set; } = "";//测量人员
    public DateTime ProcessingTime { get; set; } = DateTime.Now;//处理时间
    public string ProcessingPerson { get; set; } = "";//处理人员

    public Option Options { get; set; } = new Option();//选项
    public List<PointData> KnownPoints { get; set; } = new List<PointData>();//已知点数据
    public List<PointData> UnknownPoints { get; set; } = new List<PointData>(); //拟稳点数据
    public List<RawData> RawDatas { get; set; } = new List<RawData>();//原始数据
    public List<ObservedData> ObservedDatas { get; set; } = new List<ObservedData>();//测段观测文件

  }
}
