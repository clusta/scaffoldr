using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface ISource
    {
        Task<Resource[]> GetFolders(string containerName);
        Task<Resource[]> GetFiles(string path);
        Task<Stream> OpenRead(string path);
    }
}
