using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Template
    {
        [JsonProperty("template_path")]
        public string TemplatePath { get; set; }
    }
}
