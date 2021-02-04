

using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace NPOI
{
    using System;
	using System.Globalization;
	using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using global::NPOI.SS.UserModel;

    // NPOI 

    /// <summary>
    /// Defines some extensions for <see cref="IEnumerable{T}"/> that using NPOI to provides excel functionality.
    /// </summary>
    public static class IEnumerableNpoiExtensions
    {
        public static byte[] ToExcelContent<T>(this IEnumerable<T> source, string sheetName = "sheet0")
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var book = source.ToWorkbook(null, sheetName);

            using (var ms = new MemoryStream())
            {
                book.Write(ms);
                return ms.ToArray();
            }
        }

        public static void ToExcel<T>(this IEnumerable<T> source, string excelFile, string sheetName = "sheet0") where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (Path.GetExtension(excelFile).Equals(".xls"))
            {
                Excel.Setting.UserXlsx = false;
            }
            else if (Path.GetExtension(excelFile).Equals(".xlsx"))
            {
                Excel.Setting.UserXlsx = true;
            }
            else
            {
                throw new NotSupportedException($"not an excel file extension (*.xls | *.xlsx) {excelFile}");
            }

            var book = source.ToWorkbook(excelFile, sheetName);

            // Write the stream data of workbook to file
            using (var stream = new FileStream(excelFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                book.Write(stream);
            }
        }

        public static IWorkbook ToExcel<T>(this IEnumerable<T> source, string sheetName = "sheet0") where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            } 

            return source.ToWorkbook("", sheetName);
 
        }

        internal static IWorkbook ToWorkbook<T>(this IEnumerable<T> source, string excelFile, string sheetName)
        {
            // can static properties or only instance properties?
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            bool fluentConfigEnabled = Excel.Setting.FluentConfigs.TryGetValue(typeof(T), out var fluentConfig);
            // get the fluent config

            // find out the configs
            var cellConfigs = new CellConfig[properties.Length];
            for (var j = 0; j < properties.Length; j++)
            {
                var property = properties[j];

                // get the property config
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

            // init work book.
            var workbook = InitializeWorkbook(excelFile);

            // new sheet
            var sheet = workbook.CreateSheet(sheetName);
            sheet.DisplayGridlines = true;
            // cache cell styles
            var cellStyles = new Dictionary<int, ICellStyle>();

            // title row cell style
            var titleStyle = workbook.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.Center;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.IsBold = true;
            font.FontName = "宋体";
            titleStyle.SetFont(font);
            titleStyle.WrapText = true;//自动换行
            titleStyle.VerticalAlignment = VerticalAlignment.Center;
            titleStyle.BorderBottom = BorderStyle.Thin;//下边框为细线边框 
            titleStyle.BorderLeft = BorderStyle.Thin;//左边框 
            titleStyle.BorderRight = BorderStyle.Thin;//上边框 
            titleStyle.BorderTop = BorderStyle.Thin;//右边框 
            titleStyle.BottomBorderColor = HSSFColor.Black.Index;//下边框为细线边框 
            titleStyle.LeftBorderColor = HSSFColor.Black.Index;//左边框 
            titleStyle.RightBorderColor = HSSFColor.Black.Index;//上边框 
            titleStyle.TopBorderColor = HSSFColor.Black.Index;//右边框 
            //titleStyle.FillPattern = FillPattern.Bricks;
            //titleStyle.FillBackgroundColor = HSSFColor.Grey40Percent.Index;
            //titleStyle.FillForegroundColor = HSSFColor.White.Index;
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Left;
            cellStyle.BorderBottom = BorderStyle.Thin;//下边框为细线边框 
            cellStyle.BorderLeft = BorderStyle.Thin;//左边框 
            cellStyle.BorderRight = BorderStyle.Thin;//上边框 
            cellStyle.BorderTop = BorderStyle.Thin;//右边框 
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;//下边框为细线边框 
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;//左边框 
            cellStyle.RightBorderColor = HSSFColor.Black.Index;//上边框 
            cellStyle.TopBorderColor = HSSFColor.Black.Index;//右边框 
            cellStyle.WrapText = true;//自动换行
            var cellStyleDictionary = workbook.CreateCellStyle();
            //创建头
            List<StatisticsHeadConfig> statisticsHeadConfigList = new List<StatisticsHeadConfig>();
            if (fluentConfigEnabled)
            {
                statisticsHeadConfigList.AddRange((IEnumerable<StatisticsHeadConfig>)fluentConfig.StatisticsHeadConfigs);
            }
            else
            {
                var attributes = typeof(T).GetCustomAttributes(typeof(StatisticsHeadAttribute), true) as StatisticsHeadAttribute[];
                if (attributes != null && attributes.Length > 0)
                {
                    foreach (var item in attributes)
                    {
                        statisticsHeadConfigList.Add(item.StatisticsConfig);
                    }
                }
            }
            var rowIndex = 0;
            var headrow = sheet.CreateRow(rowIndex);
            foreach (var config in statisticsHeadConfigList)
            {
                headrow.Height = 30 * 20;
                var cell = headrow.CreateCell(config.StartCell);
                cell.SetCellValue(config.Name);
                cell.CellStyle = titleStyle;
                //合并单元格
                if (config.Rows>1 || config.StartCell < config.EndCell)
                {
                    CellRangeAddress region = new CellRangeAddress(rowIndex - config.Rows + 1, rowIndex, config.StartCell, config.EndCell);
                    sheet.AddMergedRegion(region);
                }         
                if (config.NewRow)
                {
                    rowIndex++;
                    headrow = sheet.CreateRow(rowIndex);
                }
            }
            //创建body
            var titleRow = sheet.CreateRow(rowIndex++);
            titleRow.Height = 40 * 20;
            titleRow.HeightInPoints = 40;
            foreach (var item in source)
            {
                var ExportName = string.Empty;
                var ExportValue = string.Empty;
                if (typeof(T).Name == "LiquidateCaseImportModel")
                {
                    var property = properties.Where(a => a.Name == "ExportList").FirstOrDefault();
                    ExportName = property.GetValue(item, null)?.ToString();
                }
                var row = sheet.CreateRow(rowIndex);
                row.Height = 40 * 20;
                row.HeightInPoints = 40;
                int indexCol = 0;
                for (var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
					int index = i;
					var config = cellConfigs[i];
					if (config != null)
					{
						if (config.IsIgnored)
							continue;
						index = config.Index;
                        if (!string.IsNullOrEmpty(ExportName))
                        {
                            if (ExportName.IndexOf(config.Title + "、") < 0) { continue; } else { index = indexCol; indexCol++; }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    // this is the first time.
                    if (rowIndex == statisticsHeadConfigList.Count(p=>p.NewRow)+1)
                    {                        
                        // if not title, using property name as title.
                        var title = property.Name;     
						if (!string.IsNullOrEmpty(config.Title))
						{
							title = config.Title;
						}

						//if (!string.IsNullOrEmpty(config.Formatter))
						{
                            try
                            {
                                var style = workbook.CreateCellStyle();
                                if (!string.IsNullOrEmpty(config.Formatter))
                                {
                                    var dataFormat = workbook.CreateDataFormat();
                                    style.DataFormat = dataFormat.GetFormat(config.Formatter);
                                }

                                style.VerticalAlignment = VerticalAlignment.Center;
                                style.Alignment = HorizontalAlignment.Left;
                                style.BorderBottom = BorderStyle.Thin;//下边框为细线边框 
                                style.BorderLeft = BorderStyle.Thin;//左边框 
                                style.BorderRight = BorderStyle.Thin;//上边框 
                                style.BorderTop = BorderStyle.Thin;//右边框 
                                style.BottomBorderColor = HSSFColor.Black.Index;//下边框为细线边框 
                                style.LeftBorderColor = HSSFColor.Black.Index;//左边框 
                                style.RightBorderColor = HSSFColor.Black.Index;//上边框 
                                style.TopBorderColor = HSSFColor.Black.Index;//右边框 
                                style.WrapText = true;//自动换行
                                cellStyles[i] = style;
							}
							catch (Exception ex)
							{
								// the formatter isn't excel supported formatter
								System.Diagnostics.Debug.WriteLine(ex.ToString());
							}
						}

						var titleCell = titleRow.CreateCell(index);
                        if (config.RowSpan > 0 && config.ColSpan > 0)
                        {
                            var preCell = sheet.GetRow(rowIndex - config.RowSpan).CreateCell(index);
                            preCell.CellStyle = titleStyle;
                            preCell.SetCellValue(title);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex - config.RowSpan, rowIndex - 1, index,
                                index - 1 + config.ColSpan));
                        }
                        else if (config.RowSpan > 0)
                        {
                            //startRow sheet.GetRow(startRow).CreateCell(index) 5 3
                            var preCell = sheet.GetRow(rowIndex - config.RowSpan).CreateCell(index);
                            preCell.CellStyle = titleStyle;
                            preCell.SetCellValue(title);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex - config.RowSpan, rowIndex - 1, index,
                                index));
                        }
                        else if (config.ColSpan > 0)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex - 1, rowIndex - 1, index,
                                index - 1 + config.ColSpan));
                        }

                        {
                            titleCell.CellStyle = titleStyle;
                            titleCell.SetCellValue(title);
                            sheet.SetColumnWidth(index, (config.Width==0 ? 25: config.Width) * 256);
                        }

                    }
                    
                    if (cellStyles.TryGetValue(i, out cellStyleDictionary))
                    {
                        var cell = row.CreateCell(index);
                        var value = property.GetValue(item, null);
                        if (value == null)
                        {
                            cell.CellStyle = cellStyle;
                            cell.SetCellValue("");
                            continue;
                        }
                        var unwrapType = property.PropertyType.UnwrapNullableType();
                        cell.CellStyle = cellStyleDictionary; 
						if (unwrapType == typeof(bool))
						{
							cell.SetCellValue((bool)value);
						}
						else if (unwrapType == typeof(DateTime))
						{
                            if (string.IsNullOrEmpty(config.Formatter))
                            {
                                var dataFormat = workbook.CreateDataFormat();
                                cell.CellStyle.DataFormat = dataFormat.GetFormat("yyyy年MM月dd日");
                            }
                            if (Convert.ToDateTime(value) != DateTime.MinValue)
                            {
                                cell.SetCellValue(Convert.ToDateTime(value));
                            } 
						}
                        else if (unwrapType == typeof(int))
                        {
                            
                            cell.SetCellValue((int)value);
                        }
                        else if (unwrapType == typeof(double) || unwrapType == typeof(decimal))
                        { 
                            cell.SetCellValue(Convert.ToDouble(value)); 
                        } 
						else
                        {
                            cell.SetCellValue(value.ToString());
						}
					} 
                    
                }

                rowIndex++;
            }

			// merge cells
			var mergableConfigs = cellConfigs.Where(c => c != null && c.AllowMerge).ToList();
            if (mergableConfigs.Any())
            {
				// merge cell style
				var vStyle = workbook.CreateCellStyle();
				vStyle.VerticalAlignment = VerticalAlignment.Center;

                foreach (var config in mergableConfigs)
                {
					object previous = null;
					int rowspan = 0, row = 1;
					for (row = 1; row < rowIndex; row++)
					{
						var value = sheet.GetRow(row).GetCellValue(config.Index);
						if (object.Equals(previous, value) && value != null)
						{
							rowspan++;
						}
						else
						{
							if (rowspan > 1)
							{
								sheet.GetRow(row - rowspan).Cells[config.Index].CellStyle = vStyle;
								sheet.AddMergedRegion(new CellRangeAddress(row - rowspan, row - 1, config.Index, config.Index));
							}
							rowspan = 1;
							previous = value;
						}
					}

					// in what case? -> all rows need to be merged
					if (rowspan > 1)
					{
						sheet.GetRow(row - rowspan).Cells[config.Index].CellStyle = vStyle;
						sheet.AddMergedRegion(new CellRangeAddress(row - rowspan, row - 1, config.Index, config.Index));
					}
                }
            }

            if (rowIndex > 1)
            {
                var statistics = new List<StatisticsConfig>();
		        var filterConfigs = new List<FilterConfig>();
		        var freezeConfigs = new List<FreezeConfig>();
                if (fluentConfigEnabled) 
                {
                    statistics.AddRange(fluentConfig.StatisticsConfigs);
                    freezeConfigs.AddRange(fluentConfig.FreezeConfigs);
                    filterConfigs.AddRange(fluentConfig.FilterConfigs);
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

					var freezes = typeof(T).GetCustomAttributes(typeof(FreezeAttribute), true) as FreezeAttribute[];
					if (freezes != null && freezes.Length > 0)
					{
                        foreach (var item in freezes)
                        {
                            freezeConfigs.Add(item.FreezeConfig);
                        }
                    }

					var filters = typeof(T).GetCustomAttributes(typeof(FilterAttribute), true) as FilterAttribute[];
					if (filters != null && filters.Length > 0)
					{
                        foreach (var item in filters)
                        {
                            filterConfigs.Add(item.FilterConfig);
                        }
                    }
                }

				// statistics row
				foreach (var item in statistics)
				{
					var lastRow = sheet.CreateRow(rowIndex);
					var cell = lastRow.CreateCell(0);
                    cell.SetCellValue(item.Name);

					foreach (var column in item.Columns)
					{
						cell = lastRow.CreateCell(column);
						cell.CellFormula = $"{item.Formula}({GetCellPosition(1, column)}:{GetCellPosition(rowIndex - 1, column)})";
                    }
                    rowIndex++;
				}

                // set the freeze
                foreach (var freeze in freezeConfigs)
				{
					sheet.CreateFreezePane(freeze.ColSplit, freeze.RowSplit, freeze.LeftMostColumn, freeze.TopRow);
				}

				// set the auto filter
				foreach (var filter in filterConfigs)
				{
					sheet.SetAutoFilter(new CellRangeAddress(filter.FirstRow, filter.LastRow ?? rowIndex, filter.FirstCol, filter.LastCol));
				}
            }

            //autosize the all columns
            //for (int i = 0; i < properties.Length; i++)
            //{
            //    sheet.AutoSizeColumn(i);
            //}

            return workbook;
        }

        private static IWorkbook InitializeWorkbook(string excelFile)
        {
            var setting = Excel.Setting;
            if (setting.UserXlsx)
            {
                if (!string.IsNullOrEmpty(excelFile) && File.Exists(excelFile))
                {
                    using (var file = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
                    {
                        return new XSSFWorkbook(file);
                    }
                }
                else
                {
                    return new XSSFWorkbook();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(excelFile) && File.Exists(excelFile))
                {
                    using (var file = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
                    {
                        return new HSSFWorkbook(file);
                    }
                }
                else
                {
                    var hssf = new HSSFWorkbook();

                    var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                    dsi.Company = setting.Company;
                    hssf.DocumentSummaryInformation = dsi;

                    var si = PropertySetFactory.CreateSummaryInformation();
                    si.Author = setting.Author;
                    si.Subject = setting.Subject;
                    hssf.SummaryInformation = si;

                    return hssf;
                }
            }
        }

        private static string GetCellPosition(int row, int col)
        {
            col = Convert.ToInt32('A') + col;
            row = row + 1;
            return ((char)col) + row.ToString();
        }
    }
}