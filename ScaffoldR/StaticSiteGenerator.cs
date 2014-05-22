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
                    .Where(f => f.Name.Equals("metadata.yaml", StringComparison.OrdinalIgnoreCase))
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
            return RenderFolder<MetaData>(path, template);
        }

        public async Task RenderFolder<TMetaData>(string path, ITemplate template)
        {
            var folders = await source.GetFolders(path);

            foreach (var folder in folders)
            {
                var page = await ParsePage<TMetaData>(folder.Path);
                
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
            
            return await yaml.Deserialize<TMetaData>(fileContent);
        }

        private string GetFileName(string path)
        {
            return Path.GetFileName(path);
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

        public StaticSiteGenerator(ISource source, IOutput output, IYaml yaml)
        {
            this.source = source;
            this.output = output;
            this.yaml = yaml;
        }
    }
}
