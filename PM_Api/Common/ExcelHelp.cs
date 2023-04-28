
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PM_Api.Common
{
    public class ExcelHelp
    {
        #region 导出excel
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="data">数据源</param>
        /// <param name="fileName">文件名</param>
        public static void DataTableToExcel(DataTable data, string fileName)
        {
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.Charset = "Utf-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName + ".xls", Encoding.UTF8));
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            sbHtml.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
            string NumberAsTextExp = "vnd.ms-excel.numberformat:@";
            //写表头
            string colHeaders = "";
            sbHtml.Append("<tr>");
            for (int i = 0; i < data.Columns.Count; i++)
            {
                sbHtml.Append("<td style='background-color:#ff9800'>").Append(data.Columns[i].Caption.ToString()).Append("</td>");
            }
            sbHtml.Append(colHeaders);
            sbHtml.AppendLine("</tr>");
            //写数据
            foreach (DataRow row in data.Rows)
            {
                sbHtml.Append("<tr>");
                for (int i = 0; i < row.ItemArray.Count(); i++)
                {
                    sbHtml.Append("<td style='" + NumberAsTextExp + "'>").Append(row[i]).Append("</td>");
                }
                sbHtml.AppendLine("</tr>");
            }
            sbHtml.AppendLine("</table>");
            HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Default.GetBytes(sbHtml.ToString()));
            HttpContext.Current.Response.End();
        }
        #endregion

        #region DataTable导入数据库
        /// <summary>
        /// DataTable导入数据库
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="TabelName">数据库对应表名</param>
        public static void InsertTable(DataTable dt, string TabelName)
        {
            string str = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString.ToString();
            //声明数据库连接
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            //声明SqlBulkCopy ,using释放非托管资源
            using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
            {
                //一次批量的插入的数据量
                sqlBC.BatchSize = 1000;
                //超时之前操作完成所允许的秒数，如果超时则事务不会提交 ，数据将回滚，所有已复制的行都会从目标表中移除
                sqlBC.BulkCopyTimeout = 60;
                //設定 NotifyAfter 属性，以便在每插入10000 条数据时，呼叫相应事件。 
                sqlBC.NotifyAfter = 10000;
                // sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                //设置要批量写入的表
                sqlBC.DestinationTableName = TabelName;
                //自定义的datatable和数据库的字段进行对应
                //sqlBC.ColumnMappings.Add("id", "tel");
                //sqlBC.ColumnMappings.Add("name", "neirong");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sqlBC.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                }
                //批量写入
                sqlBC.WriteToServer(dt);
            }
            conn.Dispose();
        }
        #endregion


        /// <summary>      
        /// DataTable导出到Excel的MemoryStream      
        /// </summary>      
        /// <param name="dtSource">源DataTable</param>      
        /// <param name="strSheetName">工作表名称</param>   
        /// <Author>CallmeYhz 2015-11-26 10:13:09</Author>      
        public static MemoryStream Export(DataTable dtSource, string strSheetName, string[] oldColumnNames, string[] newColumnNames)
        {
            if (oldColumnNames.Length != newColumnNames.Length)
            {
                return new MemoryStream();
            }
            HSSFWorkbook workbook = new HSSFWorkbook();
            //HSSFSheet sheet = workbook.CreateSheet();// workbook.CreateSheet();   
            ISheet sheet = workbook.CreateSheet(strSheetName);

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "http://....../";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息
                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            #region 取得列宽
            int[] arrColWidth = new int[oldColumnNames.Length];
            for (int i = 0; i < oldColumnNames.Length; i++)
            {
                arrColWidth[i] = Encoding.GetEncoding(936).GetBytes(newColumnNames[i]).Length;
            }
            /* 
            foreach (DataColumn item in dtSource.Columns) 
            { 
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length; 
            } 
             * */

            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < oldColumnNames.Length; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][oldColumnNames[j]].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
                /* 
                for (int j = 0; j < dtSource.Columns.Count; j++) 
                { 
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length; 
                    if (intTemp > arrColWidth[j]) 
                    { 
                        arrColWidth[j] = intTemp; 
                    } 
                } 
                 * */
            }
            #endregion
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet(strSheetName + ((int)rowIndex / 65535).ToString());
                    }
                    #region 列头及样式
                    {
                        //HSSFRow headerRow = sheet.CreateRow(0);   
                        IRow headerRow = sheet.CreateRow(0);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);


                        for (int i = 0; i < oldColumnNames.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(newColumnNames[i]);
                            headerRow.GetCell(i).CellStyle = headStyle;
                            //设置列宽   
                            sheet.SetColumnWidth(i, (arrColWidth[i] + 1) * 256);
                        }
                        /* 
                        foreach (DataColumn column in dtSource.Columns) 
                        { 
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName); 
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle; 
 
                            //设置列宽    
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256); 
                        } 
                         * */
                    }
                    #endregion

                    rowIndex = 1;
                }
                #endregion


                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                //foreach (DataColumn column in dtSource.Columns)   
                for (int i = 0; i < oldColumnNames.Length; i++)
                {
                    ICell newCell = dataRow.CreateCell(i);

                    string drValue = row[oldColumnNames[i]].ToString();

                    switch (dtSource.Columns[oldColumnNames[i]].DataType.ToString())
                    {
                        case "System.String"://字符串类型      
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型      
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示      
                            break;
                        case "System.Boolean"://布尔型      
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型      
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型      
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理      
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();   
                sheet = null;
                workbook = null;
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet      
                return ms;
            }
        }

        private static Image DrawText(String text, System.Drawing.Font font, Color textColor, Color backColor, double height, double width)
        {
            //创建一个指定宽度和高度的位图图像
            Image img = new Bitmap((int)width, (int)height);
            Graphics drawing = Graphics.FromImage(img);
            //获取文本大小
            SizeF textSize = drawing.MeasureString(text, font);
            //旋转图片
            drawing.TranslateTransform(((int)width - textSize.Width) / 2, ((int)height - textSize.Height) / 2);
            drawing.RotateTransform(-45);
            drawing.TranslateTransform(-((int)width - textSize.Width) / 2, -((int)height - textSize.Height) / 2);
            //绘制背景
            drawing.Clear(backColor);
            //创建文本刷
            Brush textBrush = new SolidBrush(textColor);
            drawing.DrawString(text, font, textBrush, ((int)width - textSize.Width) / 2, ((int)height - textSize.Height) / 2);
            drawing.Save();
            return img;
        }

        /// <summary>      
        /// WEB导出DataTable到Excel      
        /// </summary>      
        /// <param name="dtSource">源DataTable</param>      
        /// <param name="strFileName">文件名</param>      
        /// <Author>CallmeYhz 2015-11-26 10:13:09</Author>      
        public static void ExportByWeb(DataTable dtSource, string strFileName)
        {
            ExportByWeb(dtSource, strFileName, "sheet");
        }

        /// <summary>   
        /// WEB导出DataTable到Excel   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="strFileName">输出文件名，包含扩展名</param>   
        /// <param name="oldColumnNames">要导出的DataTable列数组</param>   
        /// <param name="newColumnNames">导出后的对应列名</param>   
        public static void ExportByWeb(DataTable dtSource, string strFileName, string[] oldColumnNames, string[] newColumnNames)
        {
            ExportByWeb(dtSource, strFileName, "sheet", oldColumnNames, newColumnNames);
        }

        /// <summary>   
        /// WEB导出DataTable到Excel   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="strFileName">输出文件名</param>   
        /// <param name="strSheetName">工作表名称</param>   
        public static void ExportByWeb(DataTable dtSource, string strFileName, string strSheetName)
        {
            HttpContext curContext = HttpContext.Current;

            // 设置编码和附件格式      
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            //生成列   
            string columns = "";
            for (int i = 0; i < dtSource.Columns.Count; i++)
            {
                if (i > 0)
                {
                    columns += ",";
                }
                columns += dtSource.Columns[i].ColumnName;
            }

            curContext.Response.BinaryWrite(Export(dtSource, strSheetName, columns.Split(','), columns.Split(',')).GetBuffer());
            curContext.Response.End();

        }

        /// <summary>   
        /// 导出DataTable到Excel   
        /// </summary>   
        /// <param name="dtSource">要导出的DataTable</param>   
        /// <param name="strFileName">文件名，包含扩展名</param>   
        /// <param name="strSheetName">工作表名</param>   
        /// <param name="oldColumnNames">要导出的DataTable列数组</param>   
        /// <param name="newColumnNames">导出后的对应列名</param>   
        public static void ExportByWeb(DataTable dtSource, string strFileName, string strSheetName, string[] oldColumnNames, string[] newColumnNames)
        {
            HttpContext curContext = HttpContext.Current;

            // 设置编码和附件格式      
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            curContext.Response.BinaryWrite(Export(dtSource, strSheetName, oldColumnNames, newColumnNames).GetBuffer());
            curContext.Response.End();
        }

        /// <summary>读取excel      
        /// 默认第一行为表头，导入第一个工作表   
        /// </summary>      
        /// <param name="strFileName">excel文档路径</param>      
        /// <returns></returns>      
        public static DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();

            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = !(Path.GetExtension(strFileName) == ".xlsx") ? (IWorkbook)new HSSFWorkbook((Stream)file) : (IWorkbook)new XSSFWorkbook((Stream)file);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        /// <summary>   
        /// 从Excel中获取数据到DataTable   
        /// </summary>   
        /// <param name="strFileName">Excel文件全路径(服务器路径)</param>   
        /// <param name="SheetName">要获取数据的工作表名称</param>   
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>   
        /// <returns></returns>   
        public static DataTable RenderDataTableFromExcel(string strFileName, string SheetName, int HeaderRowIndex)
        {
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new HSSFWorkbook(file);
                ISheet sheet = workbook.GetSheet(SheetName);
                return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
            }
        }

        /// <summary>   
        /// 从Excel中获取数据到DataTable   
        /// </summary>   
        /// <param name="strFileName">Excel文件全路径(服务器路径)</param>   
        /// <param name="SheetIndex">要获取数据的工作表序号(从0开始)</param>   
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>   
        /// <returns></returns>   
        public static DataTable RenderDataTableFromExcel(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new HSSFWorkbook(file);
                string SheetName = workbook.GetSheetName(SheetIndex);
                return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
            }
        }

        /// <summary>   
        /// 从Excel中获取数据到DataTable   
        /// </summary>   
        /// <param name="ExcelFileStream">Excel文件流</param>   
        /// <param name="SheetName">要获取数据的工作表名称</param>   
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>   
        /// <returns></returns>   
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
        {
            IWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ExcelFileStream.Close();
            return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
        }

        /// <summary>   
        /// 从Excel中获取数据到DataTable   
        /// </summary>   
        /// <param name="ExcelFileStream">Excel文件流</param>   
        /// <param name="SheetIndex">要获取数据的工作表序号(从0开始)</param>   
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>   
        /// <returns></returns>   
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
        {
            IWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ExcelFileStream.Close();
            string SheetName = workbook.GetSheetName(SheetIndex);
            return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
        }

        /// <summary>   
        /// 从Excel中获取数据到DataTable   
        /// </summary>   
        /// <param name="workbook">要处理的工作薄</param>   
        /// <param name="SheetName">要获取数据的工作表名称</param>   
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>   
        /// <returns></returns>   
        public static DataTable RenderDataTableFromExcel(IWorkbook workbook, string SheetName, int HeaderRowIndex)
        {
            ISheet sheet = workbook.GetSheet(SheetName);
            DataTable table = new DataTable();
            try
            {
                IRow headerRow = sheet.GetRow(HeaderRowIndex);
                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }

                int rowCount = sheet.LastRowNum;

                #region 循环各行各列,写入数据到DataTable
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        ICell cell = row.GetCell(j);
                        if (cell == null)
                        {
                            dataRow[j] = null;
                        }
                        else
                        {
                            //dataRow[j] = cell.ToString();   
                            switch (cell.CellType)
                            {
                                case CellType.Blank:
                                    dataRow[j] = null;
                                    break;
                                case CellType.Boolean:
                                    dataRow[j] = cell.BooleanCellValue;
                                    break;
                                case CellType.Numeric:
                                    dataRow[j] = cell.ToString();
                                    break;
                                case CellType.String:
                                    dataRow[j] = cell.StringCellValue;
                                    break;
                                case CellType.Error:
                                    dataRow[j] = cell.ErrorCellValue;
                                    break;
                                case CellType.Formula:
                                default:
                                    dataRow[j] = "=" + cell.CellFormula;
                                    break;
                            }
                        }
                    }
                    table.Rows.Add(dataRow);
                    //dataRow[j] = row.GetCell(j).ToString();   
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                table.Clear();
                table.Columns.Clear();
                table.Columns.Add("出错了");
                DataRow dr = table.NewRow();
                dr[0] = ex.Message;
                table.Rows.Add(dr);
                return table;
            }
            finally
            {
                //sheet.Dispose();   
                workbook = null;
                sheet = null;
            }
            #region 清除最后的空行
            for (int i = table.Rows.Count - 1; i > 0; i--)
            {
                bool isnull = true;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (table.Rows[i][j] != null)
                    {
                        if (table.Rows[i][j].ToString() != "")
                        {
                            isnull = false;
                            break;
                        }
                    }
                }
                if (isnull)
                {
                    table.Rows[i].Delete();
                }
            }
            #endregion
            return table;
        }
    }
}