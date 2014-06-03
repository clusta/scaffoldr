using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public interface IContainer
    {
        IFileSource ResolveFileSource(string baseAddress);
        IFileDestination ResolveFileDestination(string baseAddress, string bucketName, string accessKey, string secretKey);
        IDeserializer ResolveDeserializer(string name, string contentType);
        IDataReader ResolveDataReader(string name, string contentType);
        ITextTemplate ResolveTemplate(string path);
        ILogger ResolveLogger();
        IIndexer ResolveIndexer();
    }
}
