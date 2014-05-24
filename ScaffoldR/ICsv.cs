using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface ICsv
    {
        Task<object[]> Deserialize(string key, Stream inputStream);
    }
}
