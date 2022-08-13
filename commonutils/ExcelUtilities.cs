using AutomationTest.reporting.serilog;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AutomationTest.commonutils
{
    public class ExcelUtilities
    {
        private static readonly ILogger logger = LoggerConfig.Logger;

        public String eos;

        private ExcelPackage GetExcelPackage(string WorkBookPath)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            logger.Here().Information("WorkBookPath: " + WorkBookPath);
            FileInfo fr = new FileInfo(WorkBookPath);
            ExcelPackage File = new ExcelPackage(fr);

            return File;

        }

        private ExcelWorksheet GetWorksheet(ExcelPackage file, String sheetName)
        {
            logger.Here().Information("Sheet Name: " + sheetName);
            try
            {
                if (file.Workbook.Worksheets[sheetName] == null)
                {
                    return file.Workbook.Worksheets.Add(sheetName);
                }
                else
                    return file.Workbook.Worksheets[sheetName];
            }
            catch (Exception)
            {
                return file.Workbook.Worksheets.Add(sheetName);
            }
        }


        private List<List<String>> coreFetch(string workbookPath, string sheetName)
        {
            ExcelPackage inputFile = GetExcelPackage(workbookPath);

            ExcelWorksheet excelWorksheet = inputFile.Workbook.Worksheets[sheetName];

            String CellValue;
            List<List<String>> sheetData = new List<List<string>>();

            for (int r = 1; r <= excelWorksheet.Dimension.Rows; r++)
            {
                List<String> row = new List<string>();
                for (int c = 1; c <= excelWorksheet.Dimension.Columns; c++)
                {
                    try
                    {
                        CellValue = excelWorksheet.Cells[r, c].Text;
                        if (CellValue.Equals("n\\\\a") || CellValue.Equals("n\\a"))
                        {
                            CellValue = "";
                        }
                    }
                    catch (Exception)
                    {
                        CellValue = "";
                    }

                    row.Add(CellValue);
                }
                sheetData.Add(row);
            }

            return sheetData;

        }

        public Dictionary<String, List<String>> ReadWorkSheet(string workbookPath, string sheetName)
        {
            var workSheetData = coreFetch(workbookPath, sheetName);

            List<String> headers = workSheetData[0];
            Dictionary<String, List<String>> workSheetDataNew = new Dictionary<string, List<string>>();

            for (int row = 1; row < workSheetData.Count; row++)
            {
                for (int col = 0; col < workSheetData[row].Count; col++)
                {
                    if (workSheetDataNew.ContainsKey(headers[col]))
                        workSheetDataNew[headers[col]].Add(workSheetData[row][col]);
                    else
                    {
                        workSheetDataNew.Add(headers[col], new List<string>() { workSheetData[row][col] });
                    }
                }

            }

            return workSheetDataNew;

        }

        public void AddRowsInSheet(string workbookPath, string sheetName, List<List<String>> rows)
        {
            ExcelPackage file = GetExcelPackage(workbookPath);
            ExcelWorksheet excelWorksheet = GetWorksheet(file, sheetName);



            try
            {
                foreach (var row in rows)
                {
                    int LastRowIndex;
                    try { LastRowIndex = excelWorksheet.Dimension.Rows; } catch (Exception) { LastRowIndex = 0; }
                    LastRowIndex++;
                    for (int col = 0; col < row.Count; col++)
                    {
                        excelWorksheet.Column(col + 1).AutoFit(20, 40);
                        excelWorksheet.Cells[LastRowIndex, col + 1].Value = row[col];
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                file.Save();
                file.Dispose();
            }
        }

        public void AddRowsInSheet(string workbookPath, string sheetName, Dictionary<String, List<String>> rows)
        {
            ExcelPackage file = GetExcelPackage(workbookPath);

            ExcelWorksheet excelWorksheet = GetWorksheet(file, sheetName);


            try
            {
                var headers = SetColumnHeaders(rows.Keys.ToHashSet(), excelWorksheet);

                int LastRowIndex = excelWorksheet.Dimension.Rows;

                for (int r = 0; r < rows.Values.ElementAt(0).Count; r++)
                {
                    foreach (string h in rows.Keys)
                    {
                        excelWorksheet.Cells[r + LastRowIndex + 1, headers[h]].Value = rows[h][r];
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Here().Error($"Error while Adding Rows in WorkSheet {workbookPath}->{sheetName} : " + ex);
                throw;
            }
            finally
            {
                file.Save();
            }

        }

        Dictionary<string, int> SetColumnHeaders(HashSet<String> ColumnHeaders, ExcelWorksheet excelWorksheet)
        {
            Dictionary<String, int> headers = new Dictionary<string, int>();

            int lastColumnIndex = 0;
            try
            {
                lastColumnIndex = excelWorksheet.Dimension.Columns;
            }
            catch (Exception)
            {

            }

            for (int c = 1; c <= lastColumnIndex; c++)
            {
                excelWorksheet.Column(c + 1).AutoFit(20, 40);
                headers.Add(excelWorksheet.Cells[1, c].Text, c);
            }
            foreach (string h in ColumnHeaders)
            {
                if (!headers.ContainsKey(h))
                {
                    lastColumnIndex++;
                    excelWorksheet.Cells[1, lastColumnIndex].Value = h;
                    headers.Add(h, lastColumnIndex);
                }
            }
            return headers;
        }


        public Dictionary<String, List<String>> fetchWithCondition(String sheetPath, String sheetName, List<String> whereClause)
        {
            eos = "EndOfScript";
            Dictionary<String, List<String>> excelMap = null;
            try
            {


                excelMap = ReadWorkSheet(sheetPath, sheetName);

                foreach (String clause in whereClause)
                {
                    logger.Here().Information("Where Clause = " + clause);
                    Dictionary<String, List<String>> finalMap = new Dictionary<String, List<String>>();
                    List<int> addIndex = new List<int>();
                    foreach (String entry in excelMap.Keys)
                    {
                        int k = 0;

                        if (entry.Equals(clause.Split(new string[] { "::" }, StringSplitOptions.None)[0], StringComparison.InvariantCultureIgnoreCase))
                        {
                            List<String> vals = new List<String>();
                            foreach (String val in new List<String>(excelMap[entry]))
                            {
                                //if ((!(val.equalsIgnoreCase(eos)))&&(!(clause.split("::")[1]).isEmpty())){
                                if (val.Equals(clause.Split(new string[] { "::" }, StringSplitOptions.None)[1], StringComparison.InvariantCultureIgnoreCase))
                                {
                                    vals.Add(val);
                                    addIndex.Add(k);
                                }
                                k++;
                                //} 
                                //	else{
                                //	System.out.println("cursor here");
                                //	break;
                                //}

                            }
                            if (finalMap.ContainsKey(entry) == false)
                            {
                                finalMap.Add(entry, vals);
                            }

                            else
                            {
                                finalMap[entry] = vals;
                            }
                        }
                    }

                    foreach (string entry in excelMap.Keys)
                    {
                        List<String> vals = new List<string>();

                        if (!entry.Equals(clause.Split(new string[] { "::" }, StringSplitOptions.None)[0], StringComparison.InvariantCultureIgnoreCase))
                        {
                            foreach (int add in addIndex)
                                //foreach (string value in excelMap[entry])
                                //vals.add(entry.getValue().get(add));
                                vals.Add(excelMap[entry][add]);
                            if (finalMap.ContainsKey(entry) == false)
                            {
                                finalMap.Add(entry, vals);
                            }

                            else
                            {
                                finalMap[entry] = vals;
                            }
                        }
                    }
                    excelMap = finalMap;
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); logger.Error(e.Message);
                logger.Here().Error(e.StackTrace);
                logger.Here().Error(e.ToString());
                throw e;

            }

            return excelMap;

        }

    }
}
