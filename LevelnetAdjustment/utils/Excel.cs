using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; //XSSF是用于.xlsx（2007以后版本）
using NPOI.HSSF.UserModel; //HSSF是用于.xls（2007以前版本）
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelnetAdjustment.model;
using System.IO;
using NPOI.HSSF.Util;

namespace LevelnetAdjustment.utils {
    public class Excel {
        static readonly string Handbooktemplate = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "handbooktemplate.xlsx");

        /// <summary>
        /// 设置单元格数据类型
        /// </summary>
        /// <param name="cell">目标单元格</param>
        /// <param name="obj">数据值</param>
        /// <returns></returns>
        public static void SetCellValue(ICell cell, object obj) {
            if (obj.GetType() == typeof(int)) {
                cell.SetCellValue((int)obj);
            }
            else if (obj.GetType() == typeof(double)) {
                cell.SetCellValue((double)obj);
            }
            else if (obj.GetType() == typeof(IRichTextString)) {
                cell.SetCellValue((IRichTextString)obj);
            }
            else if (obj.GetType() == typeof(string)) {
                cell.SetCellValue(obj.ToString());
            }
            else if (obj.GetType() == typeof(DateTime)) {
                cell.SetCellValue((DateTime)obj);
            }
            else if (obj.GetType() == typeof(bool)) {
                cell.SetCellValue((bool)obj);
            }
            else {
                cell.SetCellValue(obj.ToString());
            }
        }

        /// <summary>
        /// 导出观测手簿
        /// </summary>
        /// <param name="rds"></param>
        /// <param name="ods"></param>
        internal static void ExportHandbook(List<RawData> rds, List<ObservedData> ods, string path) {
            IWorkbook workbook;
            string fileExt = Path.GetExtension(Handbooktemplate).ToLower();
            using (FileStream fs = new FileStream(Handbooktemplate, FileMode.Open, FileAccess.Read)) {
                if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); }
                else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); }
                else { workbook = null; }
                if (workbook == null) { return; }
                ISheet sheet = workbook.GetSheetAt(0);
                int firstRowNum = sheet.FirstRowNum;


                for (int i = firstRowNum + 1; i <= firstRowNum + 5; i++) {

                }

            };
        }
    }
}
