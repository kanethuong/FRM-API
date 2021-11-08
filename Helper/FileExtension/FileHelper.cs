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
        public static Tuple<bool, string> CheckExcelExtension(IFormFile file) => CheckFileExtension(
            file,
                new[] {
                    ".xlsx", ".xls"
                },
                new[] {
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "application/vnd.ms-excel"
                }
            );

        /// <summary>
        /// Check a file is has .doc or .docx or not
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true / false + error_message</returns>
        public static Tuple<bool, string> CheckDocExtension(IFormFile file) => CheckFileExtension(file,
                new[] {
                    ".doc", ".docx"
                },
                new[] {
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "application/msword"
                }
            );

        /// <summary>
        /// Check a file is has .png or .jpeg or .jpg or not
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true / false + error_message</returns>
        public static Tuple<bool, string> CheckImageExtension(IFormFile file) => CheckFileExtension(file,
                new[] {
                    ".jpeg", ".png", ".jpg"
                },
                new[] {
                    "image/jpeg", "image/png"
                }
            );

        /// <summary>
        /// Check a file is has .pdf not
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Tuple<bool, string> CheckPDFExtension(IFormFile file) => CheckFileExtension(file, new[] { ".pdf" }, new[] { "application/pdf" });

        /// <summary>
        /// Validate the file extension
        /// </summary>
        /// <param name="file">the file</param>
        /// <param name="fileTypes">the extension types</param>
        /// <param name="mimeTypes">the mime types</param>
        /// <returns>a tuple contains the validate status (boolean) and the message</returns>
        public static Tuple<bool, string> CheckFileExtension(IFormFile file, string[] fileTypes, string[] mimeTypes)
        {
            if (file == null || file.Length <= 0)
            {
                return Tuple.Create(false, "No upload file");
            }

            bool success;

            // Check file extension
            var fileType = Path.GetExtension(file.FileName);
            success = false;
            foreach (var type in fileTypes)
            {
                if (fileType.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    success = true;
                    break;
                }
            }
            if (!success)
            {
                return Tuple.Create(false, "Not support file extension");
            }

            // Check MIME type
            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);
            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);
            success = false;
            foreach (var type in mimeTypes)
            {
                if (mimeType.Equals(type))
                {
                    success = true;
                    break;
                }
            }
            if (!success)
            {
                return Tuple.Create(false, "Not support fake extension");
            }

            return Tuple.Create(true, "Validated");
        }
    }
}
