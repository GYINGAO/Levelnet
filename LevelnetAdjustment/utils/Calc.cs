using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            y = y * (1.570796288 + y * (0.3706987906E-1
                + y * (-0.8364353589E-3 + y * (-0.2250947176E-3
                + y * (0.6841218299E-5 + y * (0.5824238515E-5
                + y * (-0.1045274970E-5 + y * (0.8360937017E-7
                + y * (-0.3231081277E-8 + y * (0.3657763036E-10
                + y * 0.6936233982E-12))))))))));
            return Math.Sqrt(y);
        }
    }
}
