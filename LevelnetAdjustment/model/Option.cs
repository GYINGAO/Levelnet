using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class Option {
        public double Sigma { get; set; } = 0.001; //验前单位权中误差
        public double Alpha { get; set; } = 0.2; //粗差检验的显著水平
        public int PowerMethod { get; set; } = 0; //定权方式 0按距离定权 1按测段数定权
        public double Limit { get; set; } = 0.01; //平差迭代限差 m
        public int AdjustMethod { get; set; } = 0; //平差方法 0约束网平差 1拟稳平差
        // 水准等级
        private int level;
        public int Level {
            get => level;
            set {
                level = value;
                Coefficient = value;
            }
        }
        // 闭合差限差系数
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

        public bool IsSplit { get; set; } = true; //导入原始数据时是否分割测段,默认分割

        public Option() {
            this.Level = 2;
        }
    }
}
