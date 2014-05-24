using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class FileSource : ISource
    {
        private string basePath;
        
        public Task<IEnumerable<string>> GetFolders(string containerName)
        {
            var path = Path.Combine(basePath, containerName);
            var folders = Directory.EnumerateDirectories(path);

            return Task.FromResult(folders);
        }

        public Task<IEnumerable<string>> GetFiles(string path)
        {
            var finalPath = Path.IsPathRooted(path) ? path : Path.Combine(basePath, path);
            var files = Directory.EnumerateFiles(finalPath);
            
            return Task.FromResult(files);
        }

        public Task<Stream> OpenRead(string path)
        {
            var fileStream = (Stream)File.OpenRead(path);
            
            return Task.FromResult(fileStream);
        }

        public FileSource(string basePath)
        {
            this.basePath = basePath;
        }
    }
}
