using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.Helper.Upload
{
    public interface IMegaHelper
    {
        Task<String> Upload(Stream data, string fileName, string folderName);
        Task<Stream> Download(Uri uri);
    }
}