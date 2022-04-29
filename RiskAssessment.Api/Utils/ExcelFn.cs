﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RiskAssessment.Api.Utils
{
    public static class ExcelFn
    {
        public static DataTable ExcelToDataTable(string path, int sheet = 0, bool hasHeader = true, int skipRowWithoutHeader = 0)
        {
            var tbl = new DataTable();
            using (var streamFile = File.OpenRead(path))
            {
                using (var pck = new ExcelPackage())
                {
                    pck.Load(streamFile);
                    var listWorkSheets = pck.Workbook.Worksheets.ToList();
                    var ws = listWorkSheets.Count >= sheet + 1 ? listWorkSheets[sheet] : pck.Workbook.Worksheets.First();
                    var countCol = ws.Dimension.End.Column;
                    var maxCol = 0;
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, countCol])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                        maxCol++;
                    }
                    var startRow = (hasHeader ? 2 : 1) + skipRowWithoutHeader;
                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, maxCol];
                        var row = tbl.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;
                        }
                        var isNull = true;
                        for (var i = 0; i < maxCol; i++)
                        {
                            if (isNull && (row[i] != DBNull.Value && (row[i] + "") != ""))
                                isNull = false;
                        }
                        if (!isNull)
                            tbl.Rows.Add(row);
                    }
                }
            }
            return tbl;
        }
        public static string DataTableToJsonWithStringBuilder(DataTable table)
        {
            var jsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                jsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var rowStr = new StringBuilder();
                    var hasData = false;

                    rowStr.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            rowStr.Append("\"" + table.Columns[j].ColumnName.ToString()
                                              + "\":" + "\""
                                              + table.Rows[i][j].ToString().Replace("\n", "\\n").Replace("\t", "\\t") + "\",");

                            if (!string.IsNullOrEmpty(table.Rows[i][j].ToString()))
                                hasData = true;
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            rowStr.Append("\"" + table.Columns[j].ColumnName.ToString()
                                              + "\":" + "\""
                                              + table.Rows[i][j].ToString() + "\"");
                            if (!string.IsNullOrEmpty(table.Rows[i][j].ToString()))
                                hasData = true;
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        rowStr.Append("}");
                    }
                    else
                    {
                        rowStr.Append("},");
                    }

                    if (hasData)
                        jsonString.Append(rowStr);

                }
                jsonString.Append("]");
            }
            return jsonString.ToString();
        }
        public static List<T> ExcelToList<T>(string path, int sheet = 0, bool hasHeader = true, int skipRowWithoutHeader = 0) where T : class, new()
        {
            var tbl = new DataTable();
            using (var streamFile = File.OpenRead(path))
            {
                using (var pck = new ExcelPackage())
                {
                    pck.Load(streamFile);
                    var listWorkSheets = pck.Workbook.Worksheets.ToList();
                    var ws = listWorkSheets.Count >= sheet + 1 ? listWorkSheets[sheet] : pck.Workbook.Worksheets.First();
                    var countCol = ws.Dimension.End.Column;
                    var maxCol = 0;
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, countCol])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                        maxCol++;
                    }
                    var startRow = (hasHeader ? 2 : 1) + skipRowWithoutHeader;
                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, maxCol];
                        var row = tbl.NewRow();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;
                        }
                        var isNull = true;
                        for (var i = 0; i < maxCol; i++)
                        {
                            if (isNull && (row[i] != DBNull.Value && (row[i] + "") != ""))
                                isNull = false;
                        }
                        if (!isNull)
                            tbl.Rows.Add(row);
                    }
                }
            }
            if (tbl.Rows.Count == 0)
            {
                tbl.Dispose();
                return new List<T>();
            }
            var str = DataTableToJsonWithStringBuilder(tbl);
            tbl.Dispose();
            var lst = System.Text.Json.JsonSerializer.Deserialize<List<T>>(str);
            return lst;
        }
        public static List<T> UploadToList<T>(MemoryStream streamFile, int sheet = 0, bool hasHeader = true, int skipRowWithoutHeader = 0, Dictionary<string, string> mappingColumn = null) where T : class, new()
        {
            var tbl = new DataTable();
            using (var pck = new ExcelPackage())
            {
                pck.Load(streamFile);
                var listWorkSheets = pck.Workbook.Worksheets.ToList();
                var ws = listWorkSheets.Count >= sheet + 1 ? listWorkSheets[sheet] : pck.Workbook.Worksheets.First();
                var countCol = ws.Dimension.End.Column;
                var maxCol = 0;
                foreach (var firstRowCell in ws.Cells[1 + skipRowWithoutHeader, 1, 1 + skipRowWithoutHeader, countCol])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    maxCol++;
                }
                var startRow = (hasHeader ? 2 : 1) + skipRowWithoutHeader;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, maxCol];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Value;
                    }
                    var isNull = true;
                    for (var i = 0; i < maxCol; i++)
                    {
                        if (row[i] != DBNull.Value && (row[i] + "").Trim() != "")
                            isNull = false;
                    }
                    if (!isNull)
                        tbl.Rows.Add(row);
                }
            }
            if (tbl.Rows.Count == 0)
            {
                tbl.Dispose();
                return new List<T>();
            }
            if (mappingColumn != null && mappingColumn.Count > 0)
            {
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    var clName = tbl.Columns[i].ColumnName;
                    if (mappingColumn.TryGetValue(clName, out string clReplace))
                        tbl.Columns[i].ColumnName = clReplace;
                }
            }
            var str = DataTableToJsonWithStringBuilder(tbl);
            tbl.Dispose();
            var lst = System.Text.Json.JsonSerializer.Deserialize<List<T>>(str);
            return lst;
        }

        /// <summary>
        /// Đây là hàm clone ra từ hàm DataTableToJsonWithStringBuilder, hàm này sử dụng thư viện newtonsoft.json thay vì system.text.json
        /// hàm này vẫn đang trong giai đoạn test trước khi áp dụng chính thức
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="streamFile"></param>
        /// <param name="sheet"></param>
        /// <param name="hasHeader"></param>
        /// <param name="skipRowWithoutHeader"></param>
        /// <param name="mappingColumn"></param>
        /// <returns>String json từ datatable</returns>

        public static string DataTableToJsonWithStringBuilderProcess(DataTable table)
        {

            var list = new List<JObject>();

            foreach (DataRow row in table.Rows)
            {
                var item = new JObject();

                foreach (DataColumn column in table.Columns)
                {
                    item.Add(column.ColumnName, JToken.FromObject(row[column.ColumnName]));
                }

                list.Add(item);
            }
            return JsonConvert.SerializeObject(list).ToString();

        }

        /// <summary>
        /// Đây là hàm clone ra từ hàm UploadToList, hàm này sử dụng thư viện newtonsoft.json thay vì system.text.json
        /// hàm này vẫn đang trong giai đoạn test trước khi áp dụng chính thức
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="streamFile"></param>
        /// <param name="sheet"></param>
        /// <param name="hasHeader"></param>
        /// <param name="skipRowWithoutHeader"></param>
        /// <param name="mappingColumn"></param>
        /// <returns>list từ file excel</returns>
        public static List<T> UploadToListProcess<T>(MemoryStream streamFile, int sheet = 0, bool hasHeader = true, int skipRowWithoutHeader = 0, Dictionary<string, string> mappingColumn = null) where T : class, new()
        {
            var tbl = new DataTable();
            using (var pck = new ExcelPackage())
            {
                pck.Load(streamFile);
                var listWorkSheets = pck.Workbook.Worksheets.ToList();
                var ws = listWorkSheets.Count >= sheet + 1 ? listWorkSheets[sheet] : pck.Workbook.Worksheets.First();
                var countCol = ws.Dimension.End.Column;
                var maxCol = 0;
                foreach (var firstRowCell in ws.Cells[1 + skipRowWithoutHeader, 1, 1 + skipRowWithoutHeader, countCol])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    maxCol++;
                }
                var startRow = (hasHeader ? 2 : 1) + skipRowWithoutHeader;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, maxCol];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Value;
                    }
                    var isNull = true;
                    for (var i = 0; i < maxCol; i++)
                    {
                        if (row[i] != DBNull.Value && (row[i] + "").Trim() != "")
                            isNull = false;
                    }
                    if (!isNull)
                        tbl.Rows.Add(row);
                }
            }
            if (tbl.Rows.Count == 0)
            {
                tbl.Dispose();
                return new List<T>();
            }
            if (mappingColumn != null && mappingColumn.Count > 0)
            {
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    var clName = tbl.Columns[i].ColumnName;
                    if (mappingColumn.TryGetValue(clName, out string clReplace))
                        tbl.Columns[i].ColumnName = clReplace;
                }
            }
            var str = DataTableToJsonWithStringBuilderProcess(tbl);
            tbl.Dispose();
            var lst = System.Text.Json.JsonSerializer.Deserialize<List<T>>(str);
            return lst;
        }
    }
}
