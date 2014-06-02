using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ScaffoldR.Providers
{
    public class YamlDotNetDeserializer : IDeserializer
    {
        public T Deserialize<T>(Stream inputStream)
        {
            using (var streamReader = new StreamReader(inputStream)) 
            {
                var deserializer = new Deserializer(null, new CamelCaseNamingConvention(), true);
                
                return (T)deserializer.Deserialize(streamReader, typeof(T));
            }
        }
    }
}
