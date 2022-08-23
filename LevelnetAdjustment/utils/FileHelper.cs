using LevelnetAdjustment.model;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LevelnetAdjustment.utils {
    public class FileHelper {
        /// <summary>
        /// 读取原始观测文件
        /// </summary>
        /// <param name="knownPoints"></param>
        /// <param name="observedDatas"></param>
        /// <param name="fileName"></param>
        internal static int ReadOriginalFile(List<PointData> knownPoints, List<ObservedData> observedDatas, List<ObservedData> observedDatasNoRep, string fileName) {
            try {
                int level = 2;
                using (StreamReader sr = new StreamReader(fileName)) {
                    string line;
                    string[] dataArray;
                    while (sr.Peek() > -1) {
                        line = sr.ReadLine().Trim();
                        dataArray = Regex.Split(line, "(?:\\s*[,|，]\\s*)", RegexOptions.IgnoreCase); // 正则匹配逗号（允许空格）
                        if (dataArray.Length == 1) {
                            level = int.Parse(dataArray[0]);
                        }
                        else if (dataArray.Length == 2) {
                            PointData knownPoint = new PointData {
                                Number = dataArray[0],
                                Height = double.Parse(dataArray[1])
                            };
                            knownPoints.Add(knownPoint);
                        }
                        else {
                            if (observedDatas.Exists(p => p.End == dataArray[0] && p.Start == dataArray[1]) && observedDatas.Exists(p => p.Start == dataArray[0] && p.End == dataArray[1])) {
                                continue;
                            }
                            ObservedData observedData = new ObservedData {
                                Start = dataArray[0],
                                End = dataArray[1],
                                HeightDiff = double.Parse(dataArray[2]),
                                Distance = double.Parse(dataArray[3])
                            };
                            observedDatas.Add(observedData);
                            if (observedDatasNoRep.Exists(p => p.End == dataArray[0] && p.Start == dataArray[1]) || observedDatasNoRep.Exists(p => p.Start == dataArray[0] && p.End == dataArray[1])) {
                                continue;
                            }
                            //过滤重复的测段(往返测)
                            observedDatasNoRep.Add(observedData);
                        }
                    }
                }
                return level;
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
        internal static void ReadDAT(string filename, List<RawData> dats, List<ObservedData> ods) {
            dats.Add(new RawData());
            using (StreamReader sr = new StreamReader(filename)) {
                int stationIdx = 0;//测站索引,记录一个测站观测索引

                // string method;//观测方式 aBFFB aFBBF BFBF BBFF FBBF
                while (sr.Peek() > -1) {
                    string line = sr.ReadLine().Trim();
                    //跳过空行
                    if (string.IsNullOrEmpty(line)) {
                        continue;
                    }
                    //测段开始
                    if (line.Contains("Start-Line") || line.Contains("Start")) {
                        ods.Add(new ObservedData());
                        continue;
                    }
                    //测段结束
                    if (line.Contains("End-Line") || line.Contains("End")) {
                        continue;
                    }
                    // 不要的测量数据
                    if (line.Contains("#####")) {
                        continue;
                    }
                    //整个测站重新测量
                    if (line.Contains("Station repeated")) {
                        dats.RemoveAt(dats.Count - 1);//移除最后一站
                        dats.Add(new RawData());
                        continue;
                    }
                    //单次重新测量
                    if (line.Contains("Measurement repeated")) {
                        continue;
                    }
                    string[] arr = Regex.Split(line, "(?:\\s*[|]\\s*)", RegexOptions.IgnoreCase);
                    var str3 = arr[3].Trim();
                    var str2 = arr[2].Trim();
                    // 读取测量数据
                    if (str3.Contains("Rb") || str3.Contains("Rf")) {
                        stationIdx++;
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
                        if (stationIdx <= 2) {
                            if (arr3[0] == "Rb") {
                                dats[dats.Count - 1].BackDiff1 = Convert.ToDouble(arr3[1]) * c1;
                                dats[dats.Count - 1].BackDis1 = Convert.ToDouble(arr4[1]) * c2;
                                dats[dats.Count - 1].BackPoint = arr2[1];
                            }
                            else if (arr3[0] == "Rf") {
                                dats[dats.Count - 1].FrontDiff1 = Convert.ToDouble(arr3[1]) * c1;
                                dats[dats.Count - 1].FrontDis1 = Convert.ToDouble(arr4[1]) * c2;
                                dats[dats.Count - 1].FrontPoint = arr2[1];
                            }
                        }
                        else {
                            if (arr3[0] == "Rb") {
                                dats[dats.Count - 1].BackDiff2 = Convert.ToDouble(arr3[1]) * c1;
                                dats[dats.Count - 1].BackDis2 = Convert.ToDouble(arr4[1]) * c2;
                            }
                            else if (arr3[0] == "Rf") {
                                dats[dats.Count - 1].FrontDiff2 = Convert.ToDouble(arr3[1]) * c1;
                                dats[dats.Count - 1].FrontDis2 = Convert.ToDouble(arr4[1]) * c2;
                            }
                        }


                    }
                    // 一个测站最后一个数据读取完
                    if (stationIdx == 4) {
                        dats[dats.Count - 1].Calc();
                        stationIdx = 0;
                        dats.Add(new RawData());
                    }
                    // 包含测段数的一行，计算测段数据
                    if (str3.Contains("Db") || str3.Contains("Df")) {
                        dats[dats.Count - 2].IsEnd = true;
                        var arr2 = Regex.Split(arr[2].Trim(), "[\\s]+", RegexOptions.IgnoreCase);
                        int stationNum = int.Parse(arr2[2]);//测段测站数
                        ods[ods.Count - 1].StationNum = stationNum;
                        dats[dats.Count - stationNum - 1].IsStart = true;
                        //ods[ods.Count - 1].End = arr2[1];//测段终点
                        ods[ods.Count - 1].Start = dats[dats.Count - stationNum - 1].BackPoint;
                        ods[ods.Count - 1].End = dats[dats.Count - 2].FrontPoint;
                        //计算测段数据
                        double totalDis = 0;
                        double totalDiff = 0;
                        for (int i = dats.Count - stationNum; i < dats.Count; i++) {
                            totalDiff += dats[i].DiffAve;
                            totalDis += dats[i].DisAve;
                        }
                        ods[ods.Count - 1].HeightDiff = totalDiff;
                        ods[ods.Count - 1].Distance = totalDis;
                    }
                }
                dats.RemoveAt(dats.Count - 1);
            }

        }

        /// <summary>
        /// 读取GSI
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rawDatas"></param>
        /// <param name="observedDatas"></param>
        internal static void ReadGSI(string filename, List<RawData> rawDatas, List<ObservedData> observedDatas) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写入txt
        /// </summary>
        /// <param name="str"></param>
        /// <param name="file"></param>
        internal static void WriteStrToTxt(string str, string file) {
            FileStream fileStream = new FileStream(file, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            streamWriter.Write(str + "\r\n");
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        internal static void ExportCOSA(List<ObservedData> ods, string path) {
            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            ods.ForEach(l => {
                streamWriter.WriteLine($"{l.Start},{l.End},{l.HeightDiff},{l.Distance}");
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
        internal static void ReadGSI(string fileName, List<PointData> knownPoints) {
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
    }
}
