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
        /// 起点索引
        /// </summary>
        public int StartIndex { get; set; } = -1;

        /// <summary>
        /// 测段终点
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// 终点索引
        /// </summary>
        public int EndIndex { get; set; } = -1;

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

    public class ObservedDataWF {
        public string Start { get; set; }//起点
        public string End { get; set; }//终点
        public double Distance_W { get; set; }//往测距离/km
        public double Distance_F { get; set; }//返测距离/km
        public double HeightDiff_W { get; set; }//往测高差/m
        public double HeightDiff_F { get; set; }//返测高差/m
        public double HeightDiff_Diff { get; set; }//较差/mm
    }
}
