using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IContainer
    {
        IFileInput ResolveFileInput(string baseAddress);
        IFileOutput ResolveFileOutput(string baseAddress, string bucketName, string accessKey, string secretKey);
        IDeserializer ResolveDeserializer(string name, string contentType);
        IDataReader ResolveDataReader(string name, string contentType);
        ITextTemplate ResolveTextTemplate(string path);
        ILogger ResolveLogger();
        IIndexer ResolveIndexer();
    }
}
