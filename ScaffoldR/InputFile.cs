using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class InputFile
    {
        private static Regex regex = new Regex(@"^(?<section>\w+)(\-(?<order>\d+))?(@(?<variant>\w+))?\.(?<extension>\w+)$");

        private static Tuple<InputType, string, string>[] contentTypes = new Tuple<InputType, string, string>[] 
        {
            new Tuple<InputType, string, string>(InputType.Content, "html", Constants.ContentType.Html),
            new Tuple<InputType, string, string>(InputType.Content, "htm", Constants.ContentType.Html),
            new Tuple<InputType, string, string>(InputType.Content, "markdown", Constants.ContentType.Markdown),
            new Tuple<InputType, string, string>(InputType.Content, "md", Constants.ContentType.Markdown),
            new Tuple<InputType, string, string>(InputType.Media, "jpg", Constants.ContentType.Jpg),
            new Tuple<InputType, string, string>(InputType.Media, "jpeg", Constants.ContentType.Jpg),
            new Tuple<InputType, string, string>(InputType.Media, "png", Constants.ContentType.Png),
            new Tuple<InputType, string, string>(InputType.Metadata, "yaml", Constants.ContentType.Yaml),
            new Tuple<InputType, string, string>(InputType.Metadata, "json", Constants.ContentType.Json),
            new Tuple<InputType, string, string>(InputType.Data, "csv", Constants.ContentType.Csv)
        };

        public string Path { get; private set; }
        public string Section { get; private set; }
        public int Order { get; private set; }
        public string Variant { get; private set; }
        public string Extension { get; private set; }

        public InputType InputType
        {
            get
            {
                var definition = contentTypes.Where(c => c.Item2 == Extension).FirstOrDefault();

                return definition != null ? definition.Item1 : InputType.Unsupported;
            }
        }

        public string ContentType
        {
            get
            {
                var definition = contentTypes.Where(c => c.Item2 == Extension).FirstOrDefault();

                return definition != null ? definition.Item3 : string.Empty;
            }
        }

        public InputFile(string path)
        {
            this.Path = path;

            var fileName = System.IO.Path.GetFileName(path);

            if (!string.IsNullOrEmpty(fileName))
            {
                var match = regex.Match(fileName.ToLower());

                if (match.Success)
                {
                    int order = 0;

                    int.TryParse(match.Groups["order"].Value, out order);

                    Order = order;
                    Section = match.Groups["section"].Value;
                    Variant = match.Groups["variant"].Value;
                    Extension = match.Groups["extension"].Value;
                }
            }
        }
    }
}
