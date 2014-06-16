using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Video
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("embed")]
        public string Embed { get; set; }
    }
}
