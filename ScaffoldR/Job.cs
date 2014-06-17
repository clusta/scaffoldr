using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Job
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("input")]
        public Input Input { get; set; }

        [JsonProperty("template")]
        public Template Template { get; set; }

        [JsonProperty("output")]
        public Output Output { get; set; }
    }
}
