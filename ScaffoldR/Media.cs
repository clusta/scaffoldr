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
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("video")]
        public Video Video { get; set; }

        [JsonProperty("action")]
        public Action Action { get; set; }
    }
}
