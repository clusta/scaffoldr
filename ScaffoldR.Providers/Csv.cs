using FileHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class Csv : ICsv
    {
        private IDictionary<string, Type> typeMappings;
        
        public Task<object[]> Deserialize(string key, Stream inputStream)
        {
            using (var textReader = new StreamReader(inputStream))
            {
                var fileHelper = new FileHelperEngine(typeMappings[key]);
                var rows = fileHelper.ReadStream(textReader);
            
                return Task.FromResult(rows);
            }
        }

        public Csv(IDictionary<string, Type> typeMappings)
        {
            this.typeMappings = typeMappings;
        }
    }
}
