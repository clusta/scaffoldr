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
    public class FileHelpersCsvDeserializer : ICsvDeserializer
    {
        private IDictionary<string, Type> typeMappings;
        
        public object[] Deserialize(string key, Stream inputStream)
        {
            using (var textReader = new StreamReader(inputStream))
            {
                var fileHelper = new FileHelperEngine(typeMappings[key]);

                return fileHelper.ReadStream(textReader);
            }
        }

        public FileHelpersCsvDeserializer(IDictionary<string, Type> typeMappings)
        {
            this.typeMappings = typeMappings;
        }
    }
}
