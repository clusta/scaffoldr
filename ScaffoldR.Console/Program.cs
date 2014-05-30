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
                    var output = GetPublishOutput(options.Output, options.AccessKey, options.SecretKey);
                    var publisher = new Publisher(
                        new FileSystemPublishSource(options.Input),
                        output,
                        new ConsolePublishLog(),
                        null,
                        new YamlDotNetDeserializer(),
                        new JsonNetDeserializer(),
                        new FileHelpersCsvDeserializer(null));

                    var textTemplate = GetTextTemplate(options.Template);
                    var publishTask = publisher.PublishContainerAsync(options.ContainerName, textTemplate);

                    Task.WaitAll(publishTask);
                }
                catch(Exception e)
                {
                    WriteOutput("Error during publish '{0}'", e.Message);
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

        private const string s3Prefix = "arn:aws:s3:::";

        private static IPublishOutput GetPublishOutput(string path, string accessKey, string secretKey)
        {
            if (path.StartsWith(s3Prefix))
            {
                return new AmazonS3PublishOuput(accessKey, secretKey, path.Substring(s3Prefix.Length));
            }
            else
            {
                return new FileSystemPublishOutput(path);
            }
        }
    }
}