using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class DefaultIndexer : IIndexer
    {
        private List<Entry> entries = new List<Entry>();
        
        public Task<IDictionary<string, object>> Index(object model)
        {
            var page = model as dynamic;
            var entry = new Entry()
            {
                Kind = page.Kind,
                Uri = page.Slug
            };

            if (page.Metadata != null)
            {
                entry.Title = page.Metadata.Title;
                entry.Description = page.Metadata.Description;
                entry.Timestamp = page.Metadata.Published;
                entry.Tags = page.Metadata.Tags;
            }

            if (page.Thumbnail != null)
            {
                entry.Thumbnail = page.Thumbnail.Uri;
            }

            entries.Add(entry);

            IDictionary<string, object> data = new Dictionary<string, object>()
            {
                { "sitemap", entries }
            };

            return Task.FromResult(data);
        }
    }
}
