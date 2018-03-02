using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;

namespace Common
{
    public static class ExcelUtil
    {
         /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="tuples"></param>
        /// <param name="type">type为1表示最后一列要合并</param>
        /// <returns></returns>
        public static byte[] CreateExcel<T>(this List<T> list, List<Tuple<string, string>> tuples, int type = -1)
        {
            //Create new Excel Workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel Sheet
            var sheet = workbook.CreateSheet();

            //Create a header row
            var headerRow = sheet.CreateRow(0);
            for (int i = 0; i < tuples.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(tuples[i].Item1);
            }

            //(Optional) freeze the header row so it is not scrolled
            sheet.CreateFreezePane(0, 1, 0, 1);

            int rowNumber = 1;

            //Populate the sheet with values from the grid data

            foreach (var objArticles in list)
            {
                //Create a new Row
                var row = sheet.CreateRow(rowNumber++);

                //Set the Values for Cells
                for (int i = 0; i < tuples.Count; i++)
                {
                    if (objArticles.GetType().GetProperty(tuples[i].Item2) == null)
                    {
                        row.CreateCell(i).SetCellValue("");
                    }
                    else
                    {
                        row.CreateCell(i).SetCellValue((objArticles.GetType().GetProperty(tuples[i].Item2).GetValue(objArticles, null) ?? string.Empty).ToString());
                    }
                }
            }

            //type==1把最后一行合计前3列合并,并赋值
            if (type == 1)
            {
                sheet.AddMergedRegion(new CellRangeAddress(list.Count, list.Count, 0, 2));
                sheet.GetRow(list.Count).GetCell(0).SetCellValue("合计");
            }

            //Write the Workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            return output.ToArray();
        }

        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataTable ImportExcelFile(string filePath)
        {
            IWorkbook hssfworkbook = null;

            #region//初始化信息
            try
            {
                string fileExtension = Path.GetExtension(filePath);
                switch (fileExtension)
                {
                    case ".xls":
                        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            hssfworkbook = new HSSFWorkbook(file);
                        }
                        break;
                    case ".xlsx":
                        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            hssfworkbook = new XSSFWorkbook(file);
                        }
                        break;
                }
            }
            catch
            {

            }
            #endregion

            if (hssfworkbook != null)
            {
                ISheet sheet = hssfworkbook.GetSheetAt(0);
                DataTable table = new DataTable();
                IRow headerRow = sheet.GetRow(0); //第一行为标题行
                int cellCount = headerRow.LastCellNum; //LastCellNum = PhysicalNumberOfCells
                int rowCount = sheet.LastRowNum; //LastRowNum = PhysicalNumberOfRows - 1

                //handling header.
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }
                for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();

                    if (row != null)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                dataRow[j] = GetCellValue(row.GetCell(j));
                            }
                        }
                    }
                    table.Rows.Add(dataRow);
                }
                return table;
            }
            return null;
        }

        /// <summary>
        /// 根据Excel列类型获取列的值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns></returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                    return DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue.ToString() : cell.NumericCellValue.ToString();
                case CellType.Unknown:
                default:
                    return cell.ToString();
                //This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
    }
}