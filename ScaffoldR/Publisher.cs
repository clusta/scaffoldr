using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Publisher
    {
        private IPublishSource source;
        private IPublishOutput output;
        private IYamlDeserializer yaml;
        private IJsonDeserializer json;
        private ICsvDeserializer csv;
        private IPublishIndex indexer;
        private IPublishLog logger;

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

        public Task<Page<Metadata>> ParsePageAsync(string path)
        {
            return ParsePageAsync<Metadata>(path);
        }

        public async Task<Page<TMetadata>> ParsePageAsync<TMetadata>(string path)
        {
            var files = await source.GetPageSourcesAsync(path);
            var slug = GetSlug(path);
            var page = new Page<TMetadata>()
            {
                Slug = slug,
                Thumbnail = files
                    .Where(f => IsMedia(f) && GetFileNameWithoutExtension(f) == "thumbnail")
                    .Select(f => new Media() 
                    {
                        Source = f,
                        Uri = RewriteImagePath(slug, f),
                        ContentType = mediaContentTypes[GetExtension(f)]
                    })
                    .FirstOrDefault()
            };

            var metaDataPath = files
                    .Where(f => GetFileNameWithoutExtension(f) == "metadata" && IsMetaData(f))
                    .Select(f => f)
                    .FirstOrDefault();

            if (metaDataPath != null) 
            {
                page.Metadata = await ParseMetadataAsync<TMetadata>(metaDataPath);
            }

            page.Sections = files
                .Where(f => IsSectionName(f))
                .OrderBy(f => GetSortOrder(f))
                .GroupBy(f => GetSectionName(f))
                .ToDictionary(g => g.Key, g => new Section()
                {
                    Source = g
                        .Where(m => IsSection(m))
                        .Select(m => m)
                        .FirstOrDefault(),
                    Media = g
                        .Where(m => IsMedia(m))
                        .Select(m => new Media()
                        {
                            Source = m,
                            Uri = RewriteImagePath(page.Slug, m),
                            ContentType = mediaContentTypes[GetExtension(m)]
                        })
                        .ToList()
                });

            foreach (var section in page.Sections.Values.Where(s => !string.IsNullOrEmpty(s.Source)))
            {
                section.Content = await ReadFileAsStringAsync(section.Source);
            }

            return page;
        }

        public Task PublishContainerAsync(string path, ITextTemplate template)
        {
            return PublishContainerAsync<Metadata>(path, template);
        }

        public async Task PublishContainerAsync<TMetadata>(string containerName, ITextTemplate template)
        {
            var folders = await source.GetPagesAsync(containerName);

            Dictionary<string, object> datasources;

            try
            {
                datasources = await GetDatasourcesAsync(containerName);
            }
            catch
            {
                logger.Log("Error: failed to parse datasources");
                
                return;
            }

            Page<TMetadata> page = null;

            foreach (var folder in folders)
            {
                try
                {
                    page = await ParsePageAsync<TMetadata>(folder);

                    page.Datasources = datasources;
                }
                catch
                {
                    logger.Log(string.Format("Error: failed to parse page '{0}'", folder));

                    continue; // skip publish and index, continue processing other pages
                }

                try
                {
                    // publish page
                    var stringOutput = template.RenderTemplate(page);

                    using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(stringOutput))) 
                    {
                        await output.SaveAsync(inputStream, page.Slug, "text/html");
                    }

                    // publish images
                    foreach (var media in page.GetAllMedia())
                    {
                        using (var inputStream = await source.OpenStreamAsync(media.Source))
                        {
                            await output.SaveAsync(inputStream, media.Uri, mediaContentTypes[GetExtension(media.Uri)]);
                        }
                    }

                    logger.Log(string.Format("Success: published '{0}'", folder));
                }
                catch
                {
                    logger.Log(string.Format("Error: failed to publish page '{0}'", folder));
                }

                if (indexer != null)
                {
                    try
                    {
                        await indexer.AddOrUpdate(page);
                    }
                    catch
                    {
                        logger.Log(string.Format("Error: failed to index page '{0}'", folder));
                    }
                }
            }
        }

        public async Task<Dictionary<string, object>> GetDatasourcesAsync(string containerName)
        {
            var datasources = new Dictionary<string, object>();
            var files = await source.GetPageSourcesAsync(containerName); 
            var csv = files
                .Where(f => GetExtension(f) == "csv")
                .ToList();

            foreach (var file in csv) 
            {
                var key = GetFileNameWithoutExtension(file);

                datasources.Add(key, await ParseCsvAsync(key, file));
            }

            return datasources;
        }

        private string GetSlug(string path)
        {
            return Path.GetFileName(path);
        }

        private async Task<string> ReadFileAsStringAsync(string path)
        {
            using (var inputStream = await source.OpenStreamAsync(path))
            using (var streamReader = new StreamReader(inputStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private async Task<TMetadata> ParseMetadataAsync<TMetadata>(string path)
        {
            var extension = GetExtension(path);

            using (var inputStream = await source.OpenStreamAsync(path))
            {
                switch (extension)
                {
                    case "yaml":
                        return yaml.Deserialize<TMetadata>(inputStream);
                    case "json":
                        return json.Deserialize<TMetadata>(inputStream);
                    default:
                        return default(TMetadata);
                }
            }
        }

        private async Task<object[]> ParseCsvAsync(string key, string path)
        {
            using (var inputStream = await source.OpenStreamAsync(path))
            {
                return csv.Deserialize(key, inputStream);
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

        private string RewriteImagePath(string pageSlug, string imagePath)
        {
            return string.Format("{0}-{1}", pageSlug, GetFileName(imagePath));
        }

        public Publisher(IPublishSource source, 
            IPublishOutput output, 
            IYamlDeserializer yaml, 
            IJsonDeserializer json, 
            ICsvDeserializer csv, 
            IPublishIndex indexer, 
            IPublishLog logger)
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
