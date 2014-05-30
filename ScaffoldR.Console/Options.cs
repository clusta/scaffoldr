using CommandLine;
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
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output path")]
        public string Output { get; set; }

        [Option('c', "container", Required = true, HelpText = "Container name")]
        public string ContainerName { get; set; }

        [Option('t', "template", Required = true, HelpText = "Template path")]
        public string Template { get; set; }

        [Option('a', "accessKey", Required = false, HelpText = "Access key")]
        public string AccessKey { get; set; }

        [Option('s', "secretKey", Required = false, HelpText = "Secret key")]
        public string SecretKey { get; set; }
    }
}
