using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Input
    {
        [JsonProperty("base_address")]
        public string BaseAddress { get; set; }

        [JsonProperty("content_path")]
        public string ContentPath { get; set; }

        [JsonProperty("data_path")]
        public string DataPath { get; set; }
    }
}
