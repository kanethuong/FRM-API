using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace kroniiapi.Helper
{
    public static class ExcelHelper
    {
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
        public static List<TData> ExportDataFromExcel<TData>(this Stream dataStream, Func<Dictionary<string, object>, TData> rowConverter, Predicate<List<string>> colNamesVerifier, out bool success, out string message)
        {
            using (var package = new ExcelPackage(dataStream))
            {
                return package.Workbook.Worksheets[0].ExportDataFromExcel(rowConverter, colNamesVerifier, out success, out message);
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
        public static List<TData> ExportDataFromExcel<TData>(this ExcelWorksheet worksheet, Func<Dictionary<string, object>, TData> rowConverter, Predicate<List<string>> colNamesVerifier, out bool success, out string message)
        {
            List<TData> list = new();
            worksheet.ExportDataFromExcel(dict =>
            {
                var data = rowConverter.Invoke(dict);
                if (data is not null)
                {
                    list.Add(data);
                }
            }, colNamesVerifier, out success, out message);
            return list;
        }

        /// <summary>
        /// Export the data from the excel worksheet as a list of dictionary
        /// </summary>
        /// <param name="worksheet">The excel worksheet</param>
        /// <param name="colNamesVerifier">The column names verifier, checking if the column names match user requirement</param>
        /// <param name="success">Whether the exporting process is successful</param>
        /// <param name="message">The output message</param>
        /// <returns>A list of dictionary</returns>
        public static List<Dictionary<string, object>> ExportDataFromExcel(this ExcelWorksheet worksheet, Predicate<List<string>> colNamesVerifier, out bool success, out string message)
        {
            return ExportDataFromExcel(worksheet, dict => new Dictionary<string, object>(dict), colNamesVerifier, out success, out message);
        }

        /// <summary>
        /// Export the data from the excel worksheet for consuming
        /// </summary>
        /// <param name="worksheet">The excel worksheet</param>
        /// <param name="rowConsumer">The consumer, do action on the cells dictionary</param>
        /// <param name="colNamesVerifier">The column names verifier, checking if the column names match user requirement</param>
        /// <param name="success">Whether the exporting process is successful</param>
        /// <param name="message">The output message</param>
        public static void ExportDataFromExcel(this ExcelWorksheet worksheet, Action<Dictionary<string, object>> rowConsumer, Predicate<List<string>> colNamesVerifier, out bool success, out string message)
        {
            // Get the dimension
            var rowCount = worksheet.Dimension.Rows;
            var colCount = worksheet.Dimension.Columns;

            // Get the column names
            List<string> colNames = new();
            for (int col = 1; col <= colCount; col++)
            {
                colNames.Add(worksheet.Cells[1, col].Value?.ToString().Trim());
            }

            // Verify the column names
            if (!colNamesVerifier.Invoke(colNames))
            {
                success = false;
                message = "Column names do not match";
                return;
            }

            // Loop through each rows from row 2
            for (int row = 2; row <= rowCount; row++)
            {
                // Create the cells dictionary
                Dictionary<string, object> cellsDict = new();

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
                    return;
                }

                // Consume the dictionary
                rowConsumer.Invoke(cellsDict);
            }

            // All successful
            message = "Success";
            success = true;
        }

        /// <summary>
        /// Fill the entity to the cells in the worksheet
        /// </summary>
        /// <typeparam name="TEntity">the type of the entity</typeparam>
        /// <param name="worksheet">the worksheet</param>
        /// <param name="list">the entities</param>
        /// <param name="action">the action to fill the entity to the cells</param>
        /// <param name="startRow">the row to start the action</param>
        /// <param name="startCol">the column to start the action</param>
        /// <param name="range">how many cells per row to be selected to be filled</param>
        /// <returns>the worksheet for continuous methods</returns>
        public static ExcelWorksheet FillDataToCells<TEntity>(this ExcelWorksheet worksheet, IEnumerable<TEntity> list, Action<TEntity, IList<ExcelRangeBase>> action, int startRow, int startCol, int range)
        {
            return FillDataToRange(worksheet, list, (entity, range) => {
                List<ExcelRangeBase> cellsList = new();
                int row = range.Start.Row;
                int startCol = range.Start.Column;
                int endCol = range.End.Column;
                for (int col = startCol; col <= endCol; col++)
                    cellsList.Add(worksheet.Cells[row, col]);
                action.Invoke(entity, cellsList);
            }, startRow, startCol, range);
        }

        /// <summary>
        /// Fill the entity to the cell range in the worksheet
        /// </summary>
        /// <typeparam name="TEntity">the type of the entity</typeparam>
        /// <param name="worksheet">the worksheet</param>
        /// <param name="list">the entities</param>
        /// <param name="action">the action to fill the entity to the cell range</param>
        /// <param name="startRow">the row to start the action</param>
        /// <param name="startCol">the column to start the action</param>
        /// <param name="range">how many cells per row to be selected to be filled</param>
        /// <returns>the worksheet for continuous methods</returns>
        public static ExcelWorksheet FillDataToRange<TEntity>(this ExcelWorksheet worksheet, IEnumerable<TEntity> list, Action<TEntity, ExcelRangeBase> action, int startRow, int startCol, int range)
        {
            int row = startRow;
            foreach (var entity in list)
            {
                action.Invoke(entity, worksheet.Cells[row, startCol, row, startCol + (range - 1)]);
                row++;
            }
            return worksheet;
        }
    }
}