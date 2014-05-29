using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Page<TMetadata>
    {
        public string Slug { get; set; }
        public TMetadata Metadata { get; set; }
        public IDictionary<string, Section> Sections { get; set; }
        public IDictionary<string, object> Datasources { get; set; }
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

        public IEnumerable<Media> GetAllMedia()
        {
            var media = new List<Media>();

            if (Sections != null)
            {
                media.AddRange(Sections
                    .Where(s => s.Value.Media != null)
                    .SelectMany(s => s.Value.Media));
            }

            if (Thumbnail != null)
            {
                media.Add(Thumbnail);
            }

            return media;
        }
    }
}
