﻿using LevelnetAdjustment.form;
using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using MathNet.Numerics.LinearAlgebra;
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

namespace LevelnetAdjustment {
    public partial class MainForm : Form {

        /// <summary>
        /// 观测数据列表
        /// </summary>
        public List<ObservedData> ObservedDatas { get; set; }
        /// <summary>                                        
        /// 已知点列表(点号，高程) 
        /// </summary>
        public List<PointData> KnownPoints { get; set; }
        /// <summary>
        /// 定义列表存储近似高程(点号，高程)
        /// </summary>
        public List<PointData> UnknownPoint { get; set; }
        /// <summary>
        /// 定义列表存储近似高程(按照unknownPoints_array点号顺序排列)
        /// </summary>
        public List<PointData> UnknownPoint_new { get; set; }
        /// <summary>
        /// 所有点的信息(点号，高程)
        /// </summary>
        public List<PointData> AllKnownPoint { get; set; }
        /// <summary>
        /// 去除重复边的观测数据
        /// </summary>
        public List<ObservedData> ObservedDatasNoRep { get; set; }
        /// <summary>
        /// 未知点点号数组
        /// </summary>
        public ArrayList UnknownPoints_array { get; set; }
        /// <summary>
        /// 已知点点号数组
        /// </summary>
        public ArrayList KnownPoints_array { get; set; }
        /// <summary>
        /// 所有点号数组
        /// </summary>
        public ArrayList AllPoint_array { get; set; }
        /// <summary>
        /// 原始观测数据
        /// </summary>
        public List<RawData> RawDatas { get; set; }
        /// <summary>
        /// 水准等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 闭合差限差系数
        /// </summary>
        private int coefficient;
        public int Coefficient {
            get => coefficient;

            set {
                int c;
                switch (value) {
                    case 2:
                        c = 4;
                        break;
                    case 3:
                        c = 12;
                        break;
                    case 4:
                        c = 20;
                        break;
                    case 5:
                        c = 30;
                        break;
                    default:
                        c = 0;
                        break;
                }
                coefficient = c;
            }
        }
        /// <summary>
        /// 观测数
        /// </summary>
        public int N { get; set; }
        /// <summary>
        /// 必要观测数
        /// </summary>
        public int T { get; set; }
        /// <summary>
        /// 多余观测数
        /// </summary>
        public int R { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = "";
        /// <summary>
        /// 输出文件格式
        /// </summary>
        readonly string split = "---------------------------------------------------------------------------------";
        readonly string space = "                             ";
        /// <summary>
        /// 闭合差文件输出路径
        /// </summary>
        public string OutpathClosure { get; set; } = "";
        /// <summary>
        /// 平差文件输出路径
        /// </summary>
        public string OutpathAdj { get; set; } = "";

        Matrix<double> P, C, l, B, W, x, X, V, VTPV, Qxx, x_total;
        double sigma0, PVV;
        double[] L, Mh_P, Mh_L;

        public int PowerMethod { get; set; } = 0; //定权方式 0按距离定权 1按测段数定权
        public double limit { get; set; } = 0.01; //平差迭代限差

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm() {
            InitializeComponent();

        }

        private void CalcParams() {
            OutpathAdj = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + "平差结果.ou1");
            OutpathClosure = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + "闭合差计算结果.ou2");
            Coefficient = Level;//闭合差系数
            UnknownPoints_array = new ArrayList();
            KnownPoints_array = new ArrayList();
            KnownPoints.ForEach(item => {
                KnownPoints_array.Add(item.Number);
            });
            ObservedDatas.ForEach(item => {
                if (!UnknownPoints_array.Contains(item.Start) && !KnownPoints_array.Contains(item.Start)) {
                    UnknownPoints_array.Add(item.Start);
                }
                if (!UnknownPoints_array.Contains(item.End) && !KnownPoints_array.Contains(item.End)) {
                    UnknownPoints_array.Add(item.End);
                }
            });
            N = ObservedDatas.Count; //观测数
            T = UnknownPoints_array.Count; //必要观测数
            R = N - T; //多余观测数
            AllPoint_array = null;
            AllPoint_array = Commom.Clone(KnownPoints_array);
            for (int i = 0; i < UnknownPoints_array.Count; i++) {
                AllPoint_array.Add(UnknownPoints_array[i]);
            }
            Console.WriteLine($"观测数：{N}\n必要观测数：{T}\n多余观测数：{R}");
        }

