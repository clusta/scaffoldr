using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class FileOutput : IOutput
    {
        private string basePath;
        
        public Task<Stream> OpenWrite(string path)
        {
            var absolutePath = Path.Combine(basePath, path);
            var fileStream = (Stream)File.Open(absolutePath, FileMode.Create);
            
            return Task.FromResult(fileStream);
        }

        public FileOutput(string basePath)
        {
            this.basePath = basePath;
        }
    }
}
