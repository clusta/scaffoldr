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
            var options = new Options();

            if (TryParseArguments(args, options))
            {
                try
                {
                    var jsonString = File.ReadAllText(options.BatchPath);
                    var batch = JsonConvert.DeserializeObject<Job[]>(jsonString);
                    var container = new DefaultContainer(null);
                    var publisher = new StaticSite(container);
                    var task = publisher.PublishAsync<Metadata>(batch);

                    Task.WaitAll(task);
                }
                catch(Exception e)
                {
                    WriteOutput("Error during publish '{0}'", e.InnerException.Message);
                }
            }
            else
            {
                WriteOutput("Unable to parse arguments");
            }
        }

        private static bool TryParseArguments(string[] args, Options options)
        {
            return CommandLine.Parser.Default.ParseArguments(args, options);
        }

        private static void WriteOutput(string formatString, params object[] values)
        {
            System.Console.WriteLine(formatString, values);
        }
    }
}