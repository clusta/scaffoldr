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

        public string Path { get; private set; }
        public string Section { get; private set; }
        public int Order { get; private set; }
        public string Variant { get; private set; }
        public string Extension { get; private set; }

        public bool IsMedia
        {
            get
            {
                return Extension == "jpg" || Extension == "png";
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
