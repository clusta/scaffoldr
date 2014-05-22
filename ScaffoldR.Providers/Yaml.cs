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
    public class Yaml : IYaml
    {
        public Task<T> Deserialize<T>(string data)
        {
            var input = new StringReader(data);
            var deserializer = new Deserializer(null, new CamelCaseNamingConvention(), true);
            var result = (T)deserializer.Deserialize(input, typeof(T));

            return Task.FromResult(result);
        }
    }
}