        /// <summary>
        /// 打开任意文本文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDropItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = false,
                Title = "打开",
                Filter = "COSA文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                FilterIndex = 1,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                if (string.IsNullOrEmpty(FilePath)) {
                    FilePath = openFile.FileName;
                }

                KnownPoints = new List<PointData>();
                ObservedDatas = new List<ObservedData>();
                ObservedDatasNoRep = new List<ObservedData>();
                var tup = FileHelper.ReadOriginalFile(KnownPoints, ObservedDatas, ObservedDatasNoRep, openFile.FileName);
                Level = tup.Item1;
                PowerMethod = tup.Item2;

                CalcParams();
                FileView fileView = new FileView(new string[] { openFile.FileName }) {
                    MdiParent = this,
                };
                fileView.Show();
            }
            else {
                return;
            }
        }

        /// <summary>
        /// 计算近似高程
        /// </summary>
        void CalcApproximateHeight() {
            AllKnownPoint = Commom.Clone(KnownPoints);
            UnknownPoint = new List<PointData>();
            int count = 0;
            // 如果未知点近似高程未计算完，重复循环
            while (UnknownPoint.Count < T) {

                ObservedDatas.ForEach(item => {
                    //在已知点里面分别查找起点和终点
                    var startIndex = AllKnownPoint.FindIndex(p => p.Number == item.Start);
                    var endIndex = AllKnownPoint.FindIndex(p => p.Number == item.End);
                    //如果起点是已知点，终点是未知点
                    if (startIndex > -1 && endIndex <= -1) {
                        PointData pd = new PointData {
                            Number = item.End,
                            Height = AllKnownPoint[startIndex].Height + item.HeightDiff
                        };
                        UnknownPoint.Add(pd);
                        AllKnownPoint.Add(pd);
                    }
                    //如果终点是已知点，起点是未知点
                    if (endIndex > -1 && startIndex <= -1) {
                        PointData pd = new PointData {
                            Number = item.Start,
                            Height = AllKnownPoint[endIndex].Height - item.HeightDiff
                        };
                        UnknownPoint.Add(pd);
                        AllKnownPoint.Add(pd);
                    }
                });
                if (count == UnknownPoint.Count) {
                    throw new Exception("无法计算未知点近似高程");
                }
                count = UnknownPoint.Count;
            }

            // 将未知点按照顺序排列
            UnknownPoint_new = new List<PointData>();
            for (int i = 0; i < T; i++) {
                UnknownPoint_new.Add(UnknownPoint.Find(p => p.Number == UnknownPoints_array[i].ToString()));
            }
        }



        /// <summary>
        /// 间接平差
        /// </summary>
        void IndirectAdjustment() {
            // 生成观测值近似值矩阵
            double[] Xs = new double[T];
            for (int i = 0; i < T; i++) {
                Xs[i] = UnknownPoint_new[i].Height;
            }
            X = Matrix<double>.Build.DenseOfColumnArrays(Xs);


            // 求权阵P
            double[] powerArray = new double[N];// 定义数据存储权的值
            for (int i = 0; i < ObservedDatas.Count; i++) {
                powerArray[i] = 1 / ObservedDatas[i].Distance;
            }
            P = Matrix<double>.Build.DenseOfDiagonalArray(powerArray);// 根据数组生成权阵
            //Console.WriteLine(P.ToString());


            // 求误差方程系数阵C
            C = Matrix<double>.Build.Dense(N, T, 0); //生成n*t矩阵
            for (int i = 0; i < N; i++) {
                var startIndex = UnknownPoints_array.IndexOf(ObservedDatas[i].Start);
                var endIndex = UnknownPoints_array.IndexOf(ObservedDatas[i].End);
                if (startIndex > -1) {
                    C[i, startIndex] = -1;
                }
                if (endIndex > -1) {
                    C[i, endIndex] = 1;
                }
            }
            //Console.WriteLine(C.ToString());


            // 求误差方程常数项l
            double[] ls = new double[N];
            for (int i = 0; i < N; i++) {
                PointData startPoint = AllKnownPoint.Find(p => p.Number == ObservedDatas[i].Start);
                PointData endPoint = AllKnownPoint.Find(p => p.Number == ObservedDatas[i].End);
                ls[i] = startPoint.Height + ObservedDatas[i].HeightDiff - endPoint.Height;
            }// 计算每个常数项的值      
            l = Matrix<double>.Build.DenseOfColumnArrays(ls);
            //Console.WriteLine(l.ToString());


            // 求法方程系数阵B
            B = C.Transpose() * P * C;
            W = C.Transpose() * P * l;


            // 求解x
            x = B.Inverse() * W;
            //Console.WriteLine(x.ToString());
            x_total = x;

            // 求未知点的真实值
            X += x;

            // 求观测值改正数
            V = C * x - l;

            // 求观测值平差值
            L = new double[N];
            for (int i = 0; i < N; i++) {
                L[i] = V[i, 0] + ObservedDatas[i].HeightDiff;
            }

        }

        /// <summary>
        /// 精度评定
        /// </summary>
        void PrecisionEstimation() {
            // 求单位权中误差
            VTPV = V.Transpose() * P * V;
            sigma0 = Math.Sqrt(double.Parse(VTPV[0, 0].ToString()) / R) * 1000;
            //var sigma1 = Math.Sqrt((l.Transpose() * P * l - W.Transpose() * x)[0, 0] / R);
            //Console.WriteLine(sigma0.ToString() + "," + sigma1.ToString());



            // 求未知数的协因数阵
            Qxx = B.Inverse();
            //Console.WriteLine(Qxx.ToString());

            // 求观测值协因数矩阵
            /*var Qll = C * Qxx * C.Transpose();
            Console.WriteLine(Qll.ToString());*/

            // 求高程平差值中误差
            Mh_P = new double[T];
            for (int i = 0; i < T; i++) {
                Mh_P[i] = Math.Sqrt(Qxx[i, i]) * sigma0;
            }

            // 求高差平差值中误差
            Mh_L = new double[N];
            for (int i = 0; i < N; i++) {
                var startIdx = UnknownPoints_array.IndexOf(ObservedDatas[i].Start);
                var endIdx = UnknownPoints_array.IndexOf(ObservedDatas[i].End);
                //Mh_L[i] = Math.Sqrt(Qll[startIdx, startIdx] + Qll[endIdx, endIdx] - 2 * Qll[startIdx, endIdx]) * sigma0;
                if (startIdx == -1 && endIdx == -1) {
                    Mh_L[i] = 0;
                }
                else if (startIdx == -1 || endIdx == -1) {
                    if (startIdx == -1) {
                        Mh_L[i] = Math.Sqrt(Qxx[0, 0] + Qxx[endIdx, endIdx] - 2 * Qxx[0, endIdx]) * sigma0;
                    }
                    else {
                        Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[0, 0] - 2 * Qxx[startIdx, 0]) * sigma0;
                    }

                }
                else {
                    Mh_L[i] = Math.Sqrt(Qxx[startIdx, startIdx] + Qxx[endIdx, endIdx] - 2 * Qxx[startIdx, endIdx]) * sigma0;
                }

            }

            // 求PVV
            PVV = sigma0 * sigma0 * R;
            /* for (int i = 0; i < N; i++) {
                 PVV += P[i, i] * L[i] * L[i];
             }*/
        }

        // 是否满足限差，满足返回true
        bool isLimit(Matrix<double> mx) {
            for (int i = 0; i < mx.RowCount; i++) {
                if (Math.Abs(mx[i, 0] * 1000) > limit) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 平差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelnetDropItem_Click(object sender, EventArgs e) {
            if (File.Exists(OutpathAdj)) {
                if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    return;
                }
            }
            if (ObservedDatas == null) {
                throw new Exception("请打开观测文件");
            }



            CalcApproximateHeight();

            IndirectAdjustment();

            int iteration_count = 1;

            // 迭代计算，直到未知数改正数为0
            while (!isLimit(x)) {
                iteration_count++;


                // 求误差方程常数项l
                double[] ls = new double[N];
                for (int i = 0; i < N; i++) {
                    var startIdx = AllPoint_array.IndexOf(ObservedDatas[i].Start);
                    var startH = startIdx > KnownPoints_array.Count - 1 ? X[startIdx - KnownPoints_array.Count, 0] : KnownPoints[startIdx].Height;
                    var endIdx = AllPoint_array.IndexOf(ObservedDatas[i].End);
                    var endH = endIdx > KnownPoints_array.Count - 1 ? X[endIdx - KnownPoints_array.Count, 0] : KnownPoints[endIdx].Height;
                    ls[i] = startH + ObservedDatas[i].HeightDiff - endH;
                }// 计算每个常数项的值      
                l = Matrix<double>.Build.DenseOfColumnArrays(ls);
                B = C.Transpose() * P * C;
                W = C.Transpose() * P * l;
                // 求解x
                x = B.Inverse() * W;
                x_total += x;
                //Console.WriteLine(x.ToString());
                // 求未知点的真实值
                X += x;

                // 求观测值改正数
                V = C * x - l;

                // 求观测值平差值
                L = new double[N];
                for (int i = 0; i < N; i++) {
                    L[i] = V[i, 0] + ObservedDatas[i].HeightDiff;
                }
            }
            PrecisionEstimation();

            // 求观测距离总和
            double totalS = 0;
            ObservedDatas.ForEach(ll => totalS += ll.Distance);


            // 保存文件
            const int pad = -13;
            const int titlePad = -11;
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(split);
            //sb.AppendLine(space + "未知点近似高程");
            //sb.AppendLine(split);
            //sb.AppendLine($"{"序号",titlePad}{"点名",titlePad}{"高程(m)",titlePad}");
            //for (int i = 0; i < T; i++) {
            //    sb.AppendLine($"{i + 1,pad}{UnknownPoint_new[i].Number,pad}{UnknownPoint_new[i].Height,pad:#0.0000}");
            //}
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
            for (int i = 0; i < KnownPoints.Count; i++) {
                sb.AppendLine($"{i + 1,pad}{KnownPoints[i].Number,pad}{KnownPoints[i].Height,pad - 4:#0.00000}{0,pad:#0.00}{KnownPoints[i].Height,pad - 2:#0.00000}");
            }
            for (int i = 0; i < T; i++) {
                sb.AppendLine($"{i + 3,pad}{UnknownPoint_new[i].Number,pad}{UnknownPoint_new[i].Height,pad - 4:#0.00000}{x_total[i, 0] * 1000,pad:#0.00}{X[i, 0],pad - 2:#0.00000}{Mh_P[i],pad:#0.00}");
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
            sb.AppendLine(space + "    高程网总体信息");
            sb.AppendLine(split);
            sb.AppendLine(space + "已知高程点数：".PadRight(8) + KnownPoints.Count);
            sb.AppendLine(space + "未知高程点数：".PadRight(8) + UnknownPoint.Count);
            sb.AppendLine(space + "高差观测数：".PadRight(8) + ObservedDatas.Count);
            sb.AppendLine(space + "观测总距离：".PadRight(8) + totalS.ToString("#0.000") + "(km)");
            sb.AppendLine(space + "PVV：".PadRight(8) + PVV.ToString("#0.000"));
            sb.AppendLine(space + "验后单位权中误差：".PadRight(8) + sigma0.ToString("#0.000"));

            sb.AppendLine(split);


            FileHelper.WriteStrToTxt(sb.ToString(), OutpathAdj);
            FileView fileView = new FileView(new string[] { OutpathAdj }) {
                MdiParent = this,
            };
            MessageBox.Show($"水准网平差完毕，迭代次数：{iteration_count}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
        }

        /// <summary>
        /// 计算闭合差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosureErrorDropItem_Click(object sender, EventArgs e) {

            if (File.Exists(OutpathClosure)) {
                if (MessageBox.Show("闭合差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
                    return;
                }
            }

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

            #region 搜索附和水准路线已知点最短路径，采用Dijkstra算法 
            /*// 参考 https://blog.csdn.net/weixin_42724039/article/details/81255726
            // https://www.codetd.com/article/9950057
            var path = new List<int>();
            if (KnownPoints_array.Count < 2) {
                Console.WriteLine("已知点个数小于2");
            }
            else {
                for (int ii = 0; ii < KnownPoints_array.Count; ii++) {
                    for (int jj = ii + 1; jj < KnownPoints_array.Count; jj++) {
                        int g = ii; // 起点
                        int h = jj; // 终点
                        int[] book = new int[pointNum]; //book[i]=0表示此结点最短路未确定，为1表示已确定
                        double[] distance = new double[pointNum];//出发点到各点最短距离
                        int[] last1 = new int[pointNum];//存储最短路径，每个结点的上一个结点
                        double min;
                        int u = 0;
                        //初始化distance，这是出发点到各点的初始距离
                        for (int i = 0; i < pointNum; i++) {
                            distance[i] = cost[g, i];
                        }
                        //初始化出发点的book
                        for (int i = 0; i < pointNum; i++) { last1[i] = g; }
                        last1[g] = -1;
                        book[g] = 1;
                        //核心算法
                        for (int i = 0; i < pointNum; i++) {
                            min = inf;

                            //找到离g号结点最近的点
                            for (int j = 0; j < pointNum; j++) {
                                if (book[j] == 0 && distance[j] < min && distance[j] != 0) {
                                    min = distance[j];
                                    u = j;
                                }
                            }
                            book[u] = 1;
                            for (int v = 0; v < pointNum; v++) {
                                if (cost[u, v] < inf && cost[u, v] != 0) {
                                    if (distance[v] > distance[u] + cost[u, v]) {
                                        distance[v] = distance[u] + cost[u, v];
                                        last1[v] = u;
                                    }

                                }
                            }
                        }
                        int k = h;
                        path.Add(k);
                        while (k != g) {
                            if (distance[k] >= inf) {
                                Console.WriteLine("无法到达{0}", h);
                                break;
                            }
                            k = last1[k];
                            path.Add(k);
                        }
                        path.Reverse();
                        Console.WriteLine("{0}到{1}最短路径", AllPoint_array[g], AllPoint_array[h]);
                        double diff = 0;
                        ObservedData od = new ObservedData();
                        for (int i = 0; i < path.Count; i++) {
                            Console.Write(AllPoint_array[path[i]] + " ");
                            if (i >= 1) {
                                int zheng = ObservedDatas.FindIndex(p => (p.Start == AllPoint_array[path[i - 1]].ToString() && p.End == AllPoint_array[path[i]].ToString()));
                                int fan = ObservedDatas.FindIndex(p => (p.Start == AllPoint_array[path[i]].ToString() && p.End == AllPoint_array[path[i - 1]].ToString()));
                                if (zheng != -1) {
                                    diff += ObservedDatas[zheng].HeightDiff;
                                }
                                else if (fan != -1) {
                                    diff -= ObservedDatas[fan].HeightDiff;
                                }
                            }
                        }
                        //foreach (int i in path) {
                        //    Console.Write(AllPoint_array[i] + " ");
                        //}
                        Console.WriteLine();
                        Console.WriteLine(path.Count);
                        double w = KnownPoints[ii].Height + diff - KnownPoints[jj].Height;
                        Console.WriteLine("闭合差：" + w * 1000 + "(mm)");
                    }
                }
            }
            */
            #endregion

            Console.WriteLine("最小独立闭合环的个数：{0}-{1}-{2}", ObservedDatasNoRep.Count - (T + 2) + 1, ObservedDatasNoRep.Count, ObservedDatas.Count);
            string strLoop = LoopClosure(Coefficient);
            string strLine = LineClosure(Coefficient);

            FileHelper.WriteStrToTxt(strLine + strLoop, OutpathClosure);
            FileView fileView = new FileView(new string[] { OutpathClosure }) {
                MdiParent = this,
            };
            MessageBox.Show("闭合差计算完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
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
                    int p1 = AllPoint_array.IndexOf(ObservedDatasNoRep[j].Start); //起点点号
                    int p2 = AllPoint_array.IndexOf(ObservedDatasNoRep[j].End); //终点点号
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

        private void OptionDropItem_Click(object sender, EventArgs e) {
            Setting setting = new Setting(PowerMethod, limit, Level);
            setting.TransfEvent += frm_TransfEvent;
            setting.ShowDialog();
        }

        //事件处理方法
        void frm_TransfEvent(int method, double limit, int level) {
            this.PowerMethod = method;
            this.limit = limit;
            this.Level = level;
        }

        /// <summary>
        /// 最小独立环闭合差计算	
        /// </summary>
        /// <param name="roundi"></param>
        string LoopClosure(double roundi) {
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
                int k1 = AllPoint_array.IndexOf(ObservedDatasNoRep[i].Start); //起点点号;
                int k2 = AllPoint_array.IndexOf(ObservedDatasNoRep[i].End); //终点点号
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
                        strClosure.Append(AllPoint_array[p1] + "-");

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
        private string LineClosure(double roundi) {
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

        private void ExitDropItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            DialogResult result = MessageBox.Show("您确定要关闭软件吗？", "退出提示",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.ExitThread();
            else {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 创建新的观测文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewDropItem_Click(object sender, EventArgs e) {
            FileView fileView = new FileView(new string[] { "" }) {
                MdiParent = this,
            };
            fileView.Show();
        }

        private void AboutDropItem_Click(object sender, EventArgs e) {
            About about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// 读取水准仪原始数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RawDataDropItem_Click(object sender, EventArgs e) {
            if (RawDatas == null) {
                RawDatas = new List<RawData>();
            }
            if (ObservedDatas == null) {
                ObservedDatas = new List<ObservedData>();
            }
            if (KnownPoints == null) {
                KnownPoints = new List<PointData>();
            }
            if (ObservedDatasNoRep == null) {
                ObservedDatasNoRep = new List<ObservedData>();
            }
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "DAT观测文件|*.dat;*.DAT|GSI-8观测文件|*.gsi;*.GSI|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                if (string.IsNullOrEmpty(FilePath)) {
                    FilePath = openFile.FileNames[0];
                }
                foreach (var item in openFile.FileNames) {
                    if (Path.GetExtension(item).ToLower() == ".dat") {
                        FileHelper.ReadDAT(item, RawDatas, ObservedDatas);
                    }
                    else if (Path.GetExtension(item).ToLower() == ".gsi") {
                        FileHelper.ReadGSI(item, RawDatas, ObservedDatas, KnownPoints);
                    }
                }
                CalcParams();
                Console.WriteLine("123");
                FileView fv = new FileView(openFile.FileNames) { MdiParent = this };
                fv.Show();
            }
        }



        /// <summary>
        /// 读取已知点数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KnDropItem_Click(object sender, EventArgs e) {
            if (KnownPoints == null) {
                KnownPoints = new List<PointData>();
            }
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = true,
                Title = "打开",
                Filter = "文本文件|*.txt;*.TXT|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                FileHelper.ReadGSI(openFile.FileName, KnownPoints);
            }
        }

        /// <summary>
        /// 导出观测手簿
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandbookDropItem_Click(object sender, EventArgs e) {

            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "Excel 工作簿(*.xlsx)|*.xlsx|Excel 97-2003 工作簿(*.xls)|*.xls",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = FilePath != "" ? Path.GetFileName(Path.GetFileNameWithoutExtension(FilePath)) : "",
                InitialDirectory = FilePath != "" ? Path.GetDirectoryName(FilePath) : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                ExceHelperl.ExportHandbook(RawDatas, ObservedDatas, saveFileDialog.FileName);
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearDropItem_Click(object sender, EventArgs e) {
            ObservedDatas = null;
            KnownPoints = null;
            UnknownPoint = null;
            UnknownPoint_new = null;
            AllKnownPoint = null;
            ObservedDatasNoRep = null;
            UnknownPoints_array = null;
            UnknownPoints_array = null;
            KnownPoints_array = null;
            AllPoint_array = null;
            RawDatas = null;
            Level = 2;
            Coefficient = Level;
            FilePath = null;
            OutpathClosure = null;
            OutpathAdj = null;
        }

        /// <summary>
        /// 导出COSA按距离定权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisPower_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "COSA水准观测文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = Path.GetFileName(Path.GetFileNameWithoutExtension(FilePath)),
                InitialDirectory = Path.GetDirectoryName(FilePath),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                FileHelper.ExportCOSA(ObservedDatas, KnownPoints, saveFileDialog.FileName);
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void StationPower_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "COSA水准观测文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = Path.GetFileName(Path.GetFileNameWithoutExtension(FilePath)),
                InitialDirectory = Path.GetDirectoryName(FilePath),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                FileHelper.ExportCOSAStationPower(ObservedDatas, KnownPoints, saveFileDialog.FileName);
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

