using LevelnetAdjustment.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.utils {
    /// <summary>
    /// 读取JSON文件
    /// </summary>
    /// <param name="key">JSON文件中的key值</param>
    /// <returns>JSON文件中的value值</returns>
    public class JsonHelper {
        public static ProjectInfo ReadJson(string filePath) {
            using (StreamReader file = File.OpenText(filePath)) {
                string v = file.ReadToEnd();
                return JsonConvert.DeserializeObject<ProjectInfo>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        public static void WriteJson(ProjectInfo project) {
            string json = JsonConvert.SerializeObject(project, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            FileHelper.WriteStrToTxt(json, Path.Combine(project.Path, project.Name, project.Name + ".laproj"));
        }
    }
}
