using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    [Serializable]
    public class PointData {
        /// <summary>
        /// 已知点点号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 已知点高程
        /// </summary>
        public double Height { get; set; } = 0;
        /// <summary>
        /// 是否为拟稳点
        /// </summary>
        public bool IsStable { get; set; } = false;
    }
}
