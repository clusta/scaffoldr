using CommandLine;
using Newtonsoft.Json;
using ScaffoldR.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {
                try
                {
                    var json = File.ReadAllText(options.BatchPath);
                    var jobs = JsonConvert.DeserializeObject<Job[]>(json);
                    var container = new DefaultContainer(null, logger);
                    var staticSite = new StaticSite(container);
                    var publish = staticSite.PublishAsync<Metadata>(jobs);

                    Task.WaitAll(publish);
                }
                catch(Exception e)
                {
                    logger.Log(string.Format("Error during publish '{0}'", e.InnerException.Message));
                }
            }
            else
            {
                logger.Log("Unable to parse arguments");
            }
        }

        class Options
        {
            [Option('b', "batch", Required = true, HelpText = "Batch file path")]
            public string BatchPath { get; set; }
        }        
    }
}