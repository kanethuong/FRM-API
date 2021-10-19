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
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(file.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
            {
                return Tuple.Create(false, "Not support file extension");
            }

            // Check MIME type is XLSX
            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);

            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);

            if (!mimeType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") && !mimeType.Equals("application/vnd.ms-excel"))
            {
                return Tuple.Create(false, "Not support fake extension");
            }

            return Tuple.Create(true, "");
        }
        /// <summary>
        /// Check a file is has .doc or .docx or not
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true / false + error_message</returns>
        public static Tuple<bool, string> CheckDocExtension(IFormFile file)
        {
            // Check file length
            if (file == null || file.Length <= 0)
            {
                return Tuple.Create(false, "No upload file");
            }

            // Check file extension
            if ((!Path.GetExtension(file.FileName).Equals(".doc", StringComparison.OrdinalIgnoreCase)) && (!Path.GetExtension(file.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase)))
            {
                return Tuple.Create(false, "Not support file extension");
            }

            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);

            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);

            // Check MIME type is Docx
            if (!mimeType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
            {
                return Tuple.Create(false, "Not support fake extension");
            }
            // Check MIME type is Doc
            if (!mimeType.Equals("application/msword"))
            {
                return Tuple.Create(false, "Not support fake extension");
            }

            return Tuple.Create(true, "");
        }
        /// <summary>
        /// Check a file is has .png or .jpeg or .jpg or not
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true / false + error_message</returns>
        public static Tuple<bool, string> CheckImageExtension(IFormFile file)
        {
            // Check file length
            if (file == null || file.Length <= 0)
            {
                return Tuple.Create(false, "No upload file");
            }

            // Check file extension
            if ((!Path.GetExtension(file.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase)) && (!Path.GetExtension(file.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase)) && (!Path.GetExtension(file.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase)))
            {
                return Tuple.Create(false, "Not support file extension");
            }
            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);

            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);

            if ((!mimeType.Equals("image/jpeg")) && (!mimeType.Equals("image/jpeg")) && (!mimeType.Equals("image/png")))
            {
                return Tuple.Create(false, "Not support fake extension");
            }

            return Tuple.Create(true, "");
        }
        /// <summary>
        /// Check a file is has .pdf not
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Tuple<bool, string> CheckPDFExtension(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return Tuple.Create(false, "No upload file");
            }

            // Check file extension
            if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Tuple.Create(false, "Not support file extension");
            }

            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);

            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);

            if (!mimeType.Equals("application/pdf"))
            {
                return Tuple.Create(false, "Not support fake extension");
            }

            return Tuple.Create(true, "");
        }

        /// <summary>
        /// Export the data from the fisrt worksheet of the excel file
        /// </summary>
        /// <param name="dataStream">The file stream of the excel file</param>
        /// <param name="rowConverter">The converter, convert the cells dictionary to the final data</param>
        /// <param name="colNamesVerifier">The column names verifier, checking if the column names match user requirement</param>
        /// <param name="success">Whether the exporting process is successful</param>
        /// <param name="message">The output message</param>
        /// <typeparam name="TData">The final data the converter is trying to convert to</typeparam>
        /// <returns>A list of the converted data</returns>
        public static List<TData> ExportDataFromExcel<TData>(Stream dataStream, Func<Dictionary<string, object>, TData> rowConverter, Predicate<List<string>> colNamesVerifier, out bool success, out string message)
        {
            using (var package = new ExcelPackage(dataStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                return ExportDataFromExcel<TData>(worksheet, rowConverter, colNamesVerifier, out success, out message);
            }
        }

        /// <summary>
        /// Export the data from the excel worksheet
        /// </summary>
        /// <param name="worksheet">The excel worksheet</param>
        /// <param name="rowConverter">The converter, convert the cells dictionary to the final data. If the final data is null, It will not be added to the final list</param>
        /// <param name="colNamesVerifier">The column names verifier, checking if the column names match user requirement</param>
        /// <param name="success">Whether the exporting process is successful</param>
        /// <param name="message">The output message</param>
        /// <typeparam name="TData">The final data the converter is trying to convert to</typeparam>
        /// <returns>A list of the converted data</returns>
        public static List<TData> ExportDataFromExcel<TData>(ExcelWorksheet worksheet, Func<Dictionary<string, object>, TData> rowConverter, Predicate<List<string>> colNamesVerifier, out bool success, out string message)
        {
            List<TData> list = new List<TData>();
            ExportDataFromExcel(worksheet, dict => {
                var data = rowConverter.Invoke(dict);
                if (data is not null) {
                    list.Add(data);
                }
            }, colNamesVerifier, out success, out message);
            return list;
        }

        /// <summary>
        /// Export the data from the excel worksheet for consuming
        /// </summary>
        /// <param name="worksheet">The excel worksheet</param>
        /// <param name="rowConsumer">The consumer, do action on the cells dictionary</param>
        /// <param name="colNamesVerifier">The column names verifier, checking if the column names match user requirement</param>
        /// <param name="success">Whether the exporting process is successful</param>
        /// <param name="message">The output message</param>
        public static void ExportDataFromExcel(ExcelWorksheet worksheet, Action<Dictionary<string, object>> rowConsumer, Predicate<List<string>> colNamesVerifier, out bool success, out string message)
        {
            // Get the dimension
            var rowCount = worksheet.Dimension.Rows;
            var colCount = worksheet.Dimension.Columns;

            // Get the column names
            List<string> colNames = new List<string>();
            for (int col = 1; col <= colCount; col++)
            {
                colNames.Add(worksheet.Cells[1, col].Value.ToString().Trim());
            }

            // Verify the column names
            if (!colNamesVerifier.Invoke(colNames))
            {
                success = false;
                message = "Column names do not match";
            }
            else
            {
                success = true;
            }

            // Create the cells dictionary
            Dictionary<string, object> cellsDict = new Dictionary<string, object>();

            // Loop through each rows from row 2
            for (int row = 2; row <= rowCount; row++)
            {
                // Keep the state if fail to add
                bool failToAdd = false;

                // Add the cells from the row to the dictionary
                for (int col = 1; col <= colCount; col++)
                {
                    if (!cellsDict.TryAdd(colNames[col - 1], worksheet.Cells[row, col].Value))
                    {
                        failToAdd = true;
                        break;
                    }
                }

                // Check if fail to add
                if (failToAdd)
                {
                    message = "Fail to convert value on row " + row;
                    success = false;
                    break;
                }

                // Consume the dictionary
                rowConsumer.Invoke(cellsDict);

                // Clear the dictionary for next row
                cellsDict.Clear();
            }

            // All successful
            message = "Success";
        }
    }
}
