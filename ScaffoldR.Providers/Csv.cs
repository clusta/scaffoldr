using FileHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class Csv : ICsv
    {
        private IDictionary<string, Type> typeMappings;
        
        public Task<object[]> Deserialize(string key, string data)
        {
            var fileHelper = new FileHelperEngine(typeMappings[key]);
            var rows = fileHelper.ReadString(data);
            
            return Task.FromResult(rows);
        }

        public Csv(IDictionary<string, Type> typeMappings)
        {
            this.typeMappings = typeMappings;
        }
    }
}
