using Microsoft.AspNetCore.Http;

using System;
using System.IO;
using System.Threading.Tasks;

namespace web.Handlers
{
    public static class FileStreamHandle
    {
        public static byte[] GetAsByteArray(string filePatch)
        {
            byte[] bytes = null;
            using (FileStream fsSource = new FileStream(filePatch, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
            }
            return bytes;
        }

        public static async Task<byte[]> GetAsByteArrayAsync(IFormFile theFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await theFile.CopyToAsync(memoryStream);
            byte[] bytesOfFile = memoryStream.ToArray();
            memoryStream.Dispose();
            return bytesOfFile;
        }
    }
}
