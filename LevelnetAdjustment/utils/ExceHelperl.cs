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
using System.Collections;
using System.Threading;

namespace LevelnetAdjustment.utils {
    public class ExceHelperl {

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
        internal static void ExportHandbook(List<RawData> rds, List<ObservedData> ods, string path, List<InputFile> files) {


            IWorkbook workbook;
            string fileExt = Path.GetExtension(path).ToLower();
            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); }
            else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); }
            else { workbook = null; }
            if (workbook == null) { throw new Exception("无法创建excel"); }
            var cellNum = 10;

            #region 定义表格样式
            //样式
            ICellStyle style_title = workbook.CreateCellStyle();
            style_title.Alignment = HorizontalAlignment.CenterSelection;
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

            // 测段标题样式
            ICellStyle style2 = workbook.CreateCellStyle();
            style2.Alignment = HorizontalAlignment.CenterSelection;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderBottom = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderTop = BorderStyle.Thin;
            style2.BottomBorderColor = HSSFColor.Black.Index;
            style2.TopBorderColor = HSSFColor.Black.Index;
            style2.LeftBorderColor = HSSFColor.Black.Index;
            style2.RightBorderColor = HSSFColor.Black.Index;
            style2.WrapText = true;
            //字体
            IFont font2 = workbook.CreateFont();
            font2.FontName = "黑体";
            font2.FontHeightInPoints = 12;
            font2.IsBold = false;
            style2.SetFont(font2);

            //内容样式
            ICellStyle style3 = workbook.CreateCellStyle();
            style3.Alignment = HorizontalAlignment.Center;
            style3.VerticalAlignment = VerticalAlignment.Center;
            style3.BorderBottom = BorderStyle.Thin;
            style3.BorderLeft = BorderStyle.Thin;
            style3.BorderRight = BorderStyle.Thin;
            style3.BorderTop = BorderStyle.Thin;
            style3.BottomBorderColor = HSSFColor.Black.Index;
            style3.TopBorderColor = HSSFColor.Black.Index;
            style3.LeftBorderColor = HSSFColor.Black.Index;
            style3.RightBorderColor = HSSFColor.Black.Index;
            style3.WrapText = true;
            //字体
            IFont font3 = workbook.CreateFont();
            font3.FontName = "宋体";
            font3.FontHeightInPoints = 10;
            font3.IsBold = false;
            style3.SetFont(font3);

            //内容样式
            ICellStyle style4 = workbook.CreateCellStyle();
            style4.Alignment = HorizontalAlignment.Left;
            style4.VerticalAlignment = VerticalAlignment.Center;
            style4.WrapText = true;
            //字体
            IFont font4 = workbook.CreateFont();
            font4.FontName = "微软雅黑";
            font4.FontHeightInPoints = 11;
            font4.IsBold = false;
            style4.SetFont(font4);
            #endregion


            // 在WriteWorkbook 上添加名为 观测手簿 的数据表
            for (int i = 0; i < files.Count; i++) {
                var ext = Path.GetExtension(files[i].FileName).ToLower();
                if (ext.Contains("dat") || ext.Contains("gsi")) {
                    ISheet sheet_base = workbook.CreateSheet(Path.GetFileNameWithoutExtension(files[i].FileName));
                    //写入表格标题
                    IRow row_title = sheet_base.CreateRow(0);
                    sheet_base.AddMergedRegion(new CellRangeAddress(0, 0, 0, cellNum - 1));
                    row_title.CreateCell(0).SetCellValue("电子水准测量记录手簿");
                    row_title.GetCell(0).CellStyle = style_title;
                }
            }

            /* ISheet sheet = workbook.CreateSheet(Path.GetFileNameWithoutExtension(files[i].FileName));
             //写入表格标题
             IRow row_title = sheet.CreateRow(0);
             sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, cellNum - 1));
             row_title.CreateCell(0).SetCellValue("电子水准测量记录手簿");*/


            var sectionNum = 0; //当前测段数
            var stationNum = 0; //当前测站数
            double totalDisDiff = 0;//累计视距差
            double totalFrontDis = 0;//累计前距
            double totalBackDis = 0;//累计后距
            double totalDiff = 0;//累计高差
            double totalDis = 0;//累计距离
            string start = "", end = "";

            int sheetIdx = -1;
            ISheet sheet = null;
            for (int i = 0; i < rds.Count; i++) {
                if (rds[i].IsFileStart) {
                    sheetIdx++;
                    sheet = workbook.GetSheetAt(sheetIdx);
                }
                //添加标头
                if (rds[i].IsStart) {
                    start = rds[i].BackPoint;
                    var startRowIdx = sheet.LastRowNum + 1;
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 1, 2, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 1, 4, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 8, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx + 1, startRowIdx + 4, 9, 9));
                    sheet.AddMergedRegion(new CellRangeAddress(startRowIdx, startRowIdx, 0, cellNum - 1));

                    //表头第一行
                    var row0 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row0.CreateCell(k);
                        row0.GetCell(k).CellStyle = style2;
                    }
                    row0.GetCell(0).SetCellValue($"测段{sectionNum + 1}");
                    //表头第二行
                    var row1 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row1.CreateCell(k);
                        row1.GetCell(k).CellStyle = style3;
                    }
                    row1.GetCell(0).SetCellValue("测站");
                    row1.GetCell(1).SetCellValue("视准点");
                    row1.GetCell(2).SetCellValue("视距读数");
                    row1.GetCell(4).SetCellValue("标尺读数");
                    row1.GetCell(6).SetCellValue("读数差(mm)");
                    row1.GetCell(7).SetCellValue("高差(m)");
                    row1.GetCell(8).SetCellValue("距离(km)");
                    row1.GetCell(9).SetCellValue("高程(m)");
                    //表头第三行
                    var row2 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row2.CreateCell(k);
                        row2.GetCell(k).CellStyle = style3;
                    }
                    row2.GetCell(1).SetCellValue("后视");
                    row2.GetCell(2).SetCellValue("后距1");
                    row2.GetCell(3).SetCellValue("后距2");
                    row2.GetCell(4).SetCellValue("后尺读数1");
                    row2.GetCell(5).SetCellValue("后尺读数2");
                    //表头第四行
                    var row3 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row3.CreateCell(k);
                        row3.GetCell(k).CellStyle = style3;
                    }
                    row3.GetCell(1).SetCellValue("前视");
                    row3.GetCell(2).SetCellValue("前距1");
                    row3.GetCell(3).SetCellValue("前距2");
                    row3.GetCell(4).SetCellValue("前尺读数1");
                    row3.GetCell(5).SetCellValue("前尺读数2");
                    //表头第五行
                    var row4 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row4.CreateCell(k);
                        row4.GetCell(k).CellStyle = style3;
                    }
                    row4.GetCell(2).SetCellValue("视距差(m)");
                    row4.GetCell(3).SetCellValue("累积差(m)");
                    row4.GetCell(4).SetCellValue("高差(m)");
                    row4.GetCell(5).SetCellValue("高差(m)");

                    sectionNum++;
                }

                totalDisDiff += rds[i].DisDiffAve;
                totalFrontDis += (rds[i].FrontDis1 + rds[i].FrontDis2) / 2;
                totalBackDis += (rds[i].BackDis1 + rds[i].BackDis2) / 2;
                totalDiff += rds[i].DiffAve;
                totalDis += rds[i].DisAve;
                stationNum++;

                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum + 1, sheet.LastRowNum + 3, 0, 0));

                //测站数据第一行
                var row_1 = sheet.CreateRow(sheet.LastRowNum + 1);
                for (int k = 0; k < cellNum; k++) {
                    row_1.CreateCell(k);
                    row_1.GetCell(k).CellStyle = style3;
                }
                row_1.GetCell(0).SetCellValue(stationNum);
                row_1.GetCell(1).SetCellValue(rds[i].BackPoint);
                row_1.GetCell(2).SetCellValue(rds[i].BackDis1);
                row_1.GetCell(3).SetCellValue(rds[i].BackDis2);
                row_1.GetCell(4).SetCellValue(rds[i].BackDiff1);
                row_1.GetCell(5).SetCellValue(rds[i].BackDiff2);
                row_1.GetCell(6).SetCellValue(((rds[i].BackDiff1 - rds[i].BackDiff2) * 1000));

                //测站数据第二行
                var row_2 = sheet.CreateRow(sheet.LastRowNum + 1);
                for (int k = 0; k < cellNum; k++) {
                    row_2.CreateCell(k);
                    row_2.GetCell(k).CellStyle = style3;
                }
                row_2.GetCell(1).SetCellValue(rds[i].FrontPoint);
                row_2.GetCell(2).SetCellValue(rds[i].FrontDis1);
                row_2.GetCell(3).SetCellValue(rds[i].FrontDis2);
                row_2.GetCell(4).SetCellValue(rds[i].FrontDiff1);
                row_2.GetCell(5).SetCellValue(rds[i].FrontDiff2);
                row_2.GetCell(6).SetCellValue(((rds[i].FrontDiff1 - rds[i].FrontDiff2) * 1000));

                //测站数据第三行
                var row_3 = sheet.CreateRow(sheet.LastRowNum + 1);
                for (int k = 0; k < cellNum; k++) {
                    row_3.CreateCell(k);
                    row_3.GetCell(k).CellStyle = style3;
                }
                row_3.GetCell(2).SetCellValue(rds[i].DisDiffAve * 1000);
                row_3.GetCell(3).SetCellValue(totalDisDiff * 1000);
                row_3.GetCell(4).SetCellValue(rds[i].Diff1);
                row_3.GetCell(5).SetCellValue(rds[i].Diff2);
                row_3.GetCell(6).SetCellValue(((rds[i].Diff1 - rds[i].Diff2) * 1000));
                row_3.GetCell(7).SetCellValue(rds[i].DiffAve);
                row_3.GetCell(8).SetCellValue(rds[i].DisAve);


                //添加表尾
                if (rds[i].IsEnd) {
                    end = rds[i].FrontPoint;
                    //var startRowIdx = 1 + (sectionNum) * 9 + (i + 1) * 3 - 4;
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum + 1, sheet.LastRowNum + 4, 0, 0));

                    //表尾第一行
                    var row1 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row1.CreateCell(k);
                        row1.GetCell(k).CellStyle = style3;
                    }
                    row1.GetCell(0).SetCellValue("测段计算");
                    row1.GetCell(1).SetCellValue("测段起点");
                    row1.GetCell(2).SetCellValue(start);
                    //表尾第二行
                    var row2 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row2.CreateCell(k);
                        row2.GetCell(k).CellStyle = style3;
                    }
                    row2.GetCell(1).SetCellValue("测段终点");
                    row2.GetCell(2).SetCellValue(end);
                    row2.GetCell(4).SetCellValue("累计视距差");
                    row2.GetCell(5).SetCellValue(totalDisDiff * 1000);
                    row2.GetCell(6).SetCellValue("m");
                    //表尾第三行
                    var row3 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row3.CreateCell(k);
                        row3.GetCell(k).CellStyle = style3;
                    }
                    row3.GetCell(1).SetCellValue("累计前距");
                    row3.GetCell(2).SetCellValue(totalFrontDis);
                    row3.GetCell(3).SetCellValue("km");
                    row3.GetCell(4).SetCellValue("累计高差");
                    row3.GetCell(5).SetCellValue(totalDiff);
                    row3.GetCell(6).SetCellValue("m");
                    //表尾第四行
                    var row4 = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row4.CreateCell(k);
                        row4.GetCell(k).CellStyle = style3;
                    }
                    row4.GetCell(1).SetCellValue("累计后距");
                    row4.GetCell(2).SetCellValue(totalBackDis);
                    row4.GetCell(3).SetCellValue("km");
                    row4.GetCell(4).SetCellValue("测段距离");
                    row4.GetCell(5).SetCellValue(totalDis);
                    row4.GetCell(6).SetCellValue("km");

                    //添加一个空行
                    sheet.CreateRow(sheet.LastRowNum + 1);

                    totalDisDiff = 0;
                    totalFrontDis = 0;
                    totalBackDis = 0;
                    stationNum = 0;
                    totalDis = 0;
                }

                //添加相关信息
                if (rds[i].IsFileFinish) {
                    var row_foot = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < cellNum; k++) {
                        row_foot.CreateCell(k);
                        row_foot.GetCell(k).CellStyle = style4;
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 3, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 5, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 9));
                    row_foot.GetCell(0).SetCellValue("测量责任人：");
                    row_foot.GetCell(3).SetCellValue("复核：");
                    row_foot.GetCell(5).SetCellValue("监理：");
                    row_foot.GetCell(7).SetCellValue("观测日期：");
                }
            }

            //写入excel
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
                workbook.Write(fs);
            };
        }
    }
}
