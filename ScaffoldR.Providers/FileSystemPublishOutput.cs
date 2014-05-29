using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class FileSystemPublishOutput : IPublishOutput
    {
        private string basePath;
        
        public async Task SaveAsync(Stream inputStream, string path, string contentType)
        {
            var absolutePath = Path.Combine(basePath, path);

            using (var outputStream = File.Open(absolutePath, FileMode.Create))
            {
                await inputStream.CopyToAsync(outputStream);
            }
        }

        public FileSystemPublishOutput(string basePath)
        {
            this.basePath = basePath;
        }
    }
}
