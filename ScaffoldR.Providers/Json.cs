using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class Json : IJson
    {
        public Task<T> Deserialize<T>(Stream inputStream)
        {
            var jsonSerializer = new JsonSerializer();

            using (var streamReader = new StreamReader(inputStream))
            using (var jsonReader = new JsonTextReader(streamReader)) 
            {
                var model = jsonSerializer.Deserialize<T>(jsonReader);
                
                return Task.FromResult(model);
            }
        }
    }
}
