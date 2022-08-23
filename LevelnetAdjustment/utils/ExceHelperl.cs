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
using NPOI.SS.Util;

namespace LevelnetAdjustment.utils {
    public class ExceHelperl {
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
            string fileExt = Path.GetExtension(path).ToLower();
            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); }
            else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); }
            else { workbook = null; }
            if (workbook == null) { throw new Exception("无法创建excel"); }
            var cellNum = 10;
            // 在WriteWorkbook 上添加名为 观测手簿 的数据表
            ISheet sheet = workbook.CreateSheet("观测手簿 ");

            //写入表格标题
            IRow row_title = sheet.CreateRow(0);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, cellNum - 1));
            ICell cell_title = row_title.CreateCell(0);
            cell_title.SetCellValue("电子水准测量记录手簿");
            //样式
            ICellStyle style_title = workbook.CreateCellStyle();
            style_title.Alignment = HorizontalAlignment.Center;
            style_title.VerticalAlignment = VerticalAlignment.Center;
            style_title.BorderBottom = BorderStyle.Thin;
            style_title.BorderLeft = BorderStyle.Thin;
            style_title.BorderRight = BorderStyle.Thin;
            style_title.BorderTop = BorderStyle.Thin;
            style_title.BottomBorderColor = HSSFColor.Black.Index;
            style_title.TopBorderColor = HSSFColor.Black.Index;
            style_title.LeftBorderColor = HSSFColor.Black.Index;
            style_title.RightBorderColor = HSSFColor.Black.Index;
            //字体
            IFont font_title = workbook.CreateFont();
            font_title.FontName = "宋体";
            font_title.FontHeightInPoints = 16;
            font_title.IsBold = true;
            style_title.SetFont(font_title);
            cell_title.CellStyle = style_title;

            ICellStyle style_con = workbook.CreateCellStyle();
            style_con.Alignment = HorizontalAlignment.Center;
            style_con.VerticalAlignment = VerticalAlignment.Center;
            style_con.BorderBottom = BorderStyle.Thin;
            style_con.BorderLeft = BorderStyle.Thin;
            style_con.BorderRight = BorderStyle.Thin;
            style_con.BorderTop = BorderStyle.Thin;
            style_con.BottomBorderColor = HSSFColor.Black.Index;
            style_con.TopBorderColor = HSSFColor.Black.Index;
            style_con.LeftBorderColor = HSSFColor.Black.Index;
            style_con.RightBorderColor = HSSFColor.Black.Index;
            style_con.WrapText = true;
            //字体
            IFont font_con = workbook.CreateFont();
            font_con.FontName = "宋体";
            font_con.FontHeightInPoints = 10;
            font_con.IsBold = false;
            style_con.SetFont(font_title);

            var sectionNum = 0;


            for (int i = 0; i < rds.Count; i++) {
                //添加标头
                if (rds[i].IsStart) {
                    var startRowIdx = 9 * (sectionNum) + i * 3 + 1;
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx, startRowIdx, 0, cellNum - 1));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 1, 2, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 1, 4, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 8, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 9, 9));

                    sheet.CreateRow(startRowIdx).CreateCell(0).SetCellValue($"测段{sectionNum + 1}");
                    var row1 = sheet.CreateRow(startRowIdx + 1);
                    row1.CreateCell(0).SetCellValue("测站");
                    row1.CreateCell(1).SetCellValue("视准点");
                    row1.CreateCell(2).SetCellValue("视距读数");
                    row1.CreateCell(4).SetCellValue("标尺读数");
                    row1.CreateCell(6).SetCellValue("读数差(mm)");
                    row1.CreateCell(7).SetCellValue("高差(m)");
                    row1.CreateCell(8).SetCellValue("距离(m)");
                    row1.CreateCell(9).SetCellValue("高程(m)");
                    var row2 = sheet.CreateRow(startRowIdx + 2);
                    row2.CreateCell(1).SetCellValue("后视");
                    row2.CreateCell(2).SetCellValue("后距1");
                    row2.CreateCell(3).SetCellValue("后距2");
                    row2.CreateCell(4).SetCellValue("后尺读数1");
                    row2.CreateCell(5).SetCellValue("后尺读数2");
                    var row3 = sheet.CreateRow(startRowIdx + 3);
                    row3.CreateCell(1).SetCellValue("前视");
                    row3.CreateCell(2).SetCellValue("前距1");
                    row3.CreateCell(3).SetCellValue("前距2");
                    row3.CreateCell(4).SetCellValue("前尺读数1");
                    row3.CreateCell(5).SetCellValue("前尺读数2");
                    var row4 = sheet.CreateRow(startRowIdx + 4);
                    row4.CreateCell(2).SetCellValue("视距差(m)");
                    row4.CreateCell(3).SetCellValue("累积差(m)");
                    row4.CreateCell(4).SetCellValue("高差(m)");
                    row4.CreateCell(5).SetCellValue("高差(m)");
                    sectionNum++;
                }

                var row_1 = sheet.CreateRow(sheet.LastRowNum + 1);
                row_1.CreateCell(0).SetCellValue(i + 1);
                row_1.CreateCell(1).SetCellValue(rds[i].BackPoint);
                row_1.CreateCell(2).SetCellValue(rds[i].BackDis1);
                row_1.CreateCell(3).SetCellValue(rds[i].BackDis2);
                row_1.CreateCell(4).SetCellValue(rds[i].BackDiff1);
                row_1.CreateCell(5).SetCellValue(rds[i].BackDiff2);
                row_1.CreateCell(6).SetCellValue(((rds[i].BackDiff1 - rds[i].BackDiff2) * 1000));

                var row_2 = sheet.CreateRow(sheet.LastRowNum + 1);
                row_2.CreateCell(1).SetCellValue(rds[i].BackPoint);
                row_2.CreateCell(2).SetCellValue(rds[i].FrontDis1);
                row_2.CreateCell(3).SetCellValue(rds[i].FrontDis2);
                row_2.CreateCell(4).SetCellValue(rds[i].FrontDiff1);
                row_2.CreateCell(5).SetCellValue(rds[i].FrontDiff2);
                row_2.CreateCell(6).SetCellValue(((rds[i].FrontDiff1 - rds[i].FrontDiff2) * 1000));

                var row_3 = sheet.CreateRow(sheet.LastRowNum + 1);
                row_3.CreateCell(2).SetCellValue(((rds[i].DisDiff1 + rds[i].DisDiff2) * 500));
                row_3.CreateCell(4).SetCellValue(rds[i].Diff1);
                row_3.CreateCell(5).SetCellValue(rds[i].Diff2);
                row_3.CreateCell(6).SetCellValue(((rds[i].Diff1 - rds[i].Diff2) * 1000));
                row_3.CreateCell(7).SetCellValue(rds[i].DiffAve);

                //添加表
                if (rds[i].IsEnd) {
                    var startRowIdx = 1 + (sectionNum) * 9 + (i + 1) * 3 - 4;
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx, startRowIdx + 3, 0, 0));
                    var row1 = sheet.CreateRow(startRowIdx);
                    row1.CreateCell(0).SetCellValue("测段计算");
                    row1.CreateCell(1).SetCellValue("测段起点");
                    row1.CreateCell(2).SetCellValue(ods[sectionNum - 1].Start);
                    var row2 = sheet.CreateRow(startRowIdx + 1);
                    row2.CreateCell(1).SetCellValue("测段终点");
                    row2.CreateCell(2).SetCellValue(ods[sectionNum - 1].End);
                    row2.CreateCell(4).SetCellValue("累计视距差");
                    row2.CreateCell(6).SetCellValue("m");
                    var row3 = sheet.CreateRow(startRowIdx + 2);
                    row3.CreateCell(1).SetCellValue("累计前距");
                    row3.CreateCell(3).SetCellValue("km");
                    row3.CreateCell(4).SetCellValue("累计高差");
                    row3.CreateCell(5).SetCellValue(ods[sectionNum - 1].HeightDiff);
                    row3.CreateCell(6).SetCellValue("m");
                    var row4 = sheet.CreateRow(startRowIdx + 3);
                    row4.CreateCell(1).SetCellValue("累计后距");
                    row4.CreateCell(3).SetCellValue("km");
                    row4.CreateCell(4).SetCellValue("测段距离");
                    row4.CreateCell(5).SetCellValue(ods[sectionNum - 1].Distance);
                    row4.CreateCell(6).SetCellValue("km");
                }
            }

            //写入excel
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
                workbook.Write(fs);
            };
        }

    }
}
