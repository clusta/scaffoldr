using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class StaticSite
    {
        private IContainer container;

        private static IDictionary<string, string> mediaContentTypes = new Dictionary<string, string>()
        {
            { "jpg", Constants.ContentType.Jpg },
            { "png", Constants.ContentType.Png }
        };

        private static IDictionary<string, string> sectionContentTypes = new Dictionary<string, string>()
        {
            { "html", Constants.ContentType.Html },
            { "md", Constants.ContentType.Markdown }
        };

        private static IDictionary<string, string> metadataContentTypes = new Dictionary<string, string>()
        {
            { "yaml", Constants.ContentType.Yaml },
	        { "json", Constants.ContentType.Json }
        };

        public async Task<Page<TMetadata>> ParsePageAsync<TMetadata>(IFileSource source, string path, string kind)
        {
            var files = await source.GetFilesAsync(path);

            // ensure alpha-numeric order
            files = files.OrderBy(f => f).ToList();

            var slug = GetSlug(path);
            var page = new Page<TMetadata>()
            {
                Kind = kind,
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
                    .Where(f => GetFileNameWithoutExtension(f) == "metadata" && IsMetadata(f))
                    .Select(f => f)
                    .FirstOrDefault();

            if (metaDataPath != null) 
            {
                page.Metadata = await ParseMetadataAsync<TMetadata>(source, metaDataPath);
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
                section.Content = await ReadFileAsStringAsync(source, section.Source);
            }

            return page;
        }

        public async Task PublishAsync<TMetadata>(Job[] tasks)
        {
            var indexer = container.ResolveIndexer();
            
            foreach (var publish in tasks)
            {
                var fileSource = container.ResolveFileSource(publish.Source.BaseAddress);
                var fileDestination = container.ResolveFileDestination(publish.Destination.BaseAddress, publish.Destination.BucketName, publish.Destination.AccessKey, publish.Destination.SecretKey);
                var template = container.ResolveTemplate(publish.Template.TemplatePath);
                var folders = await fileSource.GetFoldersAsync(publish.Source.ContentPath);
                var datasources = await GetDatasourcesAsync(fileSource, publish.Source.DataPath);

                foreach (var folder in folders)
                {
                    try
                    {
                        var page = await ParsePageAsync<TMetadata>(fileSource, folder, publish.Kind);
                        var data = await indexer.Index(page);

                        // merge data from data folder and indexer, indexer values take precidence
                        page.Data = datasources
                            .Concat(data)
                            .GroupBy(d => d.Key)
                            .ToDictionary(d => d.Key, d => d.First().Value);

                        var stringOutput = template.RenderTemplate(page);

                        // publish page
                        using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(stringOutput)))
                        {
                            await fileDestination.SaveAsync(page.Slug, publish.Destination.ContentType, inputStream);
                        }

                        // publish media
                        foreach (var media in page.GetAllMedia())
                        {
                            using (var inputStream = await fileSource.OpenStreamAsync(media.Source))
                            {
                                await fileDestination.SaveAsync(media.Uri, mediaContentTypes[GetExtension(media.Uri)], inputStream);
                            }
                        }                        
                        
                        Log("Success: publishing '{0}'", folder);
                    }
                    catch(Exception e)
                    {
                        Log("Error: publishing '{0}' message '{1}'", folder, e.Message);
                    }
                }
            }
        }

        private void Log(string formatString, params object[] values) 
        {
            var logger = container.ResolveLogger();

            if (logger != null)
            {
                logger.Log(string.Format(formatString, values));
            }
        }

        public async Task<Dictionary<string, object>> GetDatasourcesAsync(IFileSource source, string containerName)
        {
            var datasources = new Dictionary<string, object>();
            var files = await source.GetFilesAsync(containerName); 
            var csv = files
                .Where(f => GetExtension(f) == "csv")
                .ToList();

            foreach (var file in csv) 
            {
                var key = GetFileNameWithoutExtension(file);

                datasources.Add(key, await ParseCsvAsync(source, key, file));
            }

            return datasources;
        }

        private string GetSlug(string path)
        {
            return Path.GetFileName(path);
        }

        private async Task<string> ReadFileAsStringAsync(IFileSource source, string path)
        {
            using (var inputStream = await source.OpenStreamAsync(path))
            using (var streamReader = new StreamReader(inputStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private async Task<TMetadata> ParseMetadataAsync<TMetadata>(IFileSource source, string path)
        {
            var extension = GetExtension(path);
            var name = Path.GetFileNameWithoutExtension(path);

            using (var inputStream = await source.OpenStreamAsync(path))
            {
                return container
                        .ResolveDeserializer(name, metadataContentTypes[extension])
                        .Deserialize<TMetadata>(inputStream);
            }
        }

        private async Task<object[]> ParseCsvAsync(IFileSource source, string key, string path)
        {
            using (var inputStream = await source.OpenStreamAsync(path))
            {
                return container
                        .ResolveDataReader(key, Constants.ContentType.Csv)
                        .ReadData(inputStream);
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

        private bool IsMetadata(string path)
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

        public StaticSite(IContainer container)
        {
            Contract.NotNull(container, "container");

            this.container = container;
        }
    }
}
