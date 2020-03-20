using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BetterRead.Shared.Services
{
    public class DownloadService : IDownloadService
    {
        private static readonly string ImgPath;

        static DownloadService()
        {
            ImgPath = Directory.GetCurrentDirectory() + "\\Img";
        }
        

        public string DownloadFile(string url, string fileName, bool isAsync = false)
        {
            if (!Directory.Exists(ImgPath))
                Directory.CreateDirectory(ImgPath);

            if (new DirectoryInfo(ImgPath).GetFiles().Any(f => f.Name.Contains(fileName)))
                return $"{ImgPath}\\{fileName}.jpg";

            using (WebClient client = new WebClient())
            {
                client.DownloadFileAsync(new Uri(url), $"Img/{fileName}.jpg");
            }

            var b = StoreFile(url);
            
            return $"{ImgPath}\\{fileName}.jpg";
        }

        public byte[] StoreFile(string url)
        {
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (WebClient client = new WebClient())
                {
                    bytes = client.DownloadData(new Uri(url));
                }

                var tw = new StreamWriter(ms);
                tw.Write(bytes);
                tw.Flush();
            }

            return bytes;
        }
    }

    public interface IDownloadService
    {
        string DownloadFile(string url, string fileName, bool isAsync = false);
    }
}