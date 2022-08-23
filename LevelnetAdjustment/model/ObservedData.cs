using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class ObservedData {
        /// <summary>
        /// 测段起点
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// 测段终点
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// 高差
        /// </summary>
        public double HeightDiff { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// 测站数
        /// </summary>
        public int StationNum { get; set; }
    }
}
