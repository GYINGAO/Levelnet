using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LevelnetAdjustment.utils {
    public class RegistryHelper {
        /// <summary>
        /// 检查一下文件类型是否已被注册.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool FileTypeRegistered(string extension) {
            RegistryKey sluKey = Registry.ClassesRoot.OpenSubKey(extension);
            if (sluKey != null)
                return true;
            return false;
        }
        /// <summary>
        /// 删除已被注册的键值
        /// </summary>
        /// <param name="extension"></param>
        public static void UnRegistFileType(string extension) {
            if (FileTypeRegistered(extension)) {
                Registry.ClassesRoot.DeleteSubKey(extension);
                string relationName = extension.Substring(1, extension.Length - 1).ToUpper() + "  FileType ";
                Registry.ClassesRoot.DeleteSubKeyTree(relationName);
                Registry.ClassesRoot.Close();
            }
        }
        /// <summary>
        /// 注册自定义文件,并与自己的应用程序相关联
        /// </summary>
        /// <param name="extension"></param>
        public static void RegistFileType(string extension) {
            UnRegistFileType(extension);
            string relationName = extension.Substring(1, extension.Length - 1).ToUpper() + "  FileType ";
            RegistryKey sluKey = Registry.ClassesRoot.CreateSubKey(extension);
            sluKey.SetValue("", relationName);
            sluKey.Close();

            RegistryKey relationKey = Registry.ClassesRoot.CreateSubKey(relationName);
            relationKey.SetValue("", "Your Description ");

            RegistryKey iconKey = relationKey.CreateSubKey(" DefaultIcon ");//图标
            iconKey.SetValue("", System.Windows.Forms.Application.StartupPath + @" \mainICO.ico ");

            RegistryKey shellKey = relationKey.CreateSubKey(" Shell ");
            RegistryKey openKey = shellKey.CreateSubKey(" Open ");
            RegistryKey commandKey = openKey.CreateSubKey(" Command ");
            commandKey.SetValue("", System.Windows.Forms.Application.ExecutablePath + "  %1 ");//是数字"1",双击文件之后就把文件的路径传递过来了.
            relationKey.Close();
        }
    }
}
