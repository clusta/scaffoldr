using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IDeserializer
    {
        T Deserialize<T>(Stream inputStream);
    }
}
