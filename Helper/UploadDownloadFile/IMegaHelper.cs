using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.Helper.Upload
{
    public interface IMegaHelper
    {
        Task<Uri> Upload(Stream data, string fileName, string folderName);
<<<<<<< HEAD
=======
        Task<Stream> Download(Uri uri);
>>>>>>> caa6815fd03229c6ce59ff85216f2fff7fba472d
    }
}