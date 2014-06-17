using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class FileSystemInput : IFileInput
    {
        private string basePath;
        
        public Task<IEnumerable<string>> GetFoldersAsync(string containerName)
        {
            var path = Path.Combine(basePath, containerName);
            var folders = Directory.EnumerateDirectories(path);

            return Task.FromResult(folders);
        }

        public Task<IEnumerable<string>> GetFilesAsync(string path)
        {
            var finalPath = Path.IsPathRooted(path) ? path : Path.Combine(basePath, path);
            var files = Directory.EnumerateFiles(finalPath);
            
            return Task.FromResult(files);
        }

        public Task<Stream> OpenStreamAsync(string path)
        {
            var fileStream = (Stream)File.OpenRead(path);
            
            return Task.FromResult(fileStream);
        }

        public FileSystemInput(string basePath)
        {
            this.basePath = basePath;
        }
    }
}
