using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Page<TMetadata>
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("metadata")]
        public TMetadata Metadata { get; set; }

        [JsonProperty("sections")]
        public IDictionary<string, Section> Sections { get; set; }

        [JsonProperty("data")]
        public IDictionary<string, object> Data { get; set; }

        [JsonProperty("thumbnail")]
        public Media Thumbnail { get; set; }

        public string GetAllContent()
        {
            if (Sections == null)
            {
                return string.Empty;
            }
                
            var content = Sections
                .Where(s => s.Value != null)
                .Select(s => s.Value.Content)
                .ToArray();
                
            return string.Concat(content);
        }

        public IEnumerable<Source> GetAllSources()
        {
            var sources = new List<Source>();

            if (Sections != null)
            {
                sources.AddRange(Sections
                    .Where(s => s.Value.Media != null)
                    .SelectMany(s => s.Value.Media)
                    .Where(m => m.Sources != null && m.Sources.Any())
                    .SelectMany(m => m.Sources)
                    .ToList());
            }

            if (Thumbnail != null && Thumbnail.Sources != null && Thumbnail.Sources.Any())
            {
                sources.AddRange(Thumbnail.Sources);
            }

            return sources;
        }
    }
}
