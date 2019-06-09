using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace AiTools.BLL.Providers
{
    public class FileProvider
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public FileProvider(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public string GetFullPath(string virtualPath)
        {
            return Path.Combine(hostingEnvironment.ContentRootPath, virtualPath);
        }
        public async Task<string> SaveFileCompressedAsync(string path, string content, bool virtualPath = true)
        {
            if(virtualPath)
                path = Path.Combine(hostingEnvironment.ContentRootPath, path);
            using(var targetStream = File.Create(path))
            {
                using(var compressStream = new GZipStream(targetStream, CompressionLevel.Optimal))
                {
                    await compressStream.WriteAsync(Encoding.UTF8.GetBytes(content));
                    return path;
                }
            }
        }
        public async Task<string> GetCompressedDataAsync(string path, bool virtualPath = true)
        {
            if (virtualPath)
                path = Path.Combine(hostingEnvironment.ContentRootPath, path);
            using(var srcStream = File.OpenRead(path))
            {
                using(var mStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(srcStream, CompressionMode.Decompress))
                    {
                        await gzipStream.CopyToAsync(mStream);
                        return Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
        }
    }
}
