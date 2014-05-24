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
        Task<IEnumerable<string>> GetFolders(string containerName);
        Task<IEnumerable<string>> GetFiles(string path);
        Task<Stream> OpenRead(string path);
    }
}
