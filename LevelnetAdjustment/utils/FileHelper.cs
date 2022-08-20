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
        public static int readOriginalFile(List<PointData> knownPoints, List<ObservedData> observedDatas, List<ObservedData> observedDatasNoRep, string fileName) {
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
        /// 写入txt
        /// </summary>
        /// <param name="str"></param>
        /// <param name="file"></param>
        public static void WriteStrToTxt(string str, string file) {
            // 如果存在要保存的文件,则删除
            if (File.Exists(file)) {
                File.Delete(file);
            }
            FileStream fileStream = new FileStream(file, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            streamWriter.Write(str + "\r\n");
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        public static void matrix2Txt(double[,] matrix) {
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
    }
}
