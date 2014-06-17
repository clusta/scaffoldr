using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Media
    {
        [JsonProperty("sources")]
        public IList<Source> Sources { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public Video Author { get; set; }

        [JsonProperty("video")]
        public Video Video { get; set; }

        [JsonProperty("action")]
        public Action Action { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; set; }

        public string GetSourceUri(params string[] prefered)
        {
            if (Sources == null)
            {
                return string.Empty;
            }
            
            return
                Sources
                    .Where(s => prefered.Any(p => s.Variant.Equals(p, StringComparison.OrdinalIgnoreCase)))
                    .Select(s => s.Uri)
                    .FirstOrDefault() ??
                Sources
                    .Where(s => string.IsNullOrEmpty(s.Variant))
                    .Select(s => s.Uri)
                    .FirstOrDefault();
        }
    }
}
