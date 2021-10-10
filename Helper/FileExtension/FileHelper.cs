using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace kroniiapi.Helper
{
    public class FileHepler
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
    }
}