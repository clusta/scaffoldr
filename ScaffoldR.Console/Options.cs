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
        [Option('b', "batch", Required = true, HelpText = "Batch file path")]
        public string BatchPath { get; set; }
    }
}
