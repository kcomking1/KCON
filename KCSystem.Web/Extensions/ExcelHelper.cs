using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NPOI.Attributes;

namespace KCSystem.Web.Extensions
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 设置单元格为下拉框并限制输入值
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="index"></param>
        /// <param name="arr"></param>
        public static void SetCellDropdownList(this ISheet sheet, int index, string[] arr)
        {
            //设置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, index, index);

            //设置 下拉框内容
            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(arr);

            //绑定下拉框和作用区域，并设置错误提示信息
            HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("输入不合法", "请输入下拉列表中的值。");
            dataValidate.ShowPromptBox = true;

            sheet.AddValidationData(dataValidate);
        }



        public static IWorkbook CreateTemplate<T>(string templateName = "sheet0") where T : class, new()
        {
            var wb = new HSSFWorkbook();
            //创建表  
            ISheet sh = wb.CreateSheet(templateName);
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return null;
            }
            //var cellConfigs = new CellConfig[properties.Length];
            var row0 = sh.CreateRow(0);
            foreach (var property in properties)
            {
                if (property.GetCustomAttributes(typeof(ExportValidAttribute), true) is ExportValidAttribute[] attrs && attrs.Length >0)
                {

                    var config = attrs[0];
                    var cell1Top0 = row0.CreateCell(config.Index);
                    setCellStyle(wb,cell1Top0);
                    switch (config.ValidType)
                    {
                        case ValidType.Date:
                            sh.SetCellDate(config.Index);
                            break;
                        case ValidType.Integer:
                            cell1Top0.SetCellType(CellType.Numeric);
                            sh.SetCellInputNumber(config.Index,"0","1000000","请输入数字");
                            break;
                        case ValidType.DropDown:
                            // sh.SetCellDropdownList(config.Index, config.DropDownValid);
                            break;
                        case ValidType.Phone:
                            sh.SetCellInputPhone(config.Index);
                            break;
                        case ValidType.Formula:
                            sh.SetCellFormula(config.Index,config.Formula);
                            break;
                        default:
                            cell1Top0.SetCellType(CellType.String);
                            break;
                    } 
                    cell1Top0.SetCellValue(config.Title);
                }
            }
            return wb;
        }

        /// <summary>
        /// 验证手机号格式
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="index"></param>
        /// <param name="formula"></param>
        /// <param name="message"></param>
        public static void SetCellFormula(this ISheet sheet, int index,string formula,string message="输入格式不正确")
        {
            //设置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, index, index); 
            var constraint = DVConstraint.
                CreateCustomFormulaConstraint(formula);
            var dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("输入不合法", message);
            //dataValidate.PromptBoxTitle = "ErrorInput";
          
            sheet.AddValidationData(dataValidate);
        }

        /// <summary>
        /// 验证手机号格式
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="index"></param>
        public static void SetCellInputPhone(this ISheet sheet, int index)
        {
            //设置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, index, index);

            //第二个参数int comparisonOperator  参考源码获取
            //https://github.com/tonyqus/npoi
            //NPOITest项目
            //var constraint = DVConstraint.
            //  CreateCustomFormulaConstraint($"(((LEFT(A1,2)=\"13\")+(LEFT(A1,2)=\"15\")+(LEFT(A1,2)=\"18\"))*(LEN(A1)=11))");
            var constraint = DVConstraint.
                CreateCustomFormulaConstraint("COUNTIF(D2,\"*@*.*\")=1");
           
            var dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("输入不合法", "请输入正确的手机号码");
            //dataValidate.PromptBoxTitle = "ErrorInput";

            sheet.AddValidationData(dataValidate);
        }
        /// <summary>
        /// 设置单元格只能输入数字
        /// </summary>
        /// <param name="sheet"></param>
        public static void SetCellInputNumber(this ISheet sheet, int index, string min, string max, string msg)
        {
            //设置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, index, index);

            //第二个参数int comparisonOperator  参考源码获取
            //https://github.com/tonyqus/npoi
            //NPOITest项目
            DVConstraint constraint = DVConstraint.CreateNumericConstraint(
                ValidationType.INTEGER, OperatorType.BETWEEN, min, max);
            
            HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("输入不合法", msg);
            //dataValidate.PromptBoxTitle = "ErrorInput";

            sheet.AddValidationData(dataValidate);
        }

        /// <summary>
        /// 设置单元格为日期
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="index"></param>
        public static void SetCellDate(this ISheet sheet, int index)
        {
            //设置生成下拉框的行和列
            var cellRegions = new CellRangeAddressList(1, 65535, index, index);

            //设置 下拉框内容
            DVConstraint constraint = DVConstraint.CreateDateConstraint(OperatorType.BETWEEN, "1900-01-01", "2999-12-31", "yyyy-MM-dd");

            //绑定下拉框和作用区域，并设置错误提示信息
            HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("输入不合法", "请输入正确的时间格式");
            dataValidate.ShowPromptBox = true;

            sheet.AddValidationData(dataValidate);
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <param name="firstCell"></param>
        /// <param name="lastCell"></param>
        public static void mergeCell(this ISheet sheet, int firstRow, int lastRow, int firstCell, int lastCell)
        {
            sheet.AddMergedRegion(new CellRangeAddress(firstRow, lastRow, firstCell, lastCell));//2.0使用 2.0以下为Region
        }

        /// <summary>
        /// 设置单元格样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell"></param>
        public static void setCellStyle(HSSFWorkbook workbook, ICell cell)
        {
            HSSFCellStyle fCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ffont = (HSSFFont)workbook.CreateFont();
            //ffont.FontHeight = 20 * 20;
            ffont.FontName = "宋体";
            //ffont.Color = HSSFColor.Red.Index;
            fCellStyle.SetFont(ffont);

            fCellStyle.VerticalAlignment = VerticalAlignment.Center;//垂直对齐
            fCellStyle.Alignment = HorizontalAlignment.Center;//水平对齐
            cell.CellStyle = fCellStyle;
        }

        /// <summary>
        /// 将Excel单表转为Datatable
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileType"></param>
        /// <param name="strMsg"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static DataTable ExcelToDatatable(Stream stream, string fileType, out string strMsg, string sheetName = null)
        {
            strMsg = "";
            DataTable dt = new DataTable();
            ISheet sheet = null;
            IWorkbook workbook = null;
            try
            {
                #region 判断excel版本
                //2007以上版本excel
                if (fileType == ".xlsx")
                {
                    workbook = new XSSFWorkbook(stream);
                }
                //2007以下版本excel
                else if (fileType == ".xls")
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else
                {
                    throw new Exception("传入的不是Excel文件！");
                }
                #endregion
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum;
                    for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                    {
                        ICell cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue.Trim();
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                DataColumn dataColumn = new DataColumn(cellValue);
                                dt.Columns.Add(dataColumn);
                            }
                        }
                    }
                    DataRow dataRow = null;
                    //遍历行
                    for (int j = sheet.FirstRowNum + 1; j <= sheet.LastRowNum; j++)
                    {
                        IRow row = sheet.GetRow(j);
                        dataRow = dt.NewRow();
                        if (row == null || row.FirstCellNum < 0)
                        {
                            continue;
                        }
                        //遍历列
                        for (int i = row.FirstCellNum; i < cellCount; i++)
                        {
                            ICell cellData = row.GetCell(i);
                            if (cellData != null)
                            {
                                //判断是否为数字型，必须加这个判断不然下面的日期判断会异常
                                if (cellData.CellType == CellType.Numeric)
                                {
                                    //判断是否日期类型
                                    if (DateUtil.IsCellDateFormatted(cellData))
                                    {
                                        dataRow[i] = cellData.DateCellValue;
                                    }
                                    else
                                    {
                                        dataRow[i] = cellData.ToString().Trim();
                                    }
                                }
                                else
                                {
                                    dataRow[i] = cellData.ToString().Trim();
                                }
                            }
                        }
                        dt.Rows.Add(dataRow);
                    }
                }
                else
                {
                    throw new Exception("没有获取到Excel中的数据表！");
                }
            }
            catch (Exception ex)
            {
                strMsg = ex.Message;
            }
            return dt;
        }
    }

}
