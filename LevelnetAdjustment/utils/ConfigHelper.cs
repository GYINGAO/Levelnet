using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.utils {
    public class ConfigHelper {
        private static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        /// <summary>
        /// 判断键值为KeyName的项是否存在
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool existItem(string keyName) {
            //判断配置文件中是否存在键为keyName的项
            foreach (string key in ConfigurationManager.AppSettings) {
                if (key == keyName) {
                    //存在
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断value是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool existItem_2(string value) {
            //判断配置文件中是否存在键为keyName的项
            foreach (string key in ConfigurationManager.AppSettings) {
                if (config.AppSettings.Settings[key].Value == value) {
                    //存在
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定节点的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key) {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key)) {
                string value = config.AppSettings.Settings[key].Value;
                return value;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// 获取最后一个设置
        /// </summary>
        /// <returns></returns>
        public static string GetLastSetting() {
            return ConfigurationManager.AppSettings.AllKeys[ConfigurationManager.AppSettings.Count - 1];
        }

        /// <summary>
        /// 增加一个appsetting节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static string AddAppSetting(string value) {
            if (existItem_2(value)) {
                return "";
            }
            int count = ConfigurationManager.AppSettings.Count;
            string key = $"terfile{count + 1}";
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");//刷新命名节，这样在下次检索它时将从磁盘重新读取它。
            return key;
        }

        /// <summary>
        /// 更新appsetting节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void UpdateAppSettings(string key, string value) {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key)) {
                //如果当前节点存在，则更新当前节点
                config.AppSettings.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            else {
                Console.WriteLine("当前节点不存在");
            }
        }


        /// <summary>
        /// 删除appsetting节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void DeleteAppSettings(string key) {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key)) {
                //如果当前节点存在，则删除当前节点
                config.AppSettings.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            else {
                Console.WriteLine("当前节点不存在");
            }
        }

        /// <summary>
        /// 删除所有节点
        /// </summary>
        public static void DelAllSettings() {
            foreach (var key in ConfigurationManager.AppSettings.AllKeys) {
                config.AppSettings.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
            }
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
