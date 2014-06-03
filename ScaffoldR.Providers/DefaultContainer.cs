﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class DefaultContainer : IContainer
    {
        private IDictionary<string, Type> mappings;
        
        public IFileSource ResolveFileSource(string baseAddress)
        {
            return new FileSystemSource(baseAddress);
        }

        public IFileDestination ResolveFileDestination(string baseAddress, string bucketName, string accessKey, string secretKey)
        {
            if (IsUri(baseAddress) && baseAddress.Contains(".amazonaws.com"))
            {
                return new AmazonS3Destination(baseAddress, bucketName, accessKey, secretKey);
            }
            else
            {
                return new FileSystemDestination(baseAddress);
            }
        }

        public IDeserializer ResolveDeserializer(string name, string contentType)
        {
            switch (contentType)
            {
                case Constants.ContentType.Json:
                    return new JsonNetDeserializer();
                case Constants.ContentType.Yaml:
                    return new YamlDotNetDeserializer();
                default:
                    return null;
            }
        }

        public IDataReader ResolveDataReader(string name, string contentType)
        {
            if (contentType.Equals(Constants.ContentType.Csv) && mappings.ContainsKey(name))
            {
                var type = mappings[name];
                
                return new FileHelpersDataReader(type);
            }
            else
            {
                return null;
            }
        }

        private bool IsUri(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute)
                && (new Uri(uri).Scheme == Uri.UriSchemeHttp || new Uri(uri).Scheme == Uri.UriSchemeHttps);
        }

        public ITextTemplate ResolveTemplate(string baseAddress)
        {
            if (IsUri(baseAddress))
            {
                return new RemoteTextTemplate(baseAddress);
            }
            else
            {
                return new MustacheTextTemplate(baseAddress);
            }
        }

        public ILogger ResolveLogger()
        {
            return new DebugLogger();
        }

        public IIndexer ResolveIndexer()
        {
            return new DefaultIndexer();
        }

        public DefaultContainer(IDictionary<string, Type> mappings)
        {
            this.mappings = mappings;
        }
    }
}