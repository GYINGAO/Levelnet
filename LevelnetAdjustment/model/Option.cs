using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.model {
    public class Option {
        public double Sigma { get; set; } = 0.001; //验前单位权中误差
        public double Alpha { get; set; } = 0.001; //粗差检验的显著水平
        public double AlphaLimit { get; set; } = 3.2905;//标准化残差限差
        public int PowerMethod { get; set; } = 0; //定权方式 0按距离定权 1按测段数定权
        public double Limit { get; set; } = 0.01; //平差迭代限差 m
        public int AdjustMethod { get; set; } = 0; //平差方法 0约束网平差 1拟稳平差
        public int UnitRight { get; set; } = 0; //单位权选择 0先验1后验
        public List<InputFile> ImportFiles { get; set; } = new List<InputFile>(); //文件列表
        public OutputFile OutputFiles { get; set; } = new OutputFile();// 文件输出路径
        public LevelParam LevelParams { get; set; } = new LevelParam();//高程网观测等级及限差系数
        public ObsDataLimit ObsDataLimits { get; set; } = new ObsDataLimit();//观测数据限差
        public ManualModify ManualModification { get; set; } = new ManualModify();//手动修改点
        public List<GrossError> GrossErrors { get; set; } = new List<GrossError>();//粗差数据
        public Option() {

        }
    }

    /// <summary>
    /// 高程网观测等级及限差系数
    /// </summary>
    public class LevelParam {
        enum Level {
            one = 0,
            two,
            three,
            four,
            precise,
            other
        }
        public int LevelingGrade { get; set; } = (int)Level.one;  // 水准等级
        public double WangFan { get; set; } = 4; //往返测限差
        public double Huan { get; set; } = 4; //环闭合差
        public double FuHe { get; set; } = 4; //附和路线闭合差
        public bool IsCP3 { get; set; } = false; //是否为cp3高程网
        public double CP3WangFan { get; set; } = 1; //往返测限差
        public double CP3Huan { get; set; } = 1; //往返测限差
    }

    /// <summary>
    /// 观测数据限差
    /// </summary>
    public class ObsDataLimit {
        public double FBDis { get; set; } = 2.0;//前后视距差/m
        public double FBDisSum { get; set; } = 6.0;//前后累计视距差/m
        public double StafLow { get; set; } = 0.45;//视线高度/m
    }


    public class ChangedPoint {
        public string Value { get; set; }
        public string ControlName { get; set; }
    }

    /// <summary>
    /// 手动修改点的数据
    /// </summary>
    public class ManualModify {
        public List<ArrayList> ChangedStationLine { get; set; } = new List<ArrayList>();//修改后的点名
        public List<ChangedPoint> ChangedPoints { get; set; } = new List<ChangedPoint>();//修改过的点集合
    }

    /// <summary>
    /// 粗差
    /// </summary>
    public class GrossError {
        public ObservedData ObservedDatas { get; set; } = new ObservedData();//观测数据
        public int Index { get; set; } = -1;//观测数据索引
        public double Correction { get; set; } = -1.0;//改正数
        public double StandardizedResidual { get; set; } = -1.0;//标准化残差
    }
}
