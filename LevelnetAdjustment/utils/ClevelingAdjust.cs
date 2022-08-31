using LevelnetAdjustment.model;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LevelnetAdjustment.utils {
    public class ClevelingAdjust {
        // 观测数据列表
        private List<ObservedData> observedDatas;
        public List<ObservedData> ObservedDatas {
            get => observedDatas;
            set {
                observedDatas = value;
                N = value.Count;
                UnknownPoints_array = new ArrayList();
                value.ForEach(item => {
                    if (!UnknownPoints_array.Contains(item.Start) && !KnownPoints_array.Contains(item.Start)) {
                        UnknownPoints_array.Add(item.Start);
                    }
                    if (!UnknownPoints_array.Contains(item.End) && !KnownPoints_array.Contains(item.End)) {
                        UnknownPoints_array.Add(item.End);
                    }
                    TotalDistence += item.Distance;
                });
                T = UnknownPoints_array.Count;
                R = N - T;
                AllPoint_array = Commom.Clone(KnownPoints_array);
                for (int i = 0; i < UnknownPoints_array.Count; i++) {
                    AllPoint_array.Add(UnknownPoints_array[i]);
                }
                M = AllPoint_array.Count;
            }
        }
        // 已知点列表(点号，高程)
        private List<PointData> knownPoints;
        public List<PointData> KnownPoints {
            get => knownPoints;
            set {
                KnownPoints_array = new ArrayList();
                value.ForEach(item => {
                    KnownPoints_array.Add(item.Number);
                });
                knownPoints = value;
                KnPnumber = value.Count;
            }
        }
        // 所有点的信息(点号，高程)
        public List<PointData> UnknownPoints { get; set; } = new List<PointData>();// 定义列表存储近似高程(点号，高程)
        public ArrayList UnknownPoints_array { get; set; } = new ArrayList();// 未知点点号数组
        public ArrayList KnownPoints_array { get; set; } = new ArrayList();// 已知点点号数组
        public List<PointData> AllPoints { get; set; } = new List<PointData>(); //所有点数据
        public ArrayList AllPoint_array { get; set; } = new ArrayList();// 所有点号数组
        public List<ObservedData> ObservedDatasNoRep { get; set; } = new List<ObservedData>();// 去除重复边的观测数据
        public List<RawData> RawDatas { get; set; } = new List<RawData>();// 原始观测数据

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
        public double PVV { get; set; } //观测值残差
        public double[] L { get; set; } //真实观测值
        public double[] Mh_P { get; set; } //高差中误差
        public double[] Mh_L { get; set; } //观测值中误差
        public double Mu { get; set; } //验后单位权中误差       
        public double TotalDistence { get; set; } = 0; //观测总距离
        public int StablePnumber { get; set; } = 0; //拟稳点数
        public Option Options { get; set; } = new Option();  //设置与选项
        public ArrayList Threshold { get; set; } //阈值
        //拟稳点号数组
        ArrayList stablePoints;
        public ArrayList StablePoints {
            get {
                return stablePoints;
            }
            set {
                stablePoints = value;
                StablePnumber = value.Count;
            }
        }

        const int pad = -13;
        const int titlePad = -11;


        public ClevelingAdjust() {

        }

        void Calc_Params() {
            N = observedDatas.Count;
            T = UnknownPoints.Count;
            R = N - T;
            M = AllPoints.Count;
            KnPnumber = knownPoints.Count;

            UnknownPoints_array = new ArrayList();
            observedDatas.ForEach(item => {
                if (!UnknownPoints_array.Contains(item.Start) && !KnownPoints_array.Contains(item.Start)) {
                    UnknownPoints_array.Add(item.Start);
                }
                if (!UnknownPoints_array.Contains(item.End) && !KnownPoints_array.Contains(item.End)) {
                    UnknownPoints_array.Add(item.End);
                }
                TotalDistence += item.Distance;
            });

            AllPoint_array = Commom.Clone(KnownPoints_array);
            for (int i = 0; i < UnknownPoints_array.Count; i++) {
                AllPoint_array.Add(UnknownPoints_array[i]);
            }

            // 重新计算不含重复测段的额观测数据

        }

        /// <summary>
        /// 计算近似高程
        /// </summary>
        void CalcApproximateHeight() {
            List<PointData> AllPoints_old = Commom.Clone(KnownPoints);
            UnknownPoints = new List<PointData>();
            int count = 0;
            // 如果未知点近似高程未计算完，重复循环
            while (UnknownPoints.Count < T) {
                ObservedDatas.ForEach(item => {
                    //在已知点里面分别查找起点和终点
                    var startIndex = AllPoints_old.FindIndex(p => p.Number == item.Start);
                    var endIndex = AllPoints_old.FindIndex(p => p.Number == item.End);
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
                if (count == UnknownPoints.Count) {
                    throw new Exception("无法计算未知点近似高程");
                }
                count = UnknownPoints.Count;
            }

            // 将未知点按照顺序排列
            List<PointData> UnknownPoint_new = new List<PointData>();
            for (int i = 0; i < T; i++) {
                UnknownPoint_new.Add(UnknownPoints.Find(p => p.Number == UnknownPoints_array[i].ToString()));
            }
            UnknownPoints = UnknownPoint_new;

            // 所有点按照顺序排列
            List<PointData> AllKnownPoint_new = new List<PointData>();
            for (int i = 0; i < M; i++) {
                AllKnownPoint_new.Add(AllPoints_old.Find(p => p.Number == AllPoint_array[i].ToString()));
            }
            AllPoints = AllKnownPoint_new;

            //约束网平差，已知点当成未知点
            if (Options.AdjustMethod == 0) {
                // 生成高程近似值矩阵
                double[] Xs = new double[M];
                for (int i = 0; i < KnPnumber; i++) {
                    Xs[i] = KnownPoints[i].Height;
                }
                for (int i = 0; i < T; i++) {
                    Xs[i + KnownPoints.Count] = UnknownPoints[i].Height;
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
            if (Options.AdjustMethod == 1) {
                for (int i = 0; i < N; i++) {
                    powerArray[i] = 1.0 / ObservedDatas[i].StationNum;
                }

            }
            else {
                for (int i = 0; i < N; i++) {
                    powerArray[i] = 1.0 / ObservedDatas[i].Distance;
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
                B = Matrix<double>.Build.Dense(N, T, 0); //生成n*m矩阵
                for (int i = 0; i < N; i++) {
                    var startIndex = UnknownPoints_array.IndexOf(ObservedDatas[i].Start);
                    var endIndex = UnknownPoints_array.IndexOf(ObservedDatas[i].End);
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
            for (int i = 0; i < N; i++) {
                var startH = X[AllPoints.FindIndex(p => p.Number == ObservedDatas[i].Start), 0];
                var endH = X[AllPoints.FindIndex(p => p.Number == ObservedDatas[i].End), 0];
                ls[i] = startH + ObservedDatas[i].HeightDiff - endH;
            }// 计算每个常数项的值      
            l = Matrix<double>.Build.DenseOfColumnArrays(ls);
        }

        /// <summary>
        /// 求法方程系数阵B
        /// </summary>
        void Calc_NBB() {

            NBB = B.Transpose() * P * B;
            if (Options.AdjustMethod == 0) {
                //法方程系数阵已知点对角线加上很大的常数，使得已知点改正数为0
                for (int i = 0; i < KnownPoints.Count; i++) {
                    NBB[i, i] = 10e20;
                }
            }
            else {
                for (int i = 0; i < T; i++) {
                    NBB[i, i] += 1.0 / T;
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
                L[i] = V[i, 0] + ObservedDatas[i].HeightDiff;
            }
        }

        /// <summary>
        /// 计算观测值残差
        /// </summary>
        void Calc_PVV() {
            for (int i = 0; i < N; i++) {
                PVV += P[i, i] * V[i, 0] * 1000 * V[i, 0] * 1000;
            }
        }

        /// <summary>
        /// 精度评定
        /// </summary>
        void PrecisionEstimation() {

            // 观测值残差PVV
            PVV = 0;

            Calc_PVV();
            // 求单位权中误差
            VTPV = V.Transpose() * P * V;
            //sigma0 = Math.Sqrt(double.Parse(VTPV[0, 0].ToString()) / R) * 1000;
            Mu = Math.Sqrt(PVV / R);
            //var sigma1 = Math.Sqrt((l.Transpose() * P * l - W.Transpose() * x)[0, 0] / R);
            //Console.WriteLine(sigma0.ToString() + "," + sigma1.ToString());



            // 求未知数的协因数阵
            Qxx = NBB.Inverse();
            //Console.WriteLine(Qxx.ToString());

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
            Qll = B * NBB.Inverse() * B.Transpose();

            var Qvv = Qll - B * NBB.Inverse() * B.Transpose();
        }

        /// <summary>
        /// 在所有点中查找指定起点序号
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        int GetStartIdx(int i) {
            return Options.AdjustMethod == 0 ? AllPoints.FindIndex(p => p.Number == ObservedDatas[i].Start) : UnknownPoints.FindIndex(p => p.Number == ObservedDatas[i].Start);
        }

        /// <summary>
        /// 在所有点中查找指定终点序号
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        int GetEndIdx(int i) {
            return Options.AdjustMethod == 0 ? AllPoints.FindIndex(p => p.Number == ObservedDatas[i].End) : UnknownPoints.FindIndex(p => p.Number == ObservedDatas[i].End);
        }

        /// <summary>
        /// 是否满足限差，满足返回true
        /// </summary>
        /// <param name="mx"></param>
        /// <returns></returns>
        bool isLimit(Matrix<double> mx) {
            for (int i = 0; i < mx.RowCount; i++) {
                if (Math.Abs(mx[i, 0] * 1000) > Options.Limit) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 最小二乘平差
        /// </summary>
        /// <returns>迭代次数</returns>
        public int LS_Adjustment() {
            CalcApproximateHeight();
            Calc_P();
            Calc_B();
            Calc_l();
            Calc_NBB();
            Calc_dX();
            x_total = x;
            V_total = V;
            double U = Calc.re_norm(1 - Options.Alpha / 2);
            int iteration_count = 1;
            // 迭代计算，直到未知数改正数为0
            while (!isLimit(x)) {
                iteration_count++;
                Calc_l();
                Calc_NBB();
                Calc_dX();
                x_total += x;
                V_total += V;
            }
            PrecisionEstimation();
            return iteration_count;
        }

        /// <summary>
        /// 输出约束网平差结果
        /// </summary>
        /// <param name="OutpathAdj"></param>
        public void ExportConstraintNetworkResult(string OutpathAdj, string split, string space) {
            // 保存文件

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(split);
            sb.AppendLine(space + "约束网平差结果");
            sb.AppendLine(split);
            sb.AppendLine(space + "已知点高程");
            sb.AppendLine(split);
            sb.AppendLine($"                  {"序号",titlePad}{"点名",titlePad}{"高程(m)",titlePad}");
            for (int i = 0; i < KnownPoints.Count; i++) {
                sb.AppendLine($"                  {i + 1,pad}{KnownPoints[i].Number,pad}{KnownPoints[i].Height,pad:#0.00000}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "高差观测数据");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"起点",titlePad}{"终点",titlePad}{"高差(m)",titlePad}{"距离(km)",titlePad}{"权重",titlePad}");
            for (int i = 0; i < N; i++) {
                sb.AppendLine($"{i + 1,pad}{ObservedDatas[i].Start,pad}{ObservedDatas[i].End,pad}{ObservedDatas[i].HeightDiff.ToString("#0.00000"),pad}{ObservedDatas[i].Distance.ToString("#0.0000"),pad}{P[i, i].ToString("#0.000"),pad}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "高程平差值及其精度");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"点名",titlePad}{"近似高程(m)",titlePad}{"累计改正数(mm)",titlePad}{"高程平差值(m)",titlePad}{"中误差(mm)",titlePad}");
            sb.AppendLine(split);
            for (int i = 0; i < M; i++) {
                sb.AppendLine($"{i + 3,pad}{AllPoints[i].Number,pad}{AllPoints[i].Height,pad - 4:#0.00000}{x_total[i, 0] * 1000,pad:#0.00}{X[i, 0],pad - 2:#0.00000}{Mh_P[i],pad:#0.00}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "观测值平差值及其精度");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"起点",titlePad}{"终点",titlePad}{"高差平差值(m)",titlePad}{"距离(km)",titlePad}{"中误差(mm)",titlePad}");
            sb.AppendLine(split);
            for (int i = 0; i < N; i++) {
                sb.AppendLine($"{i + 1,pad}{ObservedDatas[i].Start,pad}{ObservedDatas[i].End,pad}{L[i],pad - 5:#0.00000}{ObservedDatas[i].Distance,pad:#0.0000}{Mh_L[i],pad:#0.00}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "    水准网总体信息");
            sb.AppendLine(split);
            sb.AppendLine(space + "已知高程点数：".PadRight(8) + KnownPoints.Count);
            sb.AppendLine(space + "未知高程点数：".PadRight(8) + UnknownPoints.Count);
            sb.AppendLine(space + "高差测段数：".PadRight(8) + ObservedDatasNoRep.Count);
            sb.AppendLine(space + "观测总距离：".PadRight(8) + TotalDistence.ToString("#0.000") + "(km)");
            sb.AppendLine(space + "PVV：".PadRight(8) + PVV.ToString("#0.000"));
            sb.AppendLine(space + "验后单位权中误差：".PadRight(8) + Mu.ToString("#0.000"));

            sb.AppendLine(split);
            FileHelper.WriteStrToTxt(sb.ToString(), OutpathAdj);
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
            for (int i = 0; i < AllPoint_array.Count; i++) {
                neighbor[i] = -1;  // 还没有邻接点
                S[i] = 1.0e30;
            }
            S[p] = 0.0;
            diff[p] = 0.0;
            neighbor[p] = p;

            for (int i = 0; ; i++) {
                bool successful = true;
                for (int j = 0; j <= ObservedDatasNoRep.Count - 1; j++) {
                    if (j == exclude) continue;
                    int p1 = GetStartIdx(j); //起点点号
                    int p2 = GetEndIdx(j); //终点点号
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
        string LoopClosure(double roundi, string split, string space) {
            int m_Pnumber = AllPoint_array.Count;
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
                int k1 = GetStartIdx(i); //起点点号;
                int k2 = GetEndIdx(i); //终点点号
                if (used[i] != 0) continue;
                if (k2 == k1)   //后面添加修改的
                    return strClosure.ToString();

                FindShortPath(k2, i, neighbor, diff, S);//搜索最短路线，第i号观测值不能参加

                if (neighbor[k1] < 0) {
                    // strClosure.AppendLine("观测值" + AllPoint_array[k1] + "-" + AllPoint_array[k2] + "与任何观测边不构成闭合环");
                }
                else {
                    used[i] = 1;
                    closure_N++;
                    strClosure.AppendLine("闭合环号：" + closure_N);
                    strClosure.Append("线路点号：");
                    int p1 = k1;
                    while (true)//输出点名
                    {
                        int p2 = neighbor[p1];
                        strClosure.Append(AllPoints[p1].Number + "-");

                        for (int r = 0; r < m_Lnumber; r++)//将用过的观测值标定
                        {
                            if (AllPoint_array.IndexOf(ObservedDatasNoRep[r].Start) == p1 && AllPoint_array.IndexOf(ObservedDatasNoRep[r].End) == p2) {
                                used[r] = 1;
                                break;
                            }
                            else if (AllPoint_array.IndexOf(ObservedDatasNoRep[r].Start) == p2 && AllPoint_array.IndexOf(ObservedDatasNoRep[r].End) == p1) {
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
                    strClosure.Append(AllPoint_array[k2] + "-" + AllPoint_array[k1]);
                    double W = (ObservedDatasNoRep[i].HeightDiff + diff[k1]) * 1000;   //闭合差
                    double SS = S[k1] + ObservedDatasNoRep[i].Distance; //环长
                    double limit = (roundi * Math.Sqrt(SS));
                    strClosure.AppendLine("\r\n高差闭合差：" + (-W).ToString("f2") + "(mm)");
                    strClosure.AppendLine("平原限差：" + limit.ToString("f4") + "(mm)");
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
        private string LineClosure(double roundi, string split, string space) {
            int m_knPnumber = KnownPoints_array.Count;
            int m_Pnumber = AllPoint_array.Count;
            StringBuilder strClosure = new StringBuilder();

            int line_N = 0;

            strClosure.AppendLine(split);
            strClosure.AppendLine(space + "路线闭合差计算结果");
            strClosure.AppendLine(split);
            if (m_knPnumber < 2)
                return strClosure.AppendLine("已知点数小于2").ToString(); // 已知点数小于2
            int[] neighbor = new int[m_Pnumber];       //邻接点数组
            double[] diff = new double[m_Pnumber]; //高差累加值数组
            double[] S = new double[m_Pnumber];    //路线长累加值数组

            for (int ii = 0; ii < KnownPoints_array.Count; ii++) {
                FindShortPath(ii, -1, neighbor, diff, S); //搜索最短路线，用所有观测值
                for (int jj = ii + 1; jj < KnownPoints_array.Count; jj++) {
                    if (neighbor[jj] < 0) {
                        // strClosure.Append(AllPoint_array[ii] + "-" + AllPoint_array[jj] + "之间找到不到最短路线");
                        continue;
                    }
                    // 输出附合路线上的点号
                    line_N++;
                    strClosure.AppendLine("线路号：" + line_N);
                    strClosure.Append("线路点号：");
                    int k = jj;
                    while (true) {
                        strClosure.Append(AllPoint_array[k] + "-");
                        k = neighbor[k];
                        if (k == ii) break;
                    }
                    strClosure.Append(AllPoint_array[ii]);

                    //闭合差计算，限差计算与输出
                    double W = (KnownPoints[ii].Height + diff[jj] - KnownPoints[jj].Height) * 1000; // 闭合差
                    double limit = roundi * Math.Sqrt(S[jj]);  // 限差
                    strClosure.AppendLine("\r\n高差闭合差：" + (-W).ToString("#0.00") + "(mm)");
                    strClosure.AppendLine("平原限差：" + limit.ToString("f4") + "(mm)");
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
            var pointNum = AllPoint_array.Count;
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
            ObservedDatas.ForEach(pd => {
                var rowIdx = AllPoint_array.IndexOf(pd.Start);
                var columnIdx = AllPoint_array.IndexOf(pd.End);
                cost[rowIdx, columnIdx] = pd.Distance;
                cost[columnIdx, rowIdx] = pd.Distance;
            });
            #endregion

            string strLoop = LoopClosure(Options.Coefficient, split, space);
            string strLine = LineClosure(Options.Coefficient, split, space);
            FileHelper.WriteStrToTxt(strLine + strLoop, OutpathClosure);
        }

        /// <summary>
        /// 粗差探测
        /// </summary>
        public void FindGrossError(string split, string space, string path) {
            double U = Calc.re_norm(1 - Options.Alpha / 2);
            CalcApproximateHeight();
            PVV = 0;
            int k;
            List<ObservedData> odError = new List<ObservedData>(); //保存含有粗差的观测数据
            ArrayList V_err = new ArrayList();
            ArrayList threshold_err = new ArrayList();
            for (k = 0; ; k++) {
                Calc_P();
                Calc_B();
                Calc_l();
                Calc_NBB();
                Calc_dX();
                Calc_PVV();
                double max_v = 0;
                int max_i = 0;
                for (int i = 0; i < N; i++) {
                    if (P[i, i] < 1E-10) continue;
                    int startIdx = AllPoint_array.IndexOf(observedDatas[i].Start);
                    int endIdx = AllPoint_array.IndexOf(observedDatas[i].End);
                    double qii = B[startIdx, startIdx];
                    double qjj = B[endIdx, endIdx];
                    double qij = B[startIdx, endIdx];
                    double qv = 1 / P[i, i] - (qii + qjj - 2 * qij);
                    double mv = Math.Sqrt(qv) * Options.Sigma;
                    double vi = V[i, 0] / mv;
                    if (Math.Abs(vi) > max_v) {
                        max_v = Math.Abs(vi);
                        max_i = i;
                    }
                }
                if (max_v > U) {
                    odError.Add(observedDatas[max_i]);
                    V_err.Add(V[max_i, 0]);
                    threshold_err.Add(max_v);
                    observedDatas.RemoveAt(max_i);
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
                sb.AppendLine($"{"序号",titlePad}{"起点",titlePad}{"终点",titlePad}{"高差(m)",titlePad}{"距离(km)",titlePad}{"改正数(mm)",titlePad}");
                sb.AppendLine(split);
                for (int i = 0; i < odError.Count; i++) {
                    //if (P[i, i] > 1E-10) continue;
                    //sb.AppendLine($"{i + 1},{observedDatas[i].Start},{observedDatas[i].End},{V[i, 0]}");
                    sb.AppendLine($"{i + 1,pad}{odError[i].Start,pad}{odError[i].End,pad}{odError[i].HeightDiff,pad:#0.00000}{odError[i].Distance,pad:#0.0000}{Convert.ToDouble(V_err[i]) * 1000,pad:#0.0000}");
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
            // 自由网平差要求每个点有先验高程值
            Calc_P();
            Calc_B();
            Calc_l();
            Calc_NBB();
            for (int i = 0; i < T; i++) {
                for (int j = 0; j <= i; j++) {
                    if (UnknownPoints[i].IsStable && UnknownPoints[j].IsStable) {
                        NBB[i, j] += 1.0 / StablePnumber;
                    }
                }
            }
            Calc_dX();
            x_total = x;
            V_total = V;
            int iteration_count = 1;
            while (!isLimit(x)) {
                iteration_count++;
                Calc_l();
                Calc_NBB();
                for (int i = 0; i < T; i++) {
                    for (int j = 0; j <= i; j++) {
                        if (UnknownPoints[i].IsStable && UnknownPoints[j].IsStable) {
                            NBB[i, j] += 1.0 / StablePnumber;
                        }
                    }
                }
                Calc_dX();
                x_total += x;
                V_total += V;
            }
            //求出权逆阵Qx
            for (int i = 0; i < T; i++) {
                for (int j = 0; j <= i; j++) {
                    NBB[i, j] -= 1.0 / StablePnumber;
                }
            }
            Calc_PVV();
            Mu = Math.Sqrt(PVV / (R - 1));
            return iteration_count;
        }

        /// <summary>
        /// 自由网平差
        /// </summary>
        public int FreeNetAdjust() {
            // 自由网平差要求每个点有先验高程值
            CalcApproximateHeight();
            // 生成高程近似值矩阵
            Calc_P();
            Calc_B();


            Calc_l();
            Calc_NBB();

            for (int i = 0; i < M; i++) {
                for (int j = 0; j < M; j++) {
                    NBB[i, j] += 1.0 / T;
                }
            }
            Calc_dX();
            x_total = x;
            V_total = V;

            int iteration_count = 1;
            // 迭代计算，直到未知数改正数为0
            while (!isLimit(x)) {
                iteration_count++;
                Calc_l();
                Calc_NBB();
                for (int i = 0; i < M; i++) {
                    for (int j = 0; j < M; j++) {
                        NBB[i, j] += 1.0 / T;
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
                    Qxx[i, j] = NBB[i, j] - 1.0 / T;
                }
            }
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
                var startIdx = GetStartIdx(i);
                var endIdx = GetEndIdx(i);
                Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * Mu;
            }

            // 求观测值协因数矩阵
            Qll = B * NBB.Inverse() * B.Transpose();

            var Qvv = Qll - B * NBB.Inverse() * B.Transpose();

            return iteration_count;
        }

        /// <summary>
        /// 输出自由网平差结果
        /// </summary>
        /// <param name="split"></param>
        /// <param name="space"></param>
        public void ExportFreeNetworkResult(string split, string space, string path) {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(split);
            sb.AppendLine(space + "秩亏自由网平差结果");
            sb.AppendLine(split);
            sb.AppendLine(space + "高差观测数据");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"起点",titlePad}{"终点",titlePad}{"高差(m)",titlePad}{"距离(km)",titlePad}{"权重",titlePad}");
            for (int i = 0; i < N; i++) {
                sb.AppendLine($"{i + 1,pad}{ObservedDatas[i].Start,pad}{ObservedDatas[i].End,pad}{ObservedDatas[i].HeightDiff.ToString("#0.00000"),pad}{ObservedDatas[i].Distance.ToString("#0.0000"),pad}{P[i, i].ToString("#0.000"),pad}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "高程平差值及其精度");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"点名",titlePad}{"近似高程(m)",titlePad}{"累计改正数(mm)",titlePad}{"高程平差值(m)",titlePad}{"中误差(mm)",titlePad}");
            sb.AppendLine(split);
            for (int i = 0; i < M; i++) {
                sb.AppendLine($"{i + 3,pad}{AllPoints[i].Number,pad}{AllPoints[i].Height,pad - 4:#0.00000}{x_total[i, 0] * 1000,pad:#0.00}{X[i, 0],pad - 2:#0.00000}{Mh_P[i],pad:#0.00}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "观测值平差值及其精度");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"起点",titlePad}{"终点",titlePad}{"高差平差值(m)",titlePad}{"距离(km)",titlePad}{"中误差(mm)",titlePad}");
            sb.AppendLine(split);
            for (int i = 0; i < N; i++) {
                sb.AppendLine($"{i + 1,pad}{ObservedDatas[i].Start,pad}{ObservedDatas[i].End,pad}{L[i],pad - 5:#0.00000}{ObservedDatas[i].Distance,pad:#0.0000}{Mh_L[i],pad:#0.00}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "    水准网总体信息");
            sb.AppendLine(split);
            sb.AppendLine(space + "未知高程点数：".PadRight(8) + UnknownPoints.Count);
            sb.AppendLine(space + "高差测段数：".PadRight(8) + ObservedDatasNoRep.Count);
            sb.AppendLine(space + "观测总距离：".PadRight(8) + TotalDistence.ToString("#0.000") + "(km)");
            sb.AppendLine(space + "PVV：".PadRight(8) + PVV.ToString("#0.000"));
            sb.AppendLine(space + "验后单位权中误差：".PadRight(8) + Mu.ToString("#0.000"));

            sb.AppendLine(split);
            FileHelper.WriteStrToTxt(sb.ToString(), path);
        }

        public void Prepare4ConstraintNetwork() {
            AllPoint_array = Commom.Clone(KnownPoints_array);
            for (int i = 0; i < UnknownPoints_array.Count; i++) {
                AllPoint_array.Add(UnknownPoints_array[i]);
            }

            AllPoints = Commom.Clone(KnownPoints);
            for (int i = 0; i < UnknownPoints.Count; i++) {
                AllPoints.Add(UnknownPoints[i]);
            }
        }
        public void Prepare4RankDefectNetwork() {
            this.AllPoints = UnknownPoints;
            this.AllPoint_array = UnknownPoints_array;
        }
    }
}
