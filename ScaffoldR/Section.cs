using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScaffoldR
{
    public class Section
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("media")]
        public IList<Media> Media { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; set; }

        public Section()
        {
            Extensions = new Dictionary<string, object>();
        }
    }
}
