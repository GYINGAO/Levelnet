using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class FileOption {
        public string FileName { get; set; } //文件名
        public bool IsSplit { get; set; } = true; //是否按测段分割
    }
}
