using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Action
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
