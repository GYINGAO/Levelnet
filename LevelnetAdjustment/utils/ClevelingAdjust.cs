using LevelnetAdjustment.model;
using MathNet.Numerics.LinearAlgebra;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LevelnetAdjustment.utils {
  public class ClevelingAdjust {
    /// <summary>
    /// 观测数据列表
    /// </summary>
    public List<ObservedData> ObservedDatas { get; set; }
    /// <summary>
    /// 已知点列表(点号，高程)
    /// </summary>
    private List<PointData> knownPoints;
    public List<PointData> KnownPoints {
      get => knownPoints;
      set {
        knownPoints = value;
        // 更新已知点
        KnownPointEable = new List<PointData>();
        value.ForEach(item => {
          if (item.Enable) {
            KnownPointEable.Add(item);
          }
        });
        KnPnumber = KnownPointEable.Count;
      }
    }

    public List<PointData> KnownPointEable { get; set; }
    /// <summary>
    /// 所有点的信息(点号，高程)
    /// 定义列表存储近似高程(点号，高程)
    /// </summary>
    public List<PointData> UnknownPoints { get; set; } = new List<PointData>();
    /// <summary>
    /// 去除重复边的观测数据
    /// </summary>
    public List<ObservedData> ObservedDatasNoRep { get; set; } = new List<ObservedData>();
    /// <summary>
    /// 测段往返测
    /// </summary>
    public List<ObservedDataWF> ObservedDataWFs { get; set; } = new List<ObservedDataWF>();
    /// <summary>
    /// 所有点
    /// </summary>
    public List<PointData> AllPoints { get; set; } = new List<PointData>();
    /// <summary>
    /// 原始观测数据
    /// </summary>
    public List<RawData> RawDatas { get; set; } = new List<RawData>();
    /// <summary>
    /// 闭合差
    /// </summary>
    public List<Closure> Closures { get; set; }
    /// <summary>
    /// 观测数
    /// </summary>
    public int N { get; set; } = 0;
    /// <summary>
    /// 必要观测数
    /// </summary>
    public int T { get; set; } = 0;
    /// <summary>
    /// 多余观测数
    /// </summary>
    public int R { get; set; } = 0;
    /// <summary>
    /// 总点数
    /// </summary>
    public int M { get; set; } = 0;
    /// <summary>
    /// 已知点数
    /// </summary>
    public int KnPnumber { get; set; } = 0;
    /// <summary>
    /// 权阵
    /// </summary>
    public Matrix<double> P { get; set; }
    /// <summary>
    /// 误差方程系数阵
    /// </summary>
    public Matrix<double> B { get; set; }
    /// <summary>
    /// 误差方程常数项
    /// </summary>
    public Matrix<double> l { get; set; }
    /// <summary>
    /// 法方程系数阵
    /// </summary>
    public Matrix<double> NBB { get; set; }
    /// <summary>
    /// 法方程常数项
    /// </summary>
    public Matrix<double> W { get; set; }
    /// <summary>
    /// 高程改正数
    /// </summary>
    public Matrix<double> x { get; set; }
    /// <summary>
    /// 高程真实值
    /// </summary>
    public Matrix<double> X { get; set; }
    /// <summary>
    /// 观测值改正数
    /// </summary>
    public Matrix<double> V { get; set; }
    public Matrix<double> VTPV { get; set; }
    /// <summary>
    /// 未知数协因数矩阵
    /// </summary>
    public Matrix<double> Qxx { get; set; }
    /// <summary>
    /// 高程累计改正量
    /// </summary>
    public Matrix<double> x_total { get; set; }
    /// <summary>
    /// 观测值累计改正量
    /// </summary>
    public Matrix<double> V_total { get; set; }
    /// <summary>
    /// 观测值协因数矩阵
    /// </summary>
    public Matrix<double> Qll { get; set; }
    /// <summary>
    /// 改正数协因数矩阵
    /// </summary>
    public Matrix<double> Qvv { get; set; }
    public Matrix<double> Q { get; set; }
    public Matrix<double> RR { get; set; }
    /// <summary>
    /// 观测值残差
    /// </summary>
    public double PVV { get; set; }
    /// <summary>
    /// 真实观测值
    /// </summary>
    public double[] L { get; set; }
    /// <summary>
    /// 高差中误差
    /// </summary>
    public double[] Mh_P { get; set; }
    /// <summary>
    /// 观测值中误差
    /// </summary>
    public double[] Mh_L { get; set; }
    /// <summary>
    /// 验后单位权中误差 
    /// </summary>
    public double Mu { get; set; }
    /// <summary>
    /// 观测总距离
    /// </summary>
    public double TotalDistence { get; set; } = 0;
    /// <summary>
    /// 设置与选项
    /// </summary>
    public Option Options { get; set; } = new Option();
    /// <summary>
    /// 观测值残差阈值
    /// </summary>
    public ArrayList Threshold { get; set; } = new ArrayList();
    /// <summary>
    /// 警示等级
    /// </summary>
    public ArrayList WarningLevel { get; set; } = new ArrayList();
    public ClevelingAdjust() {
      ObservedDatas = new List<ObservedData>();
      KnownPoints = new List<PointData>();
    }
    /// <summary>
    /// 去除重复观测数据
    /// </summary>
    public void RemoveDuplicate() {
      Tuple<List<ObservedData>, List<ObservedDataWF>> tuple = Calc.RemoveDuplicates(ObservedDatas);
      ObservedDatasNoRep = tuple.Item1;
      ObservedDataWFs = tuple.Item2;
    }
    /// <summary>
    /// 计算未知点列表
    /// </summary>
    public void GetUkPointArray() {
      UnknownPoints = new List<PointData>();
      int i = 0;
      ObservedDatasNoRep.ForEach(item => {
        int startIdx = UnknownPoints.FindIndex(t => t.Number == item.Start);
        if (startIdx == -1) {
          int idx = KnownPointEable.FindIndex(t => t.Number == item.Start);
          if (idx == -1) {
            UnknownPoints.Add(new PointData() { Number = item.Start });
            ObservedDatasNoRep[i].StartIndex = UnknownPoints.Count - 1 + KnPnumber;
          }
          else {
            ObservedDatasNoRep[i].StartIndex = idx;
          }
        }
        else {
          ObservedDatasNoRep[i].StartIndex = startIdx + KnPnumber;
        }

        int endIdx = UnknownPoints.FindIndex(t => t.Number == item.End);
        if (endIdx == -1) {
          int idx = KnownPointEable.FindIndex(t => t.Number == item.End);
          if (idx == -1) {
            UnknownPoints.Add(new PointData() { Number = item.End });
            ObservedDatasNoRep[i].EndIndex = UnknownPoints.Count - 1 + KnPnumber;
          }
          else {
            ObservedDatasNoRep[i].EndIndex = idx;
          }
        }
        else {
          ObservedDatasNoRep[i].EndIndex = endIdx + KnPnumber;
        }
        TotalDistence += item.Distance;
        i++;
      });
    }

    public void Calc_Params(bool calcApproximateHeight = true) {
      KnownPointEable = new List<PointData>();
      KnownPointEable.AddRange(KnownPoints.FindAll(p => p.Enable));
      RemoveDuplicate();
      GetUkPointArray();
      CalcApproximateHeight(calcApproximateHeight);
      KnPnumber = KnownPointEable.Count;
      N = ObservedDatasNoRep.Count;
      T = UnknownPoints.Count;
      M = T + KnPnumber;
      R = N - T;
    }

    /// <summary>
    /// 计算近似高程
    /// </summary>
    public void CalcApproximateHeight(bool force = true) {
      if (KnownPointEable.Count == 0) {
        throw new Exception("已知点个数为0");
      }
      if (UnknownPoints != null && UnknownPoints.Count > 0 && !force) {
        return;
      }
      AllPoints = Commom.Clone(KnownPointEable);
      int count = 0;
      int idx = 0;
      // 如果未知点近似高程未计算完，重复循环
      while (UnknownPoints.FindIndex(t => t.Height == 0) != -1) {
        idx++;
        ObservedDatasNoRep.ForEach(item => {
          //在已知点里面分别查找起点和终点
          var startIndex = AllPoints.FindIndex(p => p.Number.ToLower() == item.Start.ToLower());
          var endIndex = AllPoints.FindIndex(p => p.Number.ToLower() == item.End.ToLower());
          //如果起点是已知点，终点是未知点
          if (startIndex != -1 && endIndex == -1) {
            var p = UnknownPoints.Find(t => t.Number == item.End);
            p.Height = AllPoints[startIndex].Height + item.HeightDiff;
            AllPoints.Add(p);
          }
          //如果终点是已知点，起点是未知点
          if (endIndex != -1 && startIndex == -1) {
            var p = UnknownPoints.Find(t => t.Number == item.Start);
            p.Height = AllPoints[endIndex].Height - item.HeightDiff;
            AllPoints.Add(p);
          }
        });
        if (idx == 1) {
          count = UnknownPoints.Count;
        }
        if (idx == 5) {
          idx = 0;
          if (count == UnknownPoints.Count) {
            throw new Exception("无法计算未知点近似高程\r                         请检查已知点或者观测文件");
          }
        }
      }

      /*// 将未知点按照顺序排列
      List<PointData> UnknownPoint_new = new List<PointData>();
      for (int i = 0; i < T; i++) {
          UnknownPoint_new.Add(UnknownPoints.Find(p => p.Number == UnknownPoints_array[i].ToString()));
      }
      UnknownPoints = UnknownPoint_new;*/
    }

    /// <summary>
    /// 组成近似高程矩阵
    /// </summary>
    void CalcX() {
      // 生成高程近似值矩阵
      double[] Xs = new double[M];
      for (int i = 0; i < KnPnumber; i++) {
        Xs[i] = KnownPointEable[i].Height;
      }
      for (int i = 0; i < T; i++) {
        Xs[i + KnPnumber] = UnknownPoints[i].Height;
      }
      X = Matrix<double>.Build.DenseOfColumnArrays(Xs);
    }

    /// <summary>
    /// 求权阵
    /// </summary>
    void Calc_P() {
      // 粗差探测会修改权
      if (P != null && P.RowCount != 0) {
        return;
      }
      // 求权阵P
      double[] powerArray = new double[N];// 定义数据存储权的值
      if (Options.PowerMethod == 1) {
        for (int i = 0; i < N; i++) {
          powerArray[i] = 1.0 / ObservedDatasNoRep[i].StationNum;
        }
      }
      else {
        for (int i = 0; i < N; i++) {
          powerArray[i] = 1.0 / ObservedDatasNoRep[i].Distance;
        }
      }
      P = Matrix<double>.Build.DenseOfDiagonalArray(powerArray);// 根据数组生成权阵
    }

    /// <summary>
    /// 求误差方程系数阵B
    /// </summary>
    void Calc_B() {
      B = Matrix<double>.Build.Dense(N, M, 0); //生成n*m矩阵
      for (int i = 0; i < N; i++) {
        var startIndex = ObservedDatasNoRep[i].StartIndex;
        var endIndex = ObservedDatasNoRep[i].EndIndex;
        B[i, startIndex] = -1;
        B[i, endIndex] = 1;
      }
    }

    /// <summary>
    /// 求误差方程常数项l
    /// </summary>
    void Calc_l() {
      double[] ls = new double[N];
      for (int i = 0; i < N; i++) {
        var startH = X[ObservedDatasNoRep[i].StartIndex, 0];
        var endH = X[ObservedDatasNoRep[i].EndIndex, 0];
        ls[i] = startH + ObservedDatasNoRep[i].HeightDiff - endH;
      }// 计算每个常数项的值   
      l = Matrix<double>.Build.DenseOfColumnArrays(ls);
    }

    /// <summary>
    /// 求法方程系数阵B
    /// </summary>
    void Calc_NBB() {
      NBB = B.Transpose() * P * B;
      W = B.Transpose() * P * l;
    }

    /// <summary>
    /// 平差值计算
    /// </summary>
    void Calc_dX() {
      // 求解x
      x = NBB.Inverse() * W;
      // 求未知点的真实值
      X += x;
      // 求观测值改正数
      V = B * x - l;
      // 求观测值平差值
      L = new double[N];
      for (int i = 0; i < N; i++) {
        L[i] = V[i, 0] + ObservedDatasNoRep[i].HeightDiff;
      }
    }

    /// <summary>
    /// 计算观测值残差
    /// </summary>
    void Calc_PVV() {
      VTPV = V.Transpose() * P * V;
      PVV = double.Parse(VTPV[0, 0].ToString()) * 1E6;
      // 求单位权中误差
      Mu = Math.Sqrt(PVV / R);
      /* PVV = 0;
       for (int i = 0; i < N; i++) {
           PVV += P[i, i] * V[i, 0] * 1000 * V[i, 0] * 1000;
       }*/
    }

    /// <summary>
    /// 精度评定
    /// </summary>
    void PrecisionEstimation() {

      // 观测值残差PVV
      Calc_PVV();
      CalcQ();
      CalcRR();
    }

    /// <summary>
    /// 是否超限
    /// </summary>
    /// <param name="mx"></param>
    /// <returns></returns>
    bool isOverrun(Matrix<double> mx) {
      for (int i = 0; i < mx.RowCount; i++) {
        if (Math.Abs(mx[i, 0]) > Options.Limit) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// 最小二乘平差
    /// </summary>
    /// <returns>迭代次数</returns>
    public int LS_Adjustment() {
      Options.AdjustMethod = 0;
      CalcX();
      Calc_P();
      Calc_B();
      Calc_l();
      Calc_NBB();
      //法方程系数阵已知点对角线加上很大的常数，使得已知点改正数为0
      for (int i = 0; i < KnPnumber; i++) {
        NBB[i, i] = 10e20;
      }
      Calc_dX();
      x_total = x;
      V_total = V;
      int iteration_count = 1;
      // 迭代计算，直到未知数改正数为0
      while (isOverrun(x)) {
        if (!MainForm.flag) {
          break;
        }
        iteration_count++;
        Calc_l();
        Calc_NBB();
        for (int i = 0; i < KnPnumber; i++) {
          NBB[i, i] = 10e20;
        }
        Calc_dX();
        x_total += x;
        V_total += V;
      }
      PrecisionEstimation();
      CalcThreshold();
      return iteration_count;
    }

    /// <summary>
    /// 输出平差结果
    /// </summary>
    /// <param name="OutpathAdj"></param>
    public void ExportAdjustResult(string filePath, string split, string space, string title) {
      // 保存文件
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(split);
      sb.AppendLine(space + $"{title}平差结果");
      sb.AppendLine(split);
      if (Options.AdjustMethod == 0) {
        sb.AppendLine(space + "已知点高程");
        sb.AppendLine(split);
        sb.AppendLine($"                  {"序号",-11}{"点名",-11}{"高程/m",-11}");
        for (int i = 0; i < KnownPointEable.Count; i++) {
          sb.AppendLine($"                  {i + 1,-13}{KnownPointEable[i].Number,-13}{KnownPointEable[i].Height,-13:#0.00000}");
        }
        sb.AppendLine(split);
      }
      sb.AppendLine(space + "高差观测数据");
      sb.AppendLine(split);
      sb.AppendLine($"{new string(' ', 10)}{"序号",-5}{"起点",-8}{"终点",-8}{"高差/m",-11}{"距离/km",-9}{"权重",-11}");
      for (int i = 0; i < N; i++) {
        sb.AppendLine($"{new string(' ', 10)}{i + 1,-7}{ObservedDatasNoRep[i].Start,-10}{ObservedDatasNoRep[i].End,-10}{ObservedDatasNoRep[i].HeightDiff.ToString("#0.00000"),-13}{ObservedDatasNoRep[i].Distance.ToString("#0.0000"),-11}{P[i, i].ToString("#0.000"),-13}");
      }
      sb.AppendLine(split);
      sb.AppendLine(space + "高程平差值及其精度");
      sb.AppendLine(split);
      sb.AppendLine($"{new string(' ', 5)}{"序号",-5}{"点名",-8}{"近似高程/m",-10}{"累计改正数/mm",-11}{"高程平差值/m",-10}{"中误差/mm",-10}");
      sb.AppendLine(split);
      for (int i = 0; i < M; i++) {
        sb.AppendLine($"{new string(' ', 5)}{i + 1,-7}{AllPoints[i].Number,-10}{AllPoints[i].Height,-15:#0.00000}{x_total[i, 0] * 1000,-15:#0.00}{X[i, 0],-16:#0.00000}{Mh_P[i],-13:#0.00}");
      }
      sb.AppendLine(split);
      sb.AppendLine(space + "观测值平差值及其精度");
      sb.AppendLine(split);
      sb.AppendLine($"{"序号",-5}{"起点",-8}{"终点",-8}{"距离/km",-9}{"高差平差值/m",-9}{"中误差/mm",-8}{"多余观测分量",-8}{"改正数/mm",-8}{"阈值/mm",-8}{"警示",-8}");
      sb.AppendLine(split);
      for (int i = 0; i < N; i++) {
        sb.AppendLine($"{i + 1,-7}{ObservedDatasNoRep[i].Start,-10}{ObservedDatasNoRep[i].End,-10}{ObservedDatasNoRep[i].Distance,-11:#0.0000}{L[i],-15:#0.00000}{Mh_L[i],-12:#0.00}{RR[i, i],-12:#0.000}{V[i, 0] * 1000,-11:#0.000}{Convert.ToDouble(Threshold[i]),-11:#0.000}{new string('*', Convert.ToInt32(WarningLevel[i]))}");
      }
      sb.AppendLine(split);
      sb.AppendLine(space + "    水准网总体信息");
      sb.AppendLine(split);
      if (Options.AdjustMethod == 0) {
        sb.AppendLine(space + "已知高程点数：".PadRight(8) + KnPnumber);
      }
      sb.AppendLine(space + "未知高程点数：".PadRight(8) + UnknownPoints.Count);
      sb.AppendLine(space + "高差测段数：".PadRight(8) + ObservedDatasNoRep.Count);
      sb.AppendLine(space + "观测总距离：".PadRight(8) + TotalDistence.ToString("#0.000") + "(km)");
      sb.AppendLine(space + "自由度：".PadRight(8) + R);
      sb.AppendLine(space + "PVV：".PadRight(8) + PVV.ToString("#0.000"));
      sb.AppendLine(space + "验后每公里高差偶然中误差：".PadRight(8) + Mu.ToString("#0.000"));

      sb.AppendLine(split);
      FileHelper.WriteStrToTxt(sb.ToString(), filePath);
    }

    /// <summary>
    /// 搜索最短路径
    /// </summary>
    /// <param name="p"></param>
    /// <param name="exclude"></param>
    /// <param name="neighbor"></param>
    /// <param name="diff"></param>
    /// <param name="S"></param>
    private void FindShortPath(int p, int exclude, int[] neighbor, double[] diff, double[] S) {
      for (int i = 0; i < M; i++) {
        neighbor[i] = -1;  // 还没有邻接点
        S[i] = 1.0e30;
      }
      S[p] = 0.0;
      diff[p] = 0.0;
      neighbor[p] = p;

      for (int i = 0; ; i++) {
        bool successful = true;
        for (int j = 0; j <= N - 1; j++) {
          if (j == exclude) continue;
          int p1 = ObservedDatasNoRep[j].StartIndex; //起点点号
          int p2 = ObservedDatasNoRep[j].EndIndex; //终点点号
          double S12 = ObservedDatasNoRep[j].Distance; // p1到p2的距离
          if (neighbor[p1] < 0 && neighbor[p2] < 0) continue;

          if (S[p2] > S[p1] + S12) {
            neighbor[p2] = p1;
            S[p2] = S[p1] + S12;
            diff[p2] = diff[p1] + ObservedDatasNoRep[j].HeightDiff;
            successful = false;
          }
          else if (S[p1] > S[p2] + S12) {
            neighbor[p1] = p2;
            S[p1] = S[p2] + S12;
            diff[p1] = diff[p2] - ObservedDatasNoRep[j].HeightDiff;
            successful = false;
          }
        }
        if (successful) break;
      }
      return;
    }

    /// <summary>
    /// 最小独立环闭合差计算	
    /// </summary>
    /// <param name="roundi"></param>
    string LoopClosure(string split, string space) {
      double roundi = Options.LevelParams.Huan;
      StringBuilder strClosure = new StringBuilder();
      strClosure.AppendLine(split);
      strClosure.AppendLine(space + "环闭合差计算结果");
      strClosure.AppendLine(split);
      int closure_N = 0;
      int num = ObservedDatasNoRep.Count - (M - 1);
      if (num < 1) {
        strClosure.AppendLine("无闭合环");
        return strClosure.ToString();
      }
      int[] neighbor = new int[M]; //邻接点数组
      int[] used = new int[N]; //观测值是否已经用于闭合差计算
      double[] diff = new double[M]; //高差累加值数组
      double[] S = new double[M]; //路线长数组

      for (int i = 0; i < N; i++)
        used[i] = 0;

      for (int i = 0; i < N; i++) {
        if (!MainForm.flag) {
          break;
        }
        int k1 = ObservedDatasNoRep[i].StartIndex; //起点点号;
        int k2 = ObservedDatasNoRep[i].EndIndex; //终点点号
        if (used[i] != 0) continue;
        if (k2 == k1)   //后面添加修改的
          return strClosure.ToString();

        FindShortPath(k2, i, neighbor, diff, S);//搜索最短路线，第i号观测值不能参加

        if (neighbor[k1] < 0) {
          // strClosure.AppendLine("观测值" + AllPoint_array[k1] + "-" + AllPoint_array[k2] + "与任何观测边不构成闭合环");
        }
        else {
          string msg = "";
          used[i] = 1;
          closure_N++;
          strClosure.AppendLine("闭合环号：" + closure_N);
          strClosure.Append("线路点号：");
          int p1 = k1;
          msg += "线路点号：";
          while (true)//输出点名
          {
            int p2 = neighbor[p1];
            strClosure.Append(AllPoints[p1].Number + "-");
            msg += AllPoints[p1].Number + "-";
            for (int r = 0; r < N; r++)//将用过的观测值标定
            {
              var startIdx = ObservedDatasNoRep[r].StartIndex;
              var endIdx = ObservedDatasNoRep[r].EndIndex;
              if (startIdx == p1 && endIdx == p2) {
                used[r] = 1;
                break;
              }
              else if (startIdx == p2 && endIdx == p1) {
                used[r] = 1;
                break;
              }
            }
            if (p2 == k2)
              break;
            //if (used[p2] == 1)
            //    break;
            else
              p1 = p2;
          }
          strClosure.Append(AllPoints[k2].Number + "-" + AllPoints[k1].Number);
          msg += AllPoints[k2].Number + "-" + AllPoints[k1].Number;
          double W = (ObservedDatasNoRep[i].HeightDiff + diff[k1]) * 1000;   //闭合差
          double SS = S[k1] + ObservedDatasNoRep[i].Distance; //环长
          double limit = (roundi * Math.Sqrt(SS));
          Closures.Add(new Closure { Error = -W, Length = SS, Limit = limit, Line = msg });
          string str = Math.Abs(W) > Math.Abs(limit) ? "   超限!!!" : "";
          strClosure.AppendLine($"\r\n高差闭合差：{-W:f2}(mm){str}");
          if (Options.LevelParams.IsCP3) {
            strClosure.AppendLine($"CPⅢ水准环闭合差限差：{Options.LevelParams.CP3Huan}(mm)");
          }
          else {
            strClosure.AppendLine("限差：" + limit.ToString("f4") + "(mm)");
          }
          strClosure.AppendLine($"总长度：{SS}(km)");
          strClosure.AppendLine();
        }
      }
      return strClosure.ToString();

    }

    /// <summary>
    /// 附和水准最短路径
    /// </summary>
    /// <param name="roundi"></param>
    /// <returns></returns>
    private string LineClosure(string split, string space) {
      double roundi = Options.LevelParams.FuHe;
      StringBuilder strClosure = new StringBuilder();
      int line_N = 0;
      strClosure.AppendLine(split);
      strClosure.AppendLine(space + "附合路线闭合差计算结果");
      strClosure.AppendLine(split);
      if (KnPnumber == 1)
        return strClosure.AppendLine("已知点数小于2").ToString(); // 已知点数小于2
      if (KnPnumber <= 0)
        return strClosure.AppendLine("未导入已知点").ToString(); // 已知点数等于0
      int[] neighbor = new int[M];       //邻接点数组
      double[] diff = new double[M]; //高差累加值数组
      double[] S = new double[M];    //路线长累加值数组

      for (int ii = 0; ii < KnPnumber; ii++) {
        if (!MainForm.flag) {
          break;
        }
        FindShortPath(ii, -1, neighbor, diff, S); //搜索最短路线，用所有观测值

        for (int jj = ii + 1; jj < KnPnumber; jj++) {
          if (neighbor[jj] < 0) {
            strClosure.Append(KnownPointEable[ii].Number + "-" + KnownPointEable[jj].Number + "之间找到不到最短路线");
            continue;
          }
          // 输出附合路线上的点号
          line_N++;
          string msg = "";
          strClosure.AppendLine("线路号：" + line_N);
          strClosure.Append("线路点号：");
          msg += "线路点号：";
          int k = jj;
          while (true) {
            strClosure.Append(AllPoints[k].Number + "-");
            msg += (AllPoints[k].Number + "-");
            k = neighbor[k];
            if (k == ii) break;
          }
          strClosure.Append(AllPoints[ii].Number);
          msg += AllPoints[ii].Number;

          //闭合差计算，限差计算与输出
          double W = (KnownPointEable[ii].Height + diff[jj] - KnownPointEable[jj].Height) * 1000; // 闭合差
          double limit = roundi * Math.Sqrt(S[jj]);  // 限差
          Closures.Add(new Closure { Error = -W, Length = S[jj], Limit = limit, Line = msg });
          string str = Math.Abs(W) > Math.Abs(limit) ? "   超限!!!" : "";
          strClosure.AppendLine($"\r\n高差闭合差：{-W:#0.00}(mm){str}");
          strClosure.AppendLine("限差：" + limit.ToString("f4") + "(mm)");
          strClosure.AppendLine($"总长度：{S[jj]}(km)");
          strClosure.AppendLine();

        }
      }
      return strClosure.ToString();
    }

    /// <summary>
    /// 计算闭合差
    /// </summary>
    public void CalcClosureError(string OutpathClosure, string split, string space) {
      #region 根据观测数据生成邻接表
      /*            var pointNum = M;
                  const int inf = 100000000;
                  double[,] cost = new double[pointNum, pointNum];
                  // 对角线为0，其余为inf
                  for (int i = 0; i < pointNum; i++) {
                      for (int j = 0; j < pointNum; j++) {
                          if (i == j) {
                              cost[i, j] = 0;
                          }
                          else {
                              cost[i, j] = inf;
                          }
                      }
                  }
                  // 记录观测数据
                  for (int i = 0; i < N; i++) {
                      var rowIdx = GetStartIdx(i);
                      var columnIdx = GetEndIdx(i);
                      cost[rowIdx, columnIdx] = ObservedDatasNoRep[i].Distance;
                      cost[columnIdx, rowIdx] = ObservedDatasNoRep[i].Distance;
                  }*/
      #endregion

      Closures = new List<Closure>();
      string strLoop = LoopClosure(split, space);
      string strLine = LineClosure(split, space);

      double tmse = CalcTMSE();
      string msg = $"{split}\r\n{space}由闭合差计算的观测值精度\r\n{split}\r\n{new string(' ', 23)}每公里高差中数的偶然中误差：   {tmse:#0.000}\r\n{new string(' ', 23)}多边形个数：   {Closures.Count}";

      FileHelper.WriteStrToTxt(strLine + strLoop + msg, OutpathClosure);
    }



    /// <summary>
    /// 粗差探测（数据探测法）
    /// </summary>
    public void DataSnoopingMethod(string split, string space, string path) {
      Options.AdjustMethod = 0;
      Options.GrossErrors.Clear();
      CalcX();
      Calc_P();
      Calc_B();
      Calc_l();
      PVV = 0;
      int k;
      for (k = 0; ; k++) {
        if (!MainForm.flag) {
          break;
        }
        Calc_NBB();
        //法方程系数阵已知点对角线加上很大的常数，使得已知点改正数为0
        for (int i = 0; i < KnPnumber; i++) {
          NBB[i, i] = 10e20;
        }
        Calc_dX();

        // 求单位权中误差
        Calc_PVV();
        var s = Options.UnitRight == 0 ? Options.Sigma : Mu;
        double max_v = 0;
        int max_i = 0;

        for (int i = 0; i < N; i++) {
          if (P[i, i] < 1E-10) continue;
          int startIdx = ObservedDatasNoRep[i].StartIndex;
          int endIdx = ObservedDatasNoRep[i].EndIndex;
          double qii = B[startIdx, startIdx];
          double qjj = B[endIdx, endIdx];
          double qij = B[startIdx, endIdx];
          double qv = 1 / P[i, i] - (qii + qjj - 2 * qij);
          // double qv = Qvv[i, i];
          double mv = Math.Sqrt(qv) * s; //用验前单位权中误差
          double vi = Math.Abs(V[i, 0] / mv);
          if (vi > max_v) {
            max_v = vi;
            max_i = i;
          }
        }

        /*         // 计算观测值残差
                 Qvv = P.Inverse() - B * NBB.Inverse() * B.Transpose();
                 List<double> vi = new List<double>();
                 for (int i = 0; i < Qvv.RowCount; i++) {
                     // 计算每一项的标准化残差
                     vi.Add(Math.Abs(V[i, 0] / (Math.Sqrt(Qvv[i, i]) * s)));
                 }
                 // 找到标准化残差最大的值和索引
                 var v = vi.Select((m, index) => new { index, m }).OrderByDescending(n => n.m).Take(1);
                 foreach (var item in v) {
                     max_v = item.m;
                     max_i = item.index;
                 }*/
        if (max_v > Options.AlphaLimit) {
          Options.GrossErrors.Add(new GrossError() {
            ObservedDatas = ObservedDatasNoRep[max_i],
            Index = max_i,
            Correction = V[max_i, 0],
            StandardizedResidual = max_v
          });

          P[max_i, max_i] = 0;//有粗差的观测值权设为0
        }
        else break;
      }
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(split);
      sb.AppendLine(space + "    粗差探测结果");
      sb.AppendLine(split);
      if (Options.GrossErrors?.Count > 0) {
        sb.AppendLine(space + $"    粗差总数：{Options.GrossErrors?.Count}");
        sb.AppendLine(split);
        sb.AppendLine($"{"序号",-5}{"起点",-8}{"终点",-8}{"距离/km",-9}{"高差/m",-10}{"改正数/mm",-8}{"标准化残差",-10}{"阈值",-10}");
        sb.AppendLine(split);
        for (int i = Options.GrossErrors.Count - 1; i >= 0; i--) {
          GrossError error = Options.GrossErrors[i];
          ObservedDatasNoRep.RemoveAt(error.Index);
          R--;
          N--;
          //if (P[i, i] > 1E-10) continue;
          //sb.AppendLine($"{i + 1},{observedDatas[i].Start},{observedDatas[i].End},{V[i, 0]}");
          sb.AppendLine($"{i + 1,-7}{error.ObservedDatas.Start,-10}{error.ObservedDatas.End,-10}{error.ObservedDatas.Distance,-11:#0.0000}{error.ObservedDatas.HeightDiff,-12:#0.00000}{Convert.ToDouble(error.Correction) * 1000,-14:#0.0000}{error.StandardizedResidual,-10:0.000}{Options.AlphaLimit,-12:#0.000}");
        }
        sb.AppendLine(split);
      }
      else sb.AppendLine("未发现粗差");
      sb.AppendLine(space + $"    μ = ±{Mu.ToString("f4")}");
      FileHelper.WriteStrToTxt(sb.ToString(), path);
    }

    /// <summary>
    /// 粗差探测（选权迭代法IGG）
    /// </summary>
    /// <param name="split"></param>
    /// <param name="space"></param>
    /// <param name="path"></param>
    public void IGGMethod(string split, string space, string path) {
      double limit = 0.01;
      Options.AdjustMethod = 0;
      Options.GrossErrors.Clear();
      CalcX();
      Calc_P();
      Calc_B();
      Calc_l();
      PVV = 0;
      Calc_NBB();
      //法方程系数阵已知点对角线加上很大的常数，使得已知点改正数为0
      for (int i = 0; i < KnPnumber; i++) {
        NBB[i, i] = 10e20;
      }
      Calc_dX();
      // 求单位权中误差
      Calc_PVV();
      Matrix<double> lastX = X;
      Matrix<double> minusX = Matrix<double>.Abs(X - lastX);
      double maxMinus = minusX.ToColumnMajorArray().Max();
      double lastMaxMinus = maxMinus;
      for (int j = 0; ; j++) {
        IGGFun();
        minusX = Matrix<double>.Abs(X - lastX);
        maxMinus = minusX.ToColumnMajorArray().Max();
        if (maxMinus <= limit) {
          break;
        }
        //minusX = (X - lastX).PointwiseAbs();

        if (lastMaxMinus == maxMinus) {
          break;
        }
        lastX = X;
        lastMaxMinus = maxMinus;
      }
    }

    /// <summary>
    /// IGG权函数
    /// </summary>
    public void IGGFun() {
      double k = 0.001;
      for (int i = 0; i < V.RowCount; i++) {
        double absoluteValueOfV = Math.Abs(V[i, 0]);
        if (absoluteValueOfV < 1.5 * Mu) {
          continue;
        }
        else if (absoluteValueOfV >= 2.5 * Mu) {
          P[i, i] = 0;
        }
        else {
          P[i, i] = 1 / (absoluteValueOfV + k) * P[i, i];
        }
      }
      Calc_NBB();
      //法方程系数阵已知点对角线加上很大的常数，使得已知点改正数为0
      for (int i = 0; i < KnPnumber; i++) {
        NBB[i, i] = 10e20;
      }
      Calc_dX();
      // 求单位权中误差
      Calc_PVV();
    }

    /// <summary>
    /// 拟稳平差
    /// </summary>
    public int QuasiStable() {
      Options.AdjustMethod = 1;
      Calc_Params();
      var SpNumber = AllPoints.FindAll(p => p.IsStable).Count;
      CalcX();
      Calc_P();
      Calc_B();
      Calc_l();
      Calc_NBB();
      for (int i = 0; i < M; i++) {
        for (int j = 0; j < M; j++) {
          if (AllPoints[i].IsStable && AllPoints[j].IsStable) {
            NBB[i, j] += 1.0 / SpNumber;
          }
        }
      }
      Calc_dX();
      x_total = x;
      V_total = V;
      int iteration_count = 1;
      while (isOverrun(x)) {
        if (!MainForm.flag) {
          break;
        }
        iteration_count++;
        Calc_l();
        Calc_NBB();
        for (int i = 0; i < M; i++) {
          for (int j = 0; j < M; j++) {
            if (AllPoints[i].IsStable && AllPoints[j].IsStable) {
              NBB[i, j] += 1.0 / SpNumber;
            }
          }
        }
        Calc_dX();
        x_total += x;
        V_total += V;
      }
      //求出权逆阵Qx
      Qxx = Matrix<double>.Build.Dense(M, M, 0);
      for (int i = 0; i < M; i++) {
        for (int j = 0; j < M; j++) {
          Qxx[i, j] = NBB[i, j] - 1.0 / SpNumber;
        }
      }
      // 求观测值协因数矩阵
      Q = P.Inverse();
      Qll = B * NBB.Inverse() * B.Transpose();
      Qvv = Q - Qll;
      CalcRR();
      Calc_PVV();
      Mu = Math.Sqrt(PVV / (R - 1));

      // 求高程平差值中误差
      Mh_P = new double[M];
      for (int i = 0; i < M; i++) {
        Mh_P[i] = Math.Sqrt(Qxx[i, i]) * Mu;
      }

      // 求高差平差值中误差
      Mh_L = new double[N];
      for (int i = 0; i < N; i++) {
        var startIdx = ObservedDatasNoRep[i].StartIndex;
        var endIdx = ObservedDatasNoRep[i].EndIndex;
        //Mh_L[i] = Math.Sqrt(Qll[startIdx, startIdx] + Qll[endIdx, endIdx] - 2 * Qll[startIdx, endIdx]) * sigma0;
        Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * Mu;


      }
      CalcThreshold();
      return iteration_count;
    }

    /// <summary>
    /// 自由网平差
    /// </summary>
    public int FreeNetAdjust() {
      Options.AdjustMethod = 1;
      Calc_Params();
      // 自由网平差要求每个点有先验高程值
      CalcX();
      // 生成高程近似值矩阵
      Calc_P();
      Calc_B();

      Calc_l();
      Calc_NBB();

      for (int i = 0; i < M; i++) {
        for (int j = 0; j < M; j++) {
          NBB[i, j] += 1.0 / M;
        }
      }
      Calc_dX();
      x_total = x;
      V_total = V;

      int iteration_count = 1;
      // 迭代计算，直到未知数改正数为0
      while (isOverrun(x)) {
        if (!MainForm.flag) {
          break;
        }
        iteration_count++;
        Calc_l();
        Calc_NBB();
        for (int i = 0; i < M; i++) {
          for (int j = 0; j < M; j++) {
            NBB[i, j] += 1.0 / M;
          }
        }
        Calc_dX();
        x_total += x;
        V_total += V;
      }

      //求出权逆阵Qx
      Qxx = Matrix<double>.Build.Dense(M, M, 0);
      for (int i = 0; i < M; i++) {
        for (int j = 0; j < M; j++) {
          Qxx[i, j] = NBB[i, j] - 1.0 / M;
        }
      }
      // 求观测值协因数矩阵
      Q = P.Inverse();

      Qll = B * NBB.Inverse() * B.Transpose();

      Qvv = Q - Qll;

      CalcRR();

      Calc_PVV();
      Mu = Math.Sqrt(PVV / (R - 1));
      // 求高程平差值中误差
      Mh_P = new double[M];
      for (int i = 0; i < M; i++) {
        Mh_P[i] = Math.Sqrt(Qxx[i, i]) * Mu;
      }

      // 求高差平差值中误差
      Mh_L = new double[N];
      for (int i = 0; i < N; i++) {
        var startIdx = ObservedDatasNoRep[i].StartIndex;
        var endIdx = ObservedDatasNoRep[i].EndIndex;
        //Mh_L[i] = Math.Sqrt(Qll[startIdx, startIdx] + Qll[endIdx, endIdx] - 2 * Qll[startIdx, endIdx]) * sigma0;
        Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * Mu;
      }
      CalcThreshold();
      return iteration_count;
    }

    /// <summary>
    /// 计算全中误差
    /// </summary>
    /// <returns></returns>
    public double CalcTMSE() {
      double w = 0;
      Closures.ForEach(t => {
        w += t.Error * t.Error / t.Length;
      });
      return Math.Sqrt(w / Closures.Count);
    }

    /// <summary>
    /// 计算观测值残差的阈值
    /// </summary>
    /// <returns></returns>
    public void CalcThreshold() {
      Threshold.Clear();
      WarningLevel.Clear();
      //double s = Options.UnitRight == 0 ? Options.Sigma : Mu;
      double s = Mu;
      for (int i = 0; i < RR.RowCount; i++) {
        double t, max, c;
        c = Mh_L[i] * Math.Sqrt(RR[i, i]);
        t = Options.AlphaLimit * c;
        max = 9.9 * c;
        /*if (RR[i, i] == 0) {
          c = s * Math.Sqrt(Qvv[i, i]);
          t = Options.AlphaLimit * c;
          max = 9.9 * c;
        }
        else {
          c = Mh_L[i] * Math.Sqrt(RR[i, i]);
          t = Options.AlphaLimit * c;
          max = 9.9 * c;
        }*/
        Threshold.Add(t);
        WarningLevel.Add(StartNumber(V[i, 0], t, max));
      }
    }

    /// <summary>
    /// 返回警示等级
    /// </summary>
    /// <param name="v">改正数</param>
    /// <param name="a">一星警示值</param>
    /// <param name="b">三星警示值</param>
    /// <returns></returns>
    int StartNumber(double v, double a, double b) {
      double absV = Math.Abs(v) * 1000;
      if (absV < a)
        return 0;
      else if (absV > a && absV < b)
        return 1;
      else
        return 3;
    }

    /// <summary>
    /// 计算平差因子
    /// </summary>
    /// <returns></returns>
    public void CalcRR() {
      /*Matrix<double> J = B * NBB.Inverse() * B.Transpose() * P;
      return Matrix<double>.Build.DenseIdentity(J.RowCount) - J;*/

      /*Matrix<double> J = Qvv * P;
      return Matrix<double>.Build.DenseIdentity(J.RowCount) - J;*/

      RR = Qvv * P;
    }

    public void CalcQ() {
      // 求未知数的协因数阵
      Qxx = NBB.Inverse();
      // 求高程平差值中误差
      Mh_P = new double[M];
      for (int i = 0; i < M; i++) {
        Mh_P[i] = Math.Sqrt(Qxx[i, i]) * Mu;
      }
      // 求高差平差值中误差
      Mh_L = new double[N];
      for (int i = 0; i < N; i++) {
        var startIdx = ObservedDatasNoRep[i].StartIndex;
        var endIdx = ObservedDatasNoRep[i].EndIndex;
        Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * Mu;
      }
      // 求观测值协因数矩阵
      Q = P.Inverse();

      Qll = B * NBB.Inverse() * B.Transpose();

      Qvv = Q - Qll;
    }
  }
}
