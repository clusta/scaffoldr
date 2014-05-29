using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IPublishSource
    {
        Task<IEnumerable<string>> GetPagesAsync(string containerName);
        Task<IEnumerable<string>> GetPageSourcesAsync(string path);
        Task<Stream> OpenStreamAsync(string path);
    }
}
