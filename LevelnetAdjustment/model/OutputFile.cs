using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
  public class OutputFile {
    public string OutpathAdj { get; set; }//约束网平差
    public string OutpathAdjFree { get; set; }//拟稳平差
    public string OutpathClosure { get; set; }//闭合差
    public string OutpathGrossError { get; set; }//粗差
    public string COSADis { get; set; }//平差文件
    public string Handbook { get; set; }//观测手簿
    public string WFDiff { get; set; }//往返测较差
    public string CheakRawData { get; set; }//往返测较差
    public string ExportHeightPath { get; set; }//导出高差文件路径
    public string ExportObPath { get; set; }//导出观测数据文件路径

  }
}
