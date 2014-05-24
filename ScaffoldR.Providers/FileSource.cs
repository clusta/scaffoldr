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
        
        public Task<Resource[]> GetFolders(string containerName)
        {
            var path = Path.Combine(basePath, containerName);
            var folders = Directory.EnumerateDirectories(path)
                .Select(d => new Resource()
                {
                    Path = d,
                    Name = Path.GetFileName(d)
                })
                .ToArray();

            return Task.FromResult(folders);
        }

        public Task<Resource[]> GetFiles(string path)
        {
            var finalPath = Path.IsPathRooted(path) ? path : Path.Combine(basePath, path);
            var files = Directory.EnumerateFiles(finalPath).Select(f => new Resource()
            {
                Path = f,
                Name = Path.GetFileName(f)
            })
            .ToArray();
            
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
