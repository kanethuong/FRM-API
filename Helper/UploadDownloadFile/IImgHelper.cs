using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.Helper.UploadDownloadFile
{
    public interface IImgHelper
    {
        Task<string> Upload(Stream stream, string fileName, long fileLength, string fileType);
    }
}