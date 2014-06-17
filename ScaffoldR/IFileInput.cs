using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IFileInput
    {
        Task<IEnumerable<string>> GetFoldersAsync(string uri);
        Task<IEnumerable<string>> GetFilesAsync(string uri);
        Task<Stream> OpenStreamAsync(string uri);
    }
}
