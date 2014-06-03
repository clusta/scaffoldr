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
    public class FileHelpersDataReader : IDataReader
    {
        private Type type;
        
        public object[] ReadData(Stream inputStream)
        {
            using (var textReader = new StreamReader(inputStream))
            {
                var fileHelper = new FileHelperEngine(type);

                return fileHelper.ReadStream(textReader);
            }
        }

        public FileHelpersDataReader(Type type)
        {
            this.type = type;
        }
    }
}
