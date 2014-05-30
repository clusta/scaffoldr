using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Console
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input path")]
        [JsonProperty("input")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output path")]
        [JsonProperty("output")]
        public string OutputPath { get; set; }

        [Option('c', "container", Required = false, HelpText = "Container name")]
        [JsonProperty("container")]
        public string ContainerName { get; set; }

        [Option('t', "template", Required = false, HelpText = "Template path")]
        [JsonProperty("template")]
        public string TemplatePath { get; set; }

        [Option('a', "access_key", Required = false, HelpText = "Access key")]
        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        [Option('s', "secret_key", Required = false, HelpText = "Secret key")]
        [JsonProperty("secret_key")]
        public string SecretKey { get; set; }
    }
}
