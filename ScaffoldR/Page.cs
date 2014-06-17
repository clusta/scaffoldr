using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Page<TMetadata>
    {
        public string Kind { get; set; }
        public string Slug { get; set; }
        public TMetadata Metadata { get; set; }
        public IDictionary<string, Section> Sections { get; set; }
        public IDictionary<string, object> Data { get; set; }
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
