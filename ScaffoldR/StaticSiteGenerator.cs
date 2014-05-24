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
        private ICsv csv;
        private IIndexer indexer;
        private ILogger logger;

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
            
            var page = new Page<TMetaData>()
            {
                Slug = GetSlug(path)
            };

            var metaDataPath = files
                    .Where(f => GetFileNameWithoutExtension(f.Path) == "metadata" && IsMetaData(f.Path))
                    .Select(f => f.Path)
                    .FirstOrDefault();

            if (metaDataPath != null) 
            {
                page.MetaData = await ParseMetaData<TMetaData>(metaDataPath);
            }

            page.Sections = files
                .Where(f => IsSectionName(f.Name))
                .OrderBy(f => GetSortOrder(f.Name))
                .GroupBy(f => GetSectionName(f.Name))
                .ToDictionary(g => g.Key, g => new Section()
                {
                    Source = g
                        .Where(m => IsSection(m.Name))
                        .Select(m => m.Path)
                        .FirstOrDefault(),
                    Media = g
                        .Where(m => IsMedia(m.Path))
                        .Select(m => new Media()
                        {
                            Uri = GetFileName(m.Name),
                            ContentType = mediaContentTypes[GetExtension(m.Path)]
                        })
                        .ToList()
                });

            foreach (var section in page.Sections.Values.Where(s => !string.IsNullOrEmpty(s.Source)))
            {
                section.Content = await GetFileContent(section.Source);
            }

            return page;
        }

        public Task RenderFolder(string path, ITemplate template)
        {
            return PublishContainer<MetaData>(path, template);
        }

        public async Task PublishContainer<TMetaData>(string containerName, ITemplate template)
        {
            var folders = await source.GetFolders(containerName);

            Dictionary<string, object> datasources;

            try
            {
                datasources = await GetDatasources(containerName);
            }
            catch
            {
                logger.Log("Error: failed to parse datasources");
                
                return;
            }

            Page<TMetaData> page = null;

            foreach (var folder in folders)
            {
                try
                {
                    page = await ParsePage<TMetaData>(folder.Path);

                    page.Datasources = datasources;
                }
                catch
                {
                    logger.Log(string.Format("Error: failed to parse page '{0}'", folder.Path));

                    continue; // skip publish and index, other steps can continue
                }

                try
                {
                    using (var outputStream = await output.OpenWrite(page.Slug)) 
                    {
                        await template.RenderPage(outputStream, page);

                        logger.Log(string.Format("Success: published '{0}'", folder.Name));
                    }
                }
                catch
                {
                    logger.Log(string.Format("Error: failed to publish page '{0}'", folder.Path));
                }

                if (indexer != null)
                {
                    try
                    {
                        await indexer.AddOrUpdate(page);
                    }
                    catch
                    {
                        logger.Log(string.Format("Error: failed to index page '{0}'", folder.Path));
                    }
                }
            }
        }

        public async Task<Dictionary<string, object>> GetDatasources(string containerName)
        {
            var datasources = new Dictionary<string, object>();
            var files = await source.GetFiles(containerName); 
            var csv = files
                .Where(f => GetExtension(f.Name) == "csv")
                .ToList();

            foreach (var file in csv) 
            {
                var key = GetFileNameWithoutExtension(file.Name);

                datasources.Add(key, await ParseCsv(key, file.Path));
            }

            return datasources;
        }

        private string GetSlug(string path)
        {
            return Path.GetFileName(path);
        }

        private async Task<string> GetFileContent(string path)
        {
            using (var inputStream = await source.OpenRead(path))
            using (var streamReader = new StreamReader(inputStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private async Task<TMetaData> ParseMetaData<TMetaData>(string path)
        {
            var extension = GetExtension(path);

            using (var inputStream = await source.OpenRead(path))
            {
                switch (extension)
                {
                    case "yaml":
                        return await yaml.Deserialize<TMetaData>(inputStream);
                    case "json":
                        return await json.Deserialize<TMetaData>(inputStream);
                    default:
                        return default(TMetaData);
                }
            }
        }

        private async Task<object[]> ParseCsv(string key, string path)
        {
            using (var inputStream = await source.OpenRead(path))
            {
                return await csv.Deserialize(key, inputStream);
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

        private bool IsSectionName(string path)
        {
            return !new string[] { "metadata", "thumbs", "thumbnail", "icon" }
                .Contains(GetSectionName(path));
        }

        public StaticSiteGenerator(ISource source, IOutput output, IYaml yaml, IJson json, ICsv csv, IIndexer indexer, ILogger logger)
        {
            this.source = source;
            this.output = output;
            this.yaml = yaml;
            this.json = json;
            this.csv = csv;
            this.indexer = indexer;
            this.logger = logger;
        }
    }
}
