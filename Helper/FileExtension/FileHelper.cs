using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace kroniiapi.Helper
{
    public static class FileHelper
    {
        /// <summary>
        /// Check a file is has .xlsx extension or not
        /// </summary>
        /// <param name="file">A file want to check extension</param>
        /// <returns>true / false + error_message</returns>
        public static Tuple<bool, string> CheckExcelExtension(IFormFile file)
        {
            // Check file length
            if (file == null || file.Length <= 0)
            {
                return Tuple.Create(false, "No upload file");
            }

            // Check file extension
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return Tuple.Create(false, "Not support file extension");
            }

            // Check MIME type is XLSX
            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);

            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);

            if (!mimeType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                return Tuple.Create(false, "Not support fake extension");
            }

            return Tuple.Create(true, "");
        }

        /// <summary>
        /// Export the data from the excel file.
        /// This convenient method will first convert the rows of the file to a dictionary called "cells dictionary".
        /// Then, It calls the converter to convert the dictionary to the final data.
        /// </summary>
        /// <param name="dataStream">The file stream of the excel file</param>
        /// <param name="rowConverter">The converter, convert the cells dictionary to the final data</param>
        /// <typeparam name="TData">The final data the converter is trying to convert to</typeparam>
        /// <returns>A list of the converted data</returns>
        public static List<TData> ExportDataFromExcel<TData>(Stream dataStream, Func<Dictionary<string, object>, TData> rowConverter)
        {
            // Create the final list
            List<TData> list = new List<TData>();

            // Start the excel package wrapper
            using (var package = new ExcelPackage(dataStream))
            {
                // Only use the first worksheet of the excel file
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Get the dimension
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                // Get the column names
                List<string> colNames = new List<string>();
                for (int col = 1; col <= colCount; col++) {
                    colNames.Add(worksheet.Cells[1, col].Value.ToString().Trim());
                }

                // Create the cells dictionary
                Dictionary<string, object> cellsDict = new Dictionary<string, object>();

                // Loop through each rows from row 2
                for (int row = 2; row <= rowCount; row++)
                {
                    // Add the cells from the row to the dictionary
                    for (int col = 1; col <= colCount; col++) {
                        cellsDict.TryAdd(colNames[col - 1], worksheet.Cells[row, col].Value);
                    }

                    // Convert the dictionary and add to the final list
                    list.Add(rowConverter.Invoke(cellsDict));

                    // Clear the dictionary for next row
                    cellsDict.Clear();
                }
            }

            // Return the final list
            return list;
        }
    }
}