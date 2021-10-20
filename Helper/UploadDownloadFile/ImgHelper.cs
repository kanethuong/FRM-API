using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace kroniiapi.Helper.UploadDownloadFile
{
    public class ImgHelper : IImgHelper
    {
        private readonly string apiUrl;
        public ImgHelper()
        {
            this.apiUrl="https://api.imgbb.com/1/upload?key=bdf3f4c6ec67dc7b008c3a1ad1cc5651";
        }

         public async Task<string> Upload(Stream stream, string fileName, long fileLength, string fileType)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.apiUrl);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(stream)
                    {
                        Headers =
                    {
                        ContentLength = fileLength,
                        ContentType = new MediaTypeHeaderValue(fileType)
                    }
                    }, "image", fileName);

                    var response = await client.PostAsync(this.apiUrl, content);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic dec = JsonConvert.DeserializeObject(responseBody);
                    string url=dec.data.url;
                    return url;
                }
            }
        }
    }
}