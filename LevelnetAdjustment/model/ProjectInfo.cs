using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class ProjectInfo {
        public string Path { get; set; }//项目路径
        public string Name { get; set; }//项目名
        public string Company { get; set; }//单位
        public DateTime MeasurementTime { get; set; }//测量时间
        public string MeasurementPerson { get; set; }//测量人员
        public DateTime ProcessingTime { get; set; }//处理时间
        public string ProcessingPerson { get; set; }//处理人员
        public List<RawData> RawDatas { get; set; }//原始数据
        public List<ObservedData> ObservedDatas { get; set; }//测段观测文件
        public List<PointData> KnPointDatas { get; set; }//已知点数据
        public Option Options { get; set; }//选项
    }
}
