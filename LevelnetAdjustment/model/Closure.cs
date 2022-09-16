using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class Closure {
        public double Length { get; set; } //路线长度
        public double Error { get; set; } //闭合差
        public double Limit { get; set; } //限差
        public string Line { get; set; } //线路点号
    }
}
