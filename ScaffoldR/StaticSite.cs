using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

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

        public async Task<Page<TMetadata>> ParsePageAsync<TMetadata>(IFileInput source, string path, string kind)
        {
            Func<InputFile, Source> mapFileToSource = f =>
            {
                return new Source()
                {
                    Uri = f.Path,
                    ContentType = mediaContentTypes[f.Extension],
                    Variant = f.Variant
                };             
            };
            
            var paths = await source.GetFilesAsync(path);
            var files = paths.Select(p => new InputFile(p)).OrderBy(f => f.Section).ThenBy(f => f.Order);
            var slug = GetSlug(path);
            var page = new Page<TMetadata>()
            {
                Kind = kind,
                Slug = slug,
                Thumbnail = new Media() 
                {
                    Sources = files
                        .Where(f => f.IsMedia && f.Section == "thumbnail")
                        .Select(mapFileToSource)
                        .ToArray()
                },
                Sections = new Dictionary<string, Section>()
            };

            var metaDataPath = files.Where(f => f.Section == "metadata").Select(f => f.Path).FirstOrDefault();

            if (metaDataPath != null) 
            {
                page.Metadata = await ParseMetadataAsync<TMetadata>(source, metaDataPath);
            }

            var ignoreList = new string[] { "metadata", "thumbnail", "thumbs", "icon" };

            foreach (var sectionGroup in files.Where(s => !ignoreList.Contains(s.Section)).GroupBy(f => f.Section))
            {
                Section section;

                // section metadata
                var metadataPath = sectionGroup.Where(f => metadataContentTypes.ContainsKey(f.Extension)).Select(f => f.Path).FirstOrDefault();

                if (metadataPath != null)
                {
                    section = await ParseMetadataAsync<Section>(source, metadataPath);
                }
                else
                {
                    section = new Section();
                }

                // section html or markdown content
                var contentExtenions = new string[] { "html", "htm", "md" };
                var contentPath = sectionGroup.Where(f => contentExtenions.Contains(f.Extension)).Select(f => f.Path).FirstOrDefault();

                if (contentPath != null)
                {
                    section.Content = await ReadFileAsStringAsync(source, contentPath);
                }

                // section media
                var media = sectionGroup
                        .Where(s => s.IsMedia)
                        .GroupBy(s => s.Order)
                        .Select(g => new Media()
                        {
                            Sources = g.Select(mapFileToSource).ToList()
                        })
                        .ToList();

                if (section.Media != null)
                {
                    section.Media = MergeMediaLists(media, section.Media);
                }
                else
                {
                    section.Media = media;
                }

                page.Sections.Add(sectionGroup.Key, section);
            }

            return page;
        }

        private IList<Media> MergeMediaLists(IList<Media> mediaWithSources, IList<Media> mediaWithMetadata)
        {
            return mediaWithSources
                    .Zip(mediaWithMetadata, (mediaItemWithSource, mediaItemWithMetadata) =>
                    {
                        if (mediaItemWithMetadata != null && mediaItemWithSource != null && mediaItemWithSource.Sources != null && mediaItemWithSource.Sources.Any())
                        {
                            mediaItemWithMetadata.Sources = mediaItemWithSource.Sources;

                            return mediaItemWithMetadata;
                        }
                        else
                        {
                            return mediaItemWithSource ?? mediaItemWithMetadata;
                        }
                    })
                    .ToList();
        }

        public async Task PublishAsync<TMetadata>(Job[] tasks)
        {
            var indexer = container.ResolveIndexer();
            
            foreach (var publish in tasks)
            {
                var fileSource = container.ResolveFileInput(publish.Input.BaseAddress);
                var fileDestination = container.ResolveFileOutput(publish.Output.BaseAddress, publish.Output.BucketName, publish.Output.AccessKey, publish.Output.SecretKey);
                var template = container.ResolveTextTemplate(publish.Template.TemplatePath);
                var folders = await fileSource.GetFoldersAsync(publish.Input.ContentPath);
                var datasources = await GetDatasourcesAsync(fileSource, publish.Input.DataPath);

                foreach (var folder in folders)
                {
                    try
                    {
                        var page = await ParsePageAsync<TMetadata>(fileSource, folder, publish.Kind);

                        if (indexer != null)
                        {
                            var data = await indexer.Index(page);

                            // merge data from data folder and indexer, indexer values take precidence
                            page.Data = datasources
                                .Concat(data)
                                .GroupBy(d => d.Key)
                                .ToDictionary(d => d.Key, d => d.First().Value);
                        }
                        else
                        {
                            page.Data = datasources;
                        }

                        // publish media
                        foreach (var source in page.GetAllSources())
                        {
                            var path = source.Uri;

                            source.Uri = RewriteImagePath(page.Slug, path);
                            
                            using (var inputStream = await fileSource.OpenStreamAsync(path))
                            {
                                await fileDestination.SaveAsync(source.Uri, source.ContentType, inputStream);
                            }
                        } 

                        var stringOutput = template.RenderTemplate(page);

                        // publish page
                        using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(stringOutput)))
                        {
                            await fileDestination.SaveAsync(page.Slug, publish.Output.ContentType, inputStream);
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

        public async Task<Dictionary<string, object>> GetDatasourcesAsync(IFileInput source, string path)
        {
            var datasources = new Dictionary<string, object>();
            var files = await source.GetFilesAsync(path); 
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

        private async Task<string> ReadFileAsStringAsync(IFileInput source, string path)
        {
            using (var inputStream = await source.OpenStreamAsync(path))
            using (var streamReader = new StreamReader(inputStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private async Task<TMetadata> ParseMetadataAsync<TMetadata>(IFileInput source, string path)
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

        private async Task<object[]> ParseCsvAsync(IFileInput source, string key, string path)
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

        private string GetExtension(string path)
        {
            return Path.GetExtension(path)
                .ToLower()
                .TrimStart('.');
        }

        private string RewriteImagePath(string pageSlug, string imagePath)
        {
            if (string.IsNullOrEmpty(pageSlug) && string.IsNullOrEmpty(imagePath))
            {
                return string.Empty;
            }
            else if (string.IsNullOrEmpty(pageSlug) && !string.IsNullOrEmpty(imagePath))
            {
                return Path.GetFileName(imagePath).ToLower();
            }
            else
            {
                return string.Format("{0}-{1}", pageSlug, Path.GetFileName(imagePath).ToLower());
            }
        }

        public StaticSite(IContainer container)
        {
            Requires.NotNull(container, "container");

            this.container = container;
        }
    }
}
