
 
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;

namespace NPOI
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Globalization;
    using System.Collections.Generic;
	using System.Reflection; 

    /// <summary>
    /// Represents the cell value converter, which convert the value to another value.
    /// </summary>
    /// <param name="row">The row of the excel sheet.</param>
    /// <param name="cell">The cell of the excel sheet.</param>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public delegate object ValueConverter(int row, int cell, object value);

    /// <summary>
    /// Provides some methods for loading <see cref="IEnumerable{T}"/> from excel.
    /// </summary>
    public static class Excel
    {
        /// <summary>
        /// Gets or sets the setting.
        /// </summary>
        /// <value>The setting.</value>
        public static ExcelSetting Setting { get; set; } = new ExcelSetting();

        /// <summary>
        /// Loading <see cref="IEnumerable{T}"/> from specified excel file.
        /// /// </summary>
        /// <typeparam name="T">The type of the model.</typeparam>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="startRow">The row to start read.</param>
        /// <param name="sheetIndex">Which sheet to read.</param>
        /// <param name="valueConverter">The cell value convert.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> loading from excel.</returns>
        public static IEnumerable<T> Load<T>(string excelFile, int startRow = 1, int sheetIndex = 0, ValueConverter valueConverter = null) where T : class, new()
        {
            if (!File.Exists(excelFile))
            {
                throw new FileNotFoundException();
            }

            var workbook = InitializeWorkbook(excelFile);
            return Load<T>(workbook, startRow, sheetIndex, valueConverter);
        }

        public static IEnumerable<T> Load<T>(IWorkbook workbook, int startRow = 1, int sheetIndex = 0, ValueConverter valueConverter = null) where T : class, new()
        {

            // currently, only handle sheet one (or call side using foreach to support multiple sheet)
            var sheet = workbook.GetSheetAt(sheetIndex);

            // get the writable properties
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            bool fluentConfigEnabled = Setting.FluentConfigs.TryGetValue(typeof(T), out var fluentConfig);
            // get the fluent config

            var cellConfigs = new CellConfig[properties.Length];
            for (var j = 0; j < properties.Length; j++)
            {
                var property = properties[j];
                if (fluentConfigEnabled && fluentConfig.PropertyConfigs.TryGetValue(property, out var pc))
                {
                    // fluent configure first(Hight Priority)
                    cellConfigs[j] = pc.CellConfig;
                }
                else
                {
                    var attrs = property.GetCustomAttributes(typeof(ColumnAttribute), true) as ColumnAttribute[];
                    if (attrs != null && attrs.Length > 0)
                    {
                        cellConfigs[j] = attrs[0].CellConfig;
                    }
                    else
                    {
                        cellConfigs[j] = null;
                    }
                }
            }

            var statistics = new List<StatisticsConfig>();
            if (fluentConfigEnabled)
            {
                statistics.AddRange(fluentConfig.StatisticsConfigs);
            }
            else
            {
                var attributes = typeof(T).GetCustomAttributes(typeof(StatisticsAttribute), true) as StatisticsAttribute[];
                if (attributes != null && attributes.Length > 0)
                {
                    foreach (var item in attributes)
                    {
                        statistics.Add(item.StatisticsConfig);
                    }
                }
            }

            var list = new List<T>();
            int idx = 0;

            // get the physical rows
            var rows = sheet.GetRowEnumerator();
            while (rows.MoveNext())
            {
                var row = rows.Current as IRow;
                 
                idx++;

                if (row.RowNum < startRow)
                {
                    continue;
                }

                if (idx ==381)
                {

                }
                var item = new T();
                var itemIsValid = true;
                for (int i = 0; i < properties.Length; i++)
                {
                    var prop = properties[i];

                    int index = i;
                    var config = cellConfigs[i];
                    if (config != null)
                    {
                        index = config.Index; 
                        // check again
                        if (index < 0)
                        {
                            continue;
                            //throw new ApplicationException("Please set the 'index' or 'autoIndex' by fluent api or attributes");
                        }
                    }
                    // property type
                    var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    object value = null;
                    try
                    {
                        value = row.GetCellValue(index);
                        if (valueConverter != null)
                        {
                            value = valueConverter(row.RowNum, index, value);
                        }

                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            continue;
                        }

                        // check whether is statics row
                        if (idx > startRow + 1 && index == 0
                                               &&
                                               statistics.Any(s => s.Name.Equals(value.ToString(),
                                                   StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var st = statistics.FirstOrDefault(s =>
                                s.Name.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase));
                            var formula = row.GetCellValue(st.Columns.First()).ToString();
                            if (formula.StartsWith(st.Formula, StringComparison.InvariantCultureIgnoreCase))
                            {
                                itemIsValid = false;
                                break;
                            }
                        }

                        if (value is double && (propType == typeof(DateTime) || propType == typeof(DateTime?)))
                        {
                            value = DateTime.FromOADate((double)value);
                        }

                        
                        var safeValue = Convert.ChangeType(value, propType, CultureInfo.CurrentCulture);

                        prop.SetValue(item, safeValue, null);
                    }
                    catch (Exception e)
                    {
                        var fieldName = config == null ? prop.Name : config.Title;
                        throw new Exception($"第{idx + 1}条数据中[{fieldName}]类型{propType.Name}的值[{value}]无效。", e.InnerException);
                    }
                }

                if (itemIsValid)
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public static object GetCellValue(this IRow row, int index)
        {
            var cell = row.GetCell(index);
            if (cell == null)
            {
                return null;
            }

            if (cell.IsMergedCell)
            {
                // what can I do here?

            }

            switch (cell.CellType)
            {
                // This is a trick to get the correct value of the cell.
                // NumericCellValue will return a numeric value no matter the cell value is a date or a number.
                case CellType.Numeric:
                    //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                    if (DateUtil.IsCellDateFormatted(cell))//日期类型
                    { 
                        return DateTime.FromOADate(cell.NumericCellValue); 
                    }
                    else//其他数字类型
                    {
                        return   cell.NumericCellValue;
                    } 
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;

                // how?
                case CellType.Formula:
                    try
                    {
                        if (Excel.Setting.UserXlsx)
                        {
                            XSSFFormulaEvaluator e = new XSSFFormulaEvaluator(cell.Sheet.Workbook);
                            e.EvaluateInCell(cell);
                            return cell.ToString();
                        }
                        else
                        {
                            HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                            e.EvaluateInCell(cell);
                            return cell.ToString();
                        }
                       
                    }
                    catch
                    {
                        return null;
                    }

                case CellType.Blank:
                case CellType.Unknown:
                default:
                    return null;
            }
        }
        public static DateTime? GetCellDateValue(this IRow row, int index)
        {
            var cell = row.GetCell(index);
            if (cell == null || cell.NumericCellValue == 0)
            {
                return null;
            }
            return DateTime.FromOADate(cell.NumericCellValue);

        }


        public static void SetCellValue(this IRow row,IWorkbook workbook, int index,string value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value); 
        }

        public static void SetCellValue(this IRow row, IWorkbook workbook, ICellStyle cellStyle, int index, string value)
        {
            var cell = row.CreateCell(index);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value);
        }

        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, DateTime value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            var dataFormat = workbook.CreateDataFormat();
            cellStyle.DataFormat = dataFormat.GetFormat("yyyy年MM月dd日");
            cell.CellStyle = cellStyle;
           
            cell.SetCellValue(value);
        }
        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, DateTime? value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            var dataFormat = workbook.CreateDataFormat();
            cellStyle.DataFormat = dataFormat.GetFormat("yyyy年MM月dd日");
            cell.CellStyle = cellStyle;
            if(value!= null)
            cell.SetCellValue(value.Value);
        }
        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, bool value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value);
        }
        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, double value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value);
        }
        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, int value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value);
        }

        public static void SetCellValue(this IRow row, IWorkbook workbook, ICellStyle cellStyle, int index, int value)
        {
            var cell = row.CreateCell(index);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value);
        }

        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, decimal value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Convert.ToDouble(value));
        }


        public static void SetCellValue(this IRow row, IWorkbook workbook, ICellStyle cellStyle, int index, decimal value)
        {
            var cell = row.CreateCell(index);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Convert.ToDouble(value));
        }

        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, decimal? value)
        {
            
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            cell.CellStyle = cellStyle;
            if (value != null)
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }            
        }

        public static void SetCellValue(this IRow row, IWorkbook workbook, ICellStyle cellStyle, int index, decimal? value)
        {
            var cell = row.CreateCell(index);
            cell.CellStyle = cellStyle; 
            if (value != null)
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
        }

        public static void SetCellValue(this IRow row, IWorkbook workbook, int index, IRichTextString value)
        {
            var cell = row.CreateCell(index);
            var cellStyle = CreateCellStyle(workbook);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value);
        }
        private static ICellStyle CreateCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Center;
            //cellStyle.BorderBottom = BorderStyle.Thin;//下边框为细线边框 
            //cellStyle.BorderLeft = BorderStyle.Thin;//左边框 
            //cellStyle.BorderRight = BorderStyle.Thin;//上边框 
            //cellStyle.BorderTop = BorderStyle.Thin;//右边框 
            //cellStyle.BottomBorderColor = HSSFColor.Black.Index;//下边框为细线边框 
            //cellStyle.LeftBorderColor = HSSFColor.Black.Index;//左边框 
            //cellStyle.RightBorderColor = HSSFColor.Black.Index;//上边框 
            //cellStyle.TopBorderColor = HSSFColor.Black.Index;//右边框 
            cellStyle.WrapText = true; //自动换行 
            return cellStyle;
        }

        internal static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        private static IWorkbook InitializeWorkbook(string excelFile)
        {
            if (Path.GetExtension(excelFile).Equals(".xls"))
            {
                using (var file = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
                {
                    return new HSSFWorkbook(file);
                }
            }
            else if (Path.GetExtension(excelFile).Equals(".xlsx"))
            {
                using (var file = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
                {
                    return new XSSFWorkbook(file);
                }
            }
            else
            {
                throw new NotSupportedException($"not an excel file {excelFile}");
            }
        }
    }
}
