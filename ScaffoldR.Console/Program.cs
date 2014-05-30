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

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var publisher = new Publisher(
                    new FileSystemPublishSource(options.Input),
                    new FileSystemPublishOutput(options.Output),
                    new ConsolePublishLog(),
                    null,
                    new YamlDotNetDeserializer(),
                    new JsonNetDeserializer(),
                    new FileHelpersCsvDeserializer(null));

                var textTemplate = GetTextTemplate(options.Template);
                var publishTask = publisher.PublishContainerAsync(options.ContainerName, textTemplate);

                Task.WaitAll(publishTask);
            }
            else
            {
                System.Console.WriteLine("Unable to parse arguments");
            }
        }

        private static ITextTemplate GetTextTemplate(string uri)
        {
            if (IsUri(uri))
            {
                return new RemoteTextTemplate(uri);
            }
            else
            {
                var templateSource = File.ReadAllText(uri);

                return new MustacheTextTemplate(templateSource);
            }
        }

        private static bool IsUri(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute)
                && (new Uri(uri).Scheme == Uri.UriSchemeHttp || new Uri(uri).Scheme == Uri.UriSchemeHttps);
        }
    }
}