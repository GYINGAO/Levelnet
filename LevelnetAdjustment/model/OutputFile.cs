﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class OutputFile {
        public string OutpathAdj { get; set; }//约束网平差
        public string OutpathAdjFree { get; set; }//拟稳平差
        public string OutpathClosure { get; set; }//闭合差
        public string OutpathGrossError { get; set; }//粗差
        public string COSADis { get; set; }//按距离定权
        public string COSASta { get; set; }//按测站数定权
        public string Handbook { get; set; }//观测手簿
    }
}