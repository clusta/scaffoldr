using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class FileSystemOutput : IFileOutput
    {
        private string basePath;
        
        public async Task SaveAsync(string path, string contentType, Stream inputStream)
        {
            var absolutePath = Path.Combine(basePath, path);

            using (var outputStream = File.Open(absolutePath, FileMode.Create))
            {
                await inputStream.CopyToAsync(outputStream);
            }
        }

        public FileSystemOutput(string basePath)
        {
            this.basePath = basePath;
        }
    }
}
