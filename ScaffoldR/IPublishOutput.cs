using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IPublishOutput
    {
        Task SaveAsync(Stream inputStream, string path, string contentType);
    }
}
