using LevelnetAdjustment.form;
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
        public MainForm() {
            InitializeComponent();
        }
        public List<PointData> KnownPoints { get; set; } //已知点列表
        public List<ObservedData> ObservedDatas { get; set; } //观测数据列表
        public ArrayList UnknownPoints_array { get; set; } //已知点点号数组
        public ArrayList KnownPoints_array { get; set; } //未知点点号数组
        public List<PointData> UnknownPoint { get; set; } // 定义列表存储近似高程
        public List<PointData> UnknownPoint_new { get; set; } // 定义列表存储近似高程(按照unknownPoints_array点号顺序排列)
        public List<PointData> AllKnownPoint { get; set; } //所有点的信息
        public List<ObservedData> ObservedDatasNoRep { get; set; } // 去除重复边的观测数据
        public ArrayList AllPoint { get; set; }
        public int N { get; set; } //观测数
        public int T { get; set; } //必要观测数
        public int R { get; set; } //多余观测数
        public string Path { get; set; } //文件路径
        /// <summary>
        /// 打开任意文本文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDropItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFile = new OpenFileDialog {
                Multiselect = false,
                Title = "打开",
                Filter = "水准观测文件(*.INP)|*.INP",
                FilterIndex = 1,
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                Path = openFile.FileName;
                KnownPoints = new List<PointData>();
                ObservedDatas = new List<ObservedData>();
                ObservedDatasNoRep = new List<ObservedData>();
                FileHelper.readOriginalFile(KnownPoints, ObservedDatas, ObservedDatasNoRep, openFile.FileName);
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
                Console.WriteLine($"观测数：{N}\n必要观测数：{T}\n多余观测数：{R}");
                FileView fileView = new FileView(openFile.FileName) {
                    MdiParent = this,
                };
                fileView.Show();
            }
            else {
                return;
            }
        }

        /// <summary>
        /// 打开原始观测数据，进行平差
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelnetDropItem_Click(object sender, EventArgs e) {
            if (ObservedDatas == null) {
                throw new Exception("请打开观测文件");
            }

            #region 1.求未知点的近似高程
            AllKnownPoint = Commom.Clone(KnownPoints);
            UnknownPoint = new List<PointData>();
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
                Console.WriteLine(UnknownPoint.Count);
            }
            #endregion

            // 将未知点按照顺序排列
            UnknownPoint_new = new List<PointData>();
            for (int i = 0; i < T; i++) {
                UnknownPoint_new.Add(UnknownPoint.Find(p => p.Number == UnknownPoints_array[i].ToString()));
            }


            // 生成观测值近似值矩阵
            double[] Xs = new double[T];
            for (int i = 0; i < T; i++) {
                Xs[i] = UnknownPoint_new[i].Height;
            }
            var X = Matrix<double>.Build.DenseOfColumnArrays(Xs);


            // 求权阵P
            double[] powerArray = new double[N];// 定义数据存储权的值
            for (int i = 0; i < ObservedDatas.Count; i++) {
                powerArray[i] = 1 / ObservedDatas[i].Distance;
            }
            var P = Matrix<double>.Build.DenseOfDiagonalArray(powerArray);// 根据数组生成权阵
            Console.WriteLine(P.ToString());


            // 求误差方程系数阵C
            var C = Matrix<double>.Build.Dense(N, T, 0); //生成n*t矩阵
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
            Console.WriteLine(C.ToString());


            // 求误差方程常数项l
            double[] ls = new double[N];
            for (int i = 0; i < N; i++) {
                PointData startPoint = AllKnownPoint.Find(p => p.Number == ObservedDatas[i].Start);
                PointData endPoint = AllKnownPoint.Find(p => p.Number == ObservedDatas[i].End);
                ls[i] = startPoint.Height + ObservedDatas[i].HeightDiff - endPoint.Height;
            }// 计算每个常数项的值      
            var l = Matrix<double>.Build.DenseOfColumnArrays(ls);
            Console.WriteLine(l.ToString());


            // 求法方程系数阵B
            var B = C.Transpose() * P * C;
            var W = C.Transpose() * P * l;


            // 求解x
            var x = B.Inverse() * W;
            Console.WriteLine(x.ToString());


            // 求未知点的真实值
            X += x;


            // 求观测数真实值(真实高差)
            var L = C * X;
            Console.WriteLine(L.ToString());


            // 精度评定
            var sigma0 = Math.Sqrt((l.Transpose() * P * l - W.Transpose() * x)[0, 0] / R);// 单位权中误差
            Console.WriteLine(sigma0);

            // 保存文件
            string split = "---------------------------------------------------------";
            string space = "                 ";
            const int pad = -10;
            const int titlePad = -8;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(split);
            sb.AppendLine(space + "未知点近似高程");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"点名",titlePad}{"高程(m)",titlePad}");
            for (int i = 0; i < T; i++) {
                sb.AppendLine($"{i + 1,pad}{UnknownPoint_new[i].Number,pad}{UnknownPoint_new[i].Height,pad:#0.0000}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "已知点高程");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"点名",titlePad}{"高程(m)",titlePad}");
            for (int i = 0; i < KnownPoints.Count; i++) {
                sb.AppendLine($"{i + 1,pad}{KnownPoints[i].Number,pad}{KnownPoints[i].Height,pad:#0.00000}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "高差观测数据");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"起点",titlePad}{"终点",titlePad}{"高差(m)",titlePad}{"距离(km)",titlePad}{"权重",titlePad}");
            for (int i = 0; i < N; i++) {
                sb.AppendLine($"{i + 1,pad}{ObservedDatas[i].Start,pad}{ObservedDatas[i].End,pad}{ObservedDatas[i].HeightDiff.ToString("#0.00000"),pad}{ObservedDatas[i].Distance.ToString("#0.0000"),pad}{P[i, i].ToString("#0.000"),pad}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "平差后高程");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"点名",titlePad}{"高程(m)",titlePad}{"修改值(mm)",titlePad}");
            sb.AppendLine(split);
            for (int i = 0; i < KnownPoints.Count; i++) {
                sb.AppendLine($"{i + 1,pad}{KnownPoints[i].Number,pad}{KnownPoints[i].Height,pad:#0.0000}   ");
            }
            for (int i = 0; i < T; i++) {
                sb.AppendLine($"{i + 3,pad}{UnknownPoint_new[i].Number,pad}{X[i, 0],pad:#0.0000}{x[i, 0] * 1000,pad:#0.00}");
            }
            sb.AppendLine(split);
            sb.AppendLine(space + "平差后观测数据");
            sb.AppendLine(split);
            sb.AppendLine($"{"序号",titlePad}{"起点",titlePad}{"终点",titlePad}{"高差(m)",titlePad}{"距离(km)",titlePad}{"修改值(mm)",titlePad}");
            sb.AppendLine(split);
            for (int i = 0; i < N; i++) {
                sb.AppendLine($"{i + 1,pad}{ObservedDatas[i].Start,pad}{ObservedDatas[i].End,pad}{ObservedDatas[i].HeightDiff,pad:#0.00}{ObservedDatas[i].Distance,pad:#0.0000}{L[i, 0] * 1000,pad:#0.00}");
            }
            sb.AppendLine(split);

            string outPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), System.IO.Path.GetFileNameWithoutExtension(Path) + "平差结果.OUP");
            FileHelper.WriteStrToTxt(sb.ToString(), outPath);
            FileView fileView = new FileView(outPath) {
                MdiParent = this,
            };
            MessageBox.Show("水准网平差完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fileView.Show();
        }

        private void ClosureErrorDropItem_Click(object sender, EventArgs e) {
            // 点的集合
            AllPoint = Commom.Clone(UnknownPoints_array);
            for (int i = 0; i < KnownPoints_array.Count; i++) {
                AllPoint.Add(KnownPoints_array[i]);
            }
            #region 根据观测数据生成邻接表
            var pointNum = AllPoint.Count;
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
                var rowIdx = AllPoint.IndexOf(pd.Start);
                var columnIdx = AllPoint.IndexOf(pd.End);
                cost[rowIdx, columnIdx] = pd.Distance;
                cost[columnIdx, rowIdx] = pd.Distance;
            });
            #endregion

            #region 搜索附和水准路线已知点最短路径，采用Dijkstra算法 
            // 参考 https://blog.csdn.net/weixin_42724039/article/details/81255726
            // https://www.codetd.com/article/9950057
            var path = new List<int>();
            if (KnownPoints_array.Count < 2) {
                Console.WriteLine("已知点个数小于2");
            }
            else {
                for (int ii = pointNum - 1; ii >= pointNum - KnownPoints_array.Count; ii--) {
                    for (int jj = ii - 1; jj >= pointNum - KnownPoints_array.Count; jj--) {
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
                        Console.WriteLine("{0}到{1}最短路径", AllPoint[g], AllPoint[h]);
                        foreach (int i in path) {
                            Console.Write(AllPoint[i] + " ");
                        }
                        Console.WriteLine();
                        Console.WriteLine(path.Count);
                    }
                }
            }
            #endregion

            #region 搜索所有闭合环
            Console.WriteLine("最小独立闭合环的个数：{0}-{1}-{2}", ObservedDatasNoRep.Count - (T + 2) + 1, ObservedDatasNoRep.Count, ObservedDatas.Count);


            // https://blog.csdn.net/beijinghorn/article/details/125057813
            var loops = Graph1.Drive(AllPoint, ObservedDatas);

            // https://blog.csdn.net/robin_xu_shuai/article/details/51898847
            for (int ii = pointNum - 1; ii >= pointNum - KnownPoints_array.Count; ii--) {
                VertexNode[] vertexNodes = new VertexNode[pointNum];
                for (int i = 0; i < pointNum; i++) {
                    vertexNodes[i] = new VertexNode(i, allPoint[i].ToString());
                }

                DFSAlgorithm graph = new DFSAlgorithm(vertexNodes);
                ObservedDatas.ForEach(p => {
                    var startIdx = allPoint.IndexOf(p.Start);
                    var endIdx = allPoint.IndexOf(p.End);
                    graph.insertEdge(startIdx, endIdx);
                    graph.insertEdge(endIdx, startIdx);
                });
                graph.findRing(ii);
                //   打印邻接表结构
                //for (int i = 0; i < graph.numVertex; i++) {
                //    VertexNode vertexNode = graph.vertexNodes[i];
                //    EdgeNode firstEdge = vertexNode.FirstEdge;

                //    EdgeNode currentEdge = firstEdge;
                //    Console.WriteLine(vertexNode.Data + ":");
                //    while (currentEdge != null) {
                //        int vertexNodeIndex = currentEdge.Adjvex;
                //        Console.Write("->" + vertexNodes[vertexNodeIndex].Data);
                //        currentEdge = currentEdge.Next;
                //    }
                //    Console.WriteLine("end");
                //}
            }
            #endregion
        }

        //  搜索最短路径
        void FindShortPath(int p, int exclude, int[] neighbor, double[] diff, double[] S) {
            for (int i = 0; i < AllPoint.Count; i++) {
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
                    int p1 = StarP[j];
                    int p2 = EndP[j];
                    double S12 = 1.0 / P[j];
                    if (neighbor[p1] < 0 && neighbor[p2] < 0) continue;

                    if (S[p2] > S[p1] + S12) {
                        neighbor[p2] = p1;
                        S[p2] = S[p1] + S12;
                        diff[p2] = diff[p1] + L[j];
                        successful = false;
                    }
                    else if (S[p1] > S[p2] + S12) {
                        neighbor[p1] = p2;
                        S[p1] = S[p2] + S12;
                        diff[p1] = diff[p2] - L[j];
                        successful = false;
                    }
                }
                if (successful) break;
            }
            return;
        }


    }
}

