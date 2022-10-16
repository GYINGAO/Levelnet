using LevelnetAdjustment.model;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LevelnetAdjustment.utils {
    public class ClevelingAdjust {


        // 观测数据列表
        public List<ObservedData> ObservedDatas { get; set; }

        // 已知点列表(点号，高程)
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
            }
        }

        public List<PointData> KnownPointEable { get; set; }
        // 所有点的信息(点号，高程)
        public List<PointData> UnknownPoints { get; set; } = new List<PointData>();// 定义列表存储近似高程(点号，高程)
        public ArrayList UnknownPoints_array { get; set; } = new ArrayList();// 未知点点号数组
        //public ArrayList KnownPoints_array { get; set; } = new ArrayList();// 已知点点号数组
        //public List<PointData> AllPoints { get; set; } = new List<PointData>(); //所有点数据
        //public ArrayList AllPoint_array { get; set; } = new ArrayList();// 所有点号数组
        public List<ObservedData> ObservedDatasNoRep { get; set; } = new List<ObservedData>();// 去除重复边的观测数据
        public List<ObservedDataWF> ObservedDataWFs { get; set; } = new List<ObservedDataWF>();//测段往返测
        public List<PointData> AllPoints { get; set; } = new List<PointData>();
        public List<RawData> RawDatas { get; set; } = new List<RawData>();// 原始观测数据

        public List<Closure> Closures { get; set; } //闭合差

        public int N { get; set; } = 0;// 观测数
        public int T { get; set; } = 0;// 必要观测数
        public int R { get; set; } = 0;// 多余观测数
        public int M { get; set; } = 0;//总点数
        public int KnPnumber { get; set; } = 0; //已知点数
        public Matrix<double> P { get; set; }//权阵
        public Matrix<double> B { get; set; }//误差方程系数阵
        public Matrix<double> l { get; set; }//误差方程常数项
        public Matrix<double> NBB { get; set; }//法方程系数阵
        public Matrix<double> W { get; set; }//法方程常数项
        public Matrix<double> x { get; set; }//高程改正数
        public Matrix<double> X { get; set; }//高程真实值
        public Matrix<double> V { get; set; }//观测值改正数
        public Matrix<double> VTPV { get; set; }
        public Matrix<double> Qxx { get; set; }//未知数协因数矩阵
        public Matrix<double> x_total { get; set; }//高程累计改正量
        public Matrix<double> V_total { get; set; }//观测值累计改正量
        public Matrix<double> Qll { get; set; }//观测值协因数矩阵
        public Matrix<double> Qvv { get; set; }//改正数协因数矩阵
        public Matrix<double> RR { get; set; }
        public double PVV { get; set; } //观测值残差
        public double[] L { get; set; } //真实观测值
        public double[] Mh_P { get; set; } //高差中误差
        public double[] Mh_L { get; set; } //观测值中误差
        public double Mu { get; set; } //验后单位权中误差       
        public double TotalDistence { get; set; } = 0; //观测总距离

        public Option Options { get; set; } = new Option();  //设置与选项
        public ArrayList Threshold { get; set; } = new ArrayList(); //观测值残差阈值
        public ClevelingAdjust() {
            ObservedDatas = new List<ObservedData>();
            KnownPoints = new List<PointData>();
        }

        public void Calc_Params() {
            var tuple = Calc.RemoveDuplicates(ObservedDatas);
            ObservedDatasNoRep = tuple.Item1;
            ObservedDataWFs = tuple.Item2;
            N = ObservedDatasNoRep.Count;
            KnPnumber = KnownPointEable.Count;
            UnknownPoints_array = new ArrayList();
            int i = 0;
            ObservedDatasNoRep.ForEach(item => {
                int startIdx = UnknownPoints_array.IndexOf(item.Start);
                if (startIdx == -1) {
                    int idx = KnownPointEable.FindIndex(t => t.Number == item.Start);
                    if (idx == -1) {
                        UnknownPoints_array.Add(item.Start);
                        ObservedDatasNoRep[i].StartIndex = UnknownPoints_array.Count - 1 + KnPnumber;
                    }
                    else {
                        ObservedDatasNoRep[i].StartIndex = idx;
                    }
                }
                else {
                    ObservedDatasNoRep[i].StartIndex = startIdx + KnPnumber;
                }

                int endIdx = UnknownPoints_array.IndexOf(item.End);
                if (endIdx == -1) {
                    int idx = KnownPointEable.FindIndex(t => t.Number == item.End);
                    if (idx == -1) {
                        UnknownPoints_array.Add(item.End);
                        ObservedDatasNoRep[i].EndIndex = UnknownPoints_array.Count - 1 + KnPnumber;
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
            KnownPointEable = new List<PointData>();
            KnownPointEable.AddRange(knownPoints.FindAll(p => p.Enable));
            T = UnknownPoints_array.Count;
            R = N - T;
            M = T + KnPnumber;
            if (KnownPointEable?.Count != 0 && ObservedDatasNoRep?.Count != 0) {
                CalcApproximateHeight(true);
            }
            AllPoints = Commom.Merge(KnownPointEable, UnknownPoints);
            Console.WriteLine();
        }

        /// <summary>
        /// 计算近似高程
        /// </summary>
        public void CalcApproximateHeight(bool force) {
            if (KnownPointEable.Count == 0) {
                throw new Exception("已知点个数为0");
            }
            if (UnknownPoints != null && UnknownPoints.Count > 0 && !force) {
                return;
            }
            List<PointData> AllPoints_old = Commom.Clone(KnownPointEable);
            UnknownPoints = new List<PointData>();
            int count = 0;
            int idx = 0;
            // 如果未知点近似高程未计算完，重复循环
            while (UnknownPoints.Count < T) {
                idx++;
                ObservedDatasNoRep.ForEach(item => {
                    //在已知点里面分别查找起点和终点
                    var startIndex = AllPoints_old.FindIndex(p => p.Number.ToLower() == item.Start.ToLower());
                    var endIndex = AllPoints_old.FindIndex(p => p.Number.ToLower() == item.End.ToLower());
                    //如果起点是已知点，终点是未知点
                    if (startIndex > -1 && endIndex <= -1) {
                        PointData pd = new PointData {
                            Number = item.End,
                            Height = AllPoints_old[startIndex].Height + item.HeightDiff
                        };
                        UnknownPoints.Add(pd);
                        AllPoints_old.Add(pd);
                    }
                    //如果终点是已知点，起点是未知点
                    if (endIndex > -1 && startIndex <= -1) {
                        PointData pd = new PointData {
                            Number = item.Start,
                            Height = AllPoints_old[endIndex].Height - item.HeightDiff
                        };
                        UnknownPoints.Add(pd);
                        AllPoints_old.Add(pd);
                    }
                });
                if (idx == 1) {
                    count = UnknownPoints.Count;
                }
                if (idx == 5) {
                    idx = 0;
                    if (count == UnknownPoints.Count) {
                        throw new Exception("无法计算未知点近似高程");
                    }
                }
            }

            // 将未知点按照顺序排列
            List<PointData> UnknownPoint_new = new List<PointData>();
            for (int i = 0; i < T; i++) {
                UnknownPoint_new.Add(UnknownPoints.Find(p => p.Number == UnknownPoints_array[i].ToString()));
            }
            UnknownPoints = UnknownPoint_new;
        }

        /// <summary>
        /// 组成近似高程矩阵
        /// </summary>
        void CalcX() {
            //约束网平差，已知点当成未知点
            if (Options.AdjustMethod == 0) {
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

            //秩亏网平差，仅有未知点
            else {
                // 生成高程近似值矩阵
                double[] Xs = new double[T];
                for (int i = 0; i < T; i++) {
                    Xs[i] = UnknownPoints[i].Height;
                }
                X = Matrix<double>.Build.DenseOfColumnArrays(Xs);
            }
        }

        /// <summary>
        /// 求权阵
        /// </summary>
        void Calc_P() {
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
            if (Options.AdjustMethod == 0) {
                B = Matrix<double>.Build.Dense(N, M, 0); //生成n*m矩阵
                for (int i = 0; i < N; i++) {
                    var startIndex = GetStartIdx(i);
                    var endIndex = GetEndIdx(i);
                    if (startIndex > -1) {
                        B[i, startIndex] = -1;
                    }
                    if (endIndex > -1) {
                        B[i, endIndex] = 1;
                    }
                }
            }
            else {
                B = Matrix<double>.Build.Dense(N, T, 0); //生成n*t矩阵
                for (int i = 0; i < N; i++) {
                    var startIndex = UnknownPoints.FindIndex(p => p.Number == ObservedDatasNoRep[i].Start);
                    var endIndex = UnknownPoints.FindIndex(p => p.Number == ObservedDatasNoRep[i].End);
                    if (startIndex > -1) {
                        B[i, startIndex] = -1;
                    }
                    if (endIndex > -1) {
                        B[i, endIndex] = 1;
                    }
                }
            }
        }



        /// <summary>
        /// 求误差方程常数项l
        /// </summary>
        void Calc_l() {
            double[] ls = new double[N];
            if (Options.AdjustMethod == 0) {
                for (int i = 0; i < N; i++) {
                    var startH = X[ObservedDatasNoRep[i].StartIndex, 0];
                    var endH = X[ObservedDatasNoRep[i].EndIndex, 0];
                    ls[i] = startH + ObservedDatasNoRep[i].HeightDiff - endH;
                }// 计算每个常数项的值   
            }
            else {
                for (int i = 0; i < N; i++) {
                    var startIdx = ObservedDatasNoRep[i].StartIndex;
                    var startH = startIdx < KnPnumber ? KnownPointEable[startIdx].Height : X[startIdx - KnPnumber, 0];

                    var endIdx = ObservedDatasNoRep[i].EndIndex;
                    var endH = endIdx < KnPnumber ? KnownPointEable[endIdx].Height : X[endIdx - KnPnumber, 0];
                    ls[i] = startH + ObservedDatasNoRep[i].HeightDiff - endH;
                }// 计算每个常数项的值   
            }
            l = Matrix<double>.Build.DenseOfColumnArrays(ls);
        }

        /// <summary>
        /// 求法方程系数阵B
        /// </summary>
        void Calc_NBB() {
            NBB = B.Transpose() * P * B;
            if (Options.AdjustMethod == 0) {
                //法方程系数阵已知点对角线加上很大的常数，使得已知点改正数为0
                for (int i = 0; i < KnPnumber; i++) {
                    NBB[i, i] = 10e20;
                }
            }
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
                var startIdx = GetStartIdx(i);
                var endIdx = GetEndIdx(i);
                Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * Mu;
            }

            // 求观测值协因数矩阵
            Qll = P.Inverse();

            Qvv = Qll - B * NBB.Inverse() * B.Transpose();

            RR = Qvv * P;
        }

        /// <summary>
        /// 在所有点中查找指定起点序号
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        int GetStartIdx(int i) {
            if (Options.AdjustMethod == 0) {
                int q = KnownPointEable.FindIndex(p => p.Number == ObservedDatasNoRep[i].Start);
                if (q != -1) {
                    return q;
                }
                else {
                    return UnknownPoints.FindIndex(p => p.Number == ObservedDatasNoRep[i].Start) + KnPnumber;
                }
            }
            else {
                return UnknownPoints.FindIndex(p => p.Number == ObservedDatasNoRep[i].Start);
            }
        }

        /// <summary>
        /// 在所有点中查找指定终点序号
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        int GetEndIdx(int i) {
            if (Options.AdjustMethod == 0) {
                int q = KnownPointEable.FindIndex(p => p.Number == ObservedDatasNoRep[i].End);
                if (q != -1) {
                    return q;
                }
                else {
                    return UnknownPoints.FindIndex(p => p.Number == ObservedDatasNoRep[i].End) + KnPnumber;
                }
            }
            else {
                return UnknownPoints.FindIndex(p => p.Number == ObservedDatasNoRep[i].End);
            }
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

            if (Options.AdjustMethod == 0) {
                for (int i = 0; i < M; i++) {
                    sb.AppendLine($"{new string(' ', 5)}{i + 1,-7}{AllPoints[i].Number,-10}{AllPoints[i].Height,-15:#0.00000}{x_total[i, 0] * 1000,-15:#0.00}{X[i, 0],-16:#0.00000}{Mh_P[i],-13:#0.00}");
                }
            }
            else {
                for (int i = 0; i < T; i++) {
                    sb.AppendLine($"{new string(' ', 5)}{i + 1,-7}{UnknownPoints[i].Number,-10}{UnknownPoints[i].Height,-15:#0.00000}{x_total[i, 0] * 1000,-15:#0.00}{X[i, 0],-16:#0.00000}{Mh_P[i],-13:#0.00}");
                }
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "观测值平差值及其精度");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",-5}{"起点",-8}{"终点",-8}{"距离/km",-9}{"高差平差值/m",-9}{"中误差/mm",-8}{"多余观测分量",-8}{"改正数/mm",-8}{"阈值/mm",-6}{"警示",-8}");
            sb.AppendLine(split);
            for (int i = 0; i < N; i++) {
                sb.AppendLine($"{i + 1,-7}{ObservedDatasNoRep[i].Start,-10}{ObservedDatasNoRep[i].End,-10}{ObservedDatasNoRep[i].Distance,-11:#0.0000}{L[i],-15:#0.00000}{Mh_L[i],-12:#0.00}{RR[i, i],-12:#0.000}{V[i, 0] * 1000,-11:#0.000}{Convert.ToDouble(Threshold[i]),-9:#0.000}{new string('*', StartNumber(i)),-8}");
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
            int m_Pnumber = M;
            int m_Lnumber = ObservedDatasNoRep.Count;
            StringBuilder strClosure = new StringBuilder();
            strClosure.AppendLine(split);
            strClosure.AppendLine(space + "环闭合差计算结果");
            strClosure.AppendLine(split);
            int closure_N = 0;
            int num = ObservedDatasNoRep.Count - (m_Pnumber - 1);
            if (num < 1) {
                strClosure.AppendLine("无闭合环");
                return strClosure.ToString();
            }
            int[] neighbor = new int[m_Pnumber]; //邻接点数组
            int[] used = new int[m_Lnumber]; //观测值是否已经用于闭合差计算
            double[] diff = new double[m_Pnumber]; //高差累加值数组
            double[] S = new double[m_Pnumber]; //路线长数组

            for (int i = 0; i < m_Lnumber; i++)
                used[i] = 0;

            for (int i = 0; i < m_Lnumber; i++) {
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
                        for (int r = 0; r < m_Lnumber; r++)//将用过的观测值标定
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
                    strClosure.AppendLine($"\r\n高差闭合差：{ -W:f2}(mm){str}");
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
            Options.AdjustMethod = 0;
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
            string msg = $"{split}\r\n{space}由闭合差计算的观测值精度\r\n{split}\r\n每公里高差中数的偶然中误差：   {tmse:#0.000}\r\n多边形个数：   {Closures.Count}";

            FileHelper.WriteStrToTxt(strLine + strLoop + msg, OutpathClosure);
        }

        /// <summary>
        /// 粗差探测
        /// </summary>
        public void FindGrossError(string split, string space, string path) {
            Options.AdjustMethod = 0;
            //double U = Calc.re_norm(1 - Options.Alpha / 2);
            double U = 3.3;
            CalcX();
            PVV = 0;
            int k;
            List<ObservedData> odError = new List<ObservedData>(); //保存含有粗差的观测数据
            ArrayList V_err = new ArrayList();
            ArrayList threshold_err = new ArrayList();
            var s = Options.UnitRight == 0 ? Options.Sigma : Mu;
            for (k = 0; ; k++) {
                if (!MainForm.flag) {
                    break;
                }
                Calc_P();
                Calc_B();
                Calc_l();
                Calc_NBB();
                Calc_dX();

                // 求单位权中误差
                Calc_PVV();
                Mu = Math.Sqrt(PVV / (R - k));
                /* Qll = P.Inverse();
                 Qvv = Qll - B * NBB.Inverse() * B.Transpose();*/
                double max_v = 0;
                int max_i = 0;
                for (int i = 0; i < N; i++) {
                    if (P[i, i] < 1E-10) continue;
                    int startIdx = GetStartIdx(i);
                    int endIdx = GetEndIdx(i);
                    double qii = B[startIdx, startIdx];
                    double qjj = B[endIdx, endIdx];
                    double qij = B[startIdx, endIdx];
                    double qv = 1 / P[i, i] - (qii + qjj - 2 * qij);
                    // double qv = Qvv[i, i];
                    double mv = Math.Sqrt(qv) * s; //用验前单位权中误差
                    double vi = V[i, 0] / mv;
                    if (Math.Abs(vi) > max_v) {
                        max_v = Math.Abs(vi);
                        max_i = i;
                    }
                }
                if (max_v > U) {
                    odError.Add(ObservedDatasNoRep[max_i]);
                    V_err.Add(V[max_i, 0]);
                    threshold_err.Add(max_v);
                    ObservedDatasNoRep.RemoveAt(max_i);
                    Calc_Params();
                }
                else break;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(split);
            sb.AppendLine(space + "    粗差探测结果");
            sb.AppendLine(split);
            if (k > 0) {
                sb.AppendLine(space + $"    粗差总数：{k}");
                sb.AppendLine(split);
                sb.AppendLine($"{"序号",-5}{"起点",-8}{"终点",-8}{"距离/km",-9}{"高差/m",-10}{"改正数/mm",-8}");
                sb.AppendLine(split);
                for (int i = 0; i < odError.Count; i++) {
                    //if (P[i, i] > 1E-10) continue;
                    //sb.AppendLine($"{i + 1},{observedDatas[i].Start},{observedDatas[i].End},{V[i, 0]}");
                    sb.AppendLine($"{i + 1,-7}{odError[i].Start,-10}{odError[i].End,-10}{odError[i].Distance,-11:#0.0000}{odError[i].HeightDiff,-12:#0.00000}{Convert.ToDouble(V_err[i]) * 1000,-15:#0.0000}");
                }
                sb.AppendLine(split);
            }
            else sb.AppendLine("未发现粗差");
            Mu = Math.Sqrt(PVV / (R - k));
            sb.AppendLine(space + $"    μ = ±{Mu}");

            FileHelper.WriteStrToTxt(sb.ToString(), path);
        }

        /// <summary>
        /// 拟稳平差
        /// </summary>
        public int QuasiStable() {
            Options.AdjustMethod = 1;
            var SpNumber = UnknownPoints.FindAll(p => p.IsStable).Count;
            // 自由网平差要求每个点有先验高程值
            CalcX();
            Calc_P();
            Calc_B();
            Calc_l();
            Calc_NBB();
            for (int i = 0; i < T; i++) {
                for (int j = 0; j <= i; j++) {
                    if (UnknownPoints[i].IsStable && UnknownPoints[j].IsStable) {
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
                for (int i = 0; i < T; i++) {
                    for (int j = 0; j <= i; j++) {
                        if (UnknownPoints[i].IsStable && UnknownPoints[j].IsStable) {
                            NBB[i, j] += 1.0 / SpNumber;
                        }
                    }
                }
                Calc_dX();
                x_total += x;
                V_total += V;
            }
            //求出权逆阵Qx
            Qxx = Matrix<double>.Build.Dense(T, T, 0);
            for (int i = 0; i < T; i++) {
                for (int j = 0; j <= i; j++) {
                    Qxx[i, j] = NBB[i, j] - 1.0 / SpNumber;
                }
            }

            // 求观测值协因数矩阵
            Qll = P.Inverse();
            Qvv = Qll - B * NBB.Inverse() * B.Transpose();
            RR = Qvv * P;
            Calc_PVV();
            Mu = Math.Sqrt(PVV / (R - 1));

            // 求高程平差值中误差
            Mh_P = new double[T];
            for (int i = 0; i < T; i++) {
                Mh_P[i] = Math.Sqrt(Qxx[i, i]) * Mu;
            }

            // 求高差平差值中误差
            Mh_L = new double[N];
            for (int i = 0; i < N; i++) {
                var startIdx = GetStartIdx(i);
                var endIdx = GetEndIdx(i);
                //Mh_L[i] = Math.Sqrt(Qll[startIdx, startIdx] + Qll[endIdx, endIdx] - 2 * Qll[startIdx, endIdx]) * sigma0;
                if (startIdx == -1 && endIdx == -1) {
                    Mh_L[i] = 0;
                }
                else if (startIdx == -1 || endIdx == -1) {
                    if (startIdx == -1) {
                        Mh_L[i] = Math.Sqrt(Qxx[0, 0] + Qxx[endIdx, endIdx] - 2 * Qxx[0, endIdx]) * Mu;
                    }
                    else {
                        Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[0, 0] - 2 * Qxx[startIdx, 0]) * Mu;
                    }

                }
                else {
                    Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * Mu;
                }

            }
            CalcThreshold();
            return iteration_count;
        }

        /// <summary>
        /// 自由网平差
        /// </summary>
        public int FreeNetAdjust() {
            Options.AdjustMethod = 1;
            // 自由网平差要求每个点有先验高程值
            CalcX();
            // 生成高程近似值矩阵
            Calc_P();
            Calc_B();

            Calc_l();
            Calc_NBB();

            for (int i = 0; i < T; i++) {
                for (int j = 0; j < T; j++) {
                    NBB[i, j] += 1.0 / T;
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
                for (int i = 0; i < T; i++) {
                    for (int j = 0; j < T; j++) {
                        NBB[i, j] += 1.0 / T;
                    }
                }
                Calc_dX();
                x_total += x;
                V_total += V;
            }

            //求出权逆阵Qx
            Qxx = Matrix<double>.Build.Dense(T, T, 0);
            for (int i = 0; i < T; i++) {
                for (int j = 0; j < T; j++) {
                    Qxx[i, j] = NBB[i, j] - 1.0 / T;
                }
            }
            // 求观测值协因数矩阵
            Qll = P.Inverse();

            Qvv = Qll - B * NBB.Inverse() * B.Transpose();

            RR = Qvv * P;

            Calc_PVV();
            Mu = Math.Sqrt(PVV / (R - 1));
            // 求高程平差值中误差
            Mh_P = new double[T];
            for (int i = 0; i < T; i++) {
                Mh_P[i] = Math.Sqrt(Qxx[i, i]) * Mu;
            }

            // 求高差平差值中误差
            Mh_L = new double[N];
            for (int i = 0; i < N; i++) {
                var startIdx = GetStartIdx(i);
                var endIdx = GetEndIdx(i);
                //Mh_L[i] = Math.Sqrt(Qll[startIdx, startIdx] + Qll[endIdx, endIdx] - 2 * Qll[startIdx, endIdx]) * sigma0;
                if (startIdx == -1 && endIdx == -1) {
                    Mh_L[i] = 0;
                }
                else if (startIdx == -1 || endIdx == -1) {
                    if (startIdx == -1) {
                        Mh_L[i] = Math.Sqrt(Qxx[0, 0] + Qxx[endIdx, endIdx] - 2 * Qxx[0, endIdx]) * Mu;
                    }
                    else {
                        Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[0, 0] - 2 * Qxx[startIdx, 0]) * Mu;
                    }

                }
                else {
                    Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * Mu;
                }

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
            Threshold = new ArrayList();
            var s = Options.UnitRight == 0 ? Options.Sigma : Mu;
            for (int i = 0; i < RR.RowCount; i++) {
                if (RR[i, i] == 0) {
                    Threshold.Add(3.3 * s * Math.Sqrt(Qvv[i, i]));
                }
                else {
                    Threshold.Add(3.3 * Mh_L[i] * Math.Sqrt(RR[i, i]));
                }
            }

        }

        /// <summary>
        /// 返回警示等级
        /// </summary>
        int StartNumber(int i) {
            var s = Options.UnitRight == 0 ? Options.Sigma : Mu;
            var wi = V[i, 0] / (s * Math.Sqrt(Qvv[i, i]));
            if (wi < 3.3)
                return 0;
            if (wi > 3.3 && wi < 10)
                return 1;
            else
                return 3;
        }
    }
}
