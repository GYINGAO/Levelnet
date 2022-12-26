using System.Collections.Generic;
using System.Dynamic;

namespace LevelnetAdjustment.model {
  public class ExportHeight {
    public string PointName { get; set; }
    public double Height { get; set; }
    public bool IsExport { get; set; } = true;
  }

  public class ExportObserve {
    public string StartPoint { get; set; }
    public string EndPoint { get; set; }
    public double HeightDiff { get; set; }
    public double Dis { get; set; }
    public bool IsExport { get; set; } = true;
  }

  public class Comparison {
    public string PointName { get; set; }
    public Dictionary<string, double> PeriodData { get; set; }
  }
}
