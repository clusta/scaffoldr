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
            var files = Directory.EnumerateFiles(path).Select(f => new Resource()
            {
                Path = f,
                Name = Path.GetFileName(f)
            })
            .ToArray();
            
            return Task.FromResult(files);
        }

        public Task<string> ReadAsString(string path)
        {
            var text = File.ReadAllText(path);
            
            return Task.FromResult(text);
        }

        public FileSource(string basePath)
        {
            this.basePath = basePath;
        }
    }
}
