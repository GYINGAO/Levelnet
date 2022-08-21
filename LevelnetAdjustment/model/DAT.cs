using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class DAT {
        public string BackPoint { get; set; } //测站后视点
        public string FrontPoint { get; set; } //测站前视点

        public double BackDiff1 { get; set; } //第一个后视读数
        public double BackDiff2 { get; set; } //第一个前视读数
        public double FrontDiff1 { get; set; } //第一个前视读数
        public double FrontDiff2 { get; set; } //第二个前视读数
        public double Diff1 { get; set; } //第一个高差
        public double Diff2 { get; set; } //第二个高差
        public double DiffAve { get; set; } //高差平均值

        public double BackDis1 { get; set; } //第一个后距读数
        public double BackDis2 { get; set; } //第一个前距读数
        public double FrontDis1 { get; set; } //第一个前距读数
        public double FrontDis2 { get; set; } //第二个前距读数
        public double DisDiff1 { get; set; } //第一个视距差
        public double DisDiff2 { get; set; } //第二个视距差
        public double DisDiffAve { get; set; } //视距差平均值
        public double Dis1 { get; set; } //第一个观测距离
        public double Dis2 { get; set; } //第二个观测距离
        public double DisAve { get; set; } //观测距离平均值

        /// <summary>
        /// 计算高差、视距、观测距离
        /// </summary>
        /// <param name="diff1"></param>
        /// <param name="diff2"></param>
        private void Calc() {
            Diff1 = BackDiff1 - FrontDiff1;
            Diff2 = BackDiff2 - FrontDiff2;
            DiffAve = (Diff1 + Diff2) / 2;

            DisDiff1 = BackDis1 - FrontDis1;
            DisDiff2 = BackDis2 - FrontDis2;
            DisDiffAve = (DisDiff1 + DisDiff2) / 2;

            Dis1 = BackDis1 + FrontDis1;
            Dis2 = BackDis2 + FrontDis2;
            DisAve = (Dis1 + Dis2) / 2;
        }
    }
}
