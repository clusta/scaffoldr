using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class StaticSiteGenerator
    {
        private ISource source;
        private IOutput output;
        private IYaml yaml;
        private IJson json;
        private ICsv csv; // todo: implement
        private IIndexer indexer;
        private ILogger logger; // todo: implement

        private static IDictionary<string, string> mediaContentTypes = new Dictionary<string, string>()
        {
            { "jpg", "image/jpg" },
            { "png", "image/png" }
        };

        private static IDictionary<string, string> sectionContentTypes = new Dictionary<string, string>()
        {
            { "html", "text/html" },
            { "md", "text/x-markdown" }
        };

        private static IDictionary<string, string> metadataContentTypes = new Dictionary<string, string>()
        {
            { "yaml", "text/x-yaml" },
	        { "json", "application/json" }
        };

        public Task<Page<MetaData>> ParsePage(string path)
        {
            return ParsePage<MetaData>(path);
        }

        public async Task<Page<TMetaData>> ParsePage<TMetaData>(string path)
        {
            var files = await source.GetFiles(path);           
            
            return new Page<TMetaData>()
            {
                Slug = Path.GetFileName(path),
                MetaData = files
                    .Where(f => GetFileNameWithoutExtension(f.Path) == "metadata" && IsMetaData(f.Path))
                    .Select(f => ParseMetaData<TMetaData>(f.Path).Result) // todo: make async
                    .FirstOrDefault(),
                Sections = files  
                    .OrderBy(f => GetSortOrder(f.Name))
                    .GroupBy(f => GetSectionName(f.Name))
                    .ToDictionary(g => g.Key, g => new Section() 
                    {
                        Content = g
                            .Where(m => IsSection(m.Name))
                            .Select(m => GetFileContent(m.Path).Result) // todo: make async
                            .FirstOrDefault(),
                        Media = g
                            .Where(m => IsMedia(m.Path))
                            .Select(m => new Media() 
                            {
                                Uri = GetFileName(m.Name),
                                ContentType = mediaContentTypes[GetExtension(m.Path)]
                            })
                            .ToList()
                    })
            };
        }

        public Task RenderFolder(string path, ITemplate template)
        {
            return PublishContainer<MetaData>(path, template);
        }

        public async Task PublishContainer<TMetaData>(string containerName, ITemplate template)
        {
            var folders = await source.GetFolders(containerName);

            foreach (var folder in folders)
            {
                var page = await ParsePage<TMetaData>(folder.Path);

                if (indexer != null)
                {
                    await indexer.AddOrUpdate(page);
                }
                
                using (var outputStream = await output.OpenWriteableStream(page.Slug)) 
                {
                    await template.RenderPage(outputStream, page);
                }
            }
        }

        private Task<string> GetFileContent(string path)
        {
            return source.ReadAsString(path);
        }

        private async Task<TMetaData> ParseMetaData<TMetaData>(string path)
        {
            var fileContent = await GetFileContent(path);
            var extension = GetExtension(path);

            switch (extension)
            {
                case "yaml":
                    return await yaml.Deserialize<TMetaData>(fileContent);
                case "json":
                    return await json.Deserialize<TMetaData>(fileContent);
                default:
                    return default(TMetaData);
            }
        }

        private string GetFileName(string path)
        {
            return Path.GetFileName(path)
                .ToLower();
        }

        private string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path)
                .ToLower();
        }

        private string GetSectionName(string path)
        {
            return Path.GetFileNameWithoutExtension(path)
                .Split('-')
                .Select(n => n.ToLower())
                .First();
        }

        private string GetExtension(string path)
        {
            return Path.GetExtension(path)
                .ToLower()
                .TrimStart('.');
        }

        private string GetSortOrder(string path)
        {
            return Path.GetFileNameWithoutExtension(path)
                .Split('-')
                .Skip(1)
                .Select(n => n.ToLower())
                .FirstOrDefault();
        }

        private bool IsMedia(string path)
        {
            return mediaContentTypes.ContainsKey(GetExtension(path));
        }

        private bool IsSection(string path)
        {
            return sectionContentTypes.ContainsKey(GetExtension(path));
        }

        private bool IsMetaData(string path)
        {
            return metadataContentTypes.ContainsKey(GetExtension(path));
        }

        public StaticSiteGenerator(ISource source, IOutput output, IYaml yaml, IJson json, ICsv csv, IIndexer indexer, ILogger logger)
        {
            this.source = source;
            this.output = output;
            this.yaml = yaml;
            this.json = json;
            this.csv = csv;
            this.indexer = indexer;
        }
    }
}
