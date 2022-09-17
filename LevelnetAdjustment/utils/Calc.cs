using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelnetAdjustment.model;

namespace LevelnetAdjustment.utils {
    public class Calc {
        /// <summary>
        /// 正态分布的反函数：p(-∞,u)=p 已知p，返回u
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double re_norm(double p) {
            if (p == 0.5) return 0.0;
            if (p > 0.9999997) return 5.0;
            if (p < 0.0000003) return -5.0;
            if (p < 0.5) return -re_norm(1.0 - p);

            double y = -Math.Log10(4.0 * p * (1.0 - p));
            y *= (1.570796288 + y * (0.3706987906E-1
                + y * (-0.8364353589E-3 + y * (-0.2250947176E-3
                + y * (0.6841218299E-5 + y * (0.5824238515E-5
                + y * (-0.1045274970E-5 + y * (0.8360937017E-7
                + y * (-0.3231081277E-8 + y * (0.3657763036E-10
                + y * 0.6936233982E-12))))))))));
            return Math.Sqrt(y);
        }

        /// <summary>
        /// 标准正态分布
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static double norm(double u) {
            if (u < -5.0) return 0.0;
            if (u > 5.0) return 1.0;
            double y = Math.Abs(u) / Math.Sqrt(2);
            double p = 1.0 + y * (0.0705230784 + y * (0.0422820123 + y * (0.0092705272 + y * (0.0001520143 + y * (0.0002765672 + y * 0.0000430638)))));
            double er = 1 - Math.Pow(p, -16);
            return u < 0 ? 0.5 - 0.5 * er : 0.5 + 0.5 * er;
        }

        /// <summary>
        ///  观测数据去除重复边
        /// </summary>
        /// <param name="ods"></param>
        /// <returns></returns>
        public static List<ObservedData> RemoveDuplicates(List<ObservedData> ods) {
            List<ObservedData> ods_new = new List<ObservedData>();
            ods.ForEach(l => {
                if (!ods_new.Exists(p => (p.Start == l.Start && p.End == l.End) || (p.Start == l.End && p.End == l.Start))) {
                    ods_new.Add(l);
                }
            });
            return ods_new;
        }

        /// <summary>
        /// 将原始观测数据转换为高差观测数据
        /// </summary>
        /// <param name="rds"></param>
        /// <returns></returns>
        public static List<ObservedData> Rd2Od(List<RawData> rds, string zd) {
            List<ObservedData> ods = new List<ObservedData>();
            double totalDis = 0;//距离
            double totalDiff = 0;//高差
            int stationNum = 0;//测站数
            foreach (var rd in rds) {
                stationNum++;
                if (stationNum == 1) {
                    ods.Add(new ObservedData() { Start = rd.BackPoint });
                }
                //有转点
                totalDiff += rd.DiffAve;
                totalDis += rd.DisAve;
                if (IsZD(zd, rd.FrontPoint)) {
                    continue;
                }
                else {
                    ods[ods.Count - 1].StationNum = stationNum;
                    ods[ods.Count - 1].Distance = totalDis;
                    ods[ods.Count - 1].HeightDiff = totalDiff;
                    ods[ods.Count - 1].End = rd.FrontPoint;
                    stationNum = 0;
                    totalDis = 0;
                    totalDiff = 0;
                }
            }
            return ods;
        }

        static bool IsZD(string zd, string number) {
            return number.ToLower().StartsWith(zd.ToLower());
        }
    }
}
