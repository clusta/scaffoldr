using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class Json : IJson
    {
        public Task<T> Deserialize<T>(string data)
        {
            var result = JsonConvert.DeserializeObject<T>(data);

            return Task.FromResult(result);
        }
    }
}
