using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IFileDestination
    {
        Task SaveAsync(string path, string contentType, Stream inputStream);
    }
}
