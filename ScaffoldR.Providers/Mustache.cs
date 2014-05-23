using Mustache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class Mustache : ITemplate
    {
        private Generator generator;

        public async Task RenderPage(Stream outputStream, object page)
        {
            var output = generator.Render(page);

            using (var streamWriter = new StreamWriter(outputStream))
            {
                await streamWriter.WriteAsync(output);
            }
        }

        public Mustache(string template) 
        {
            var compiler = new FormatCompiler();

            generator = compiler.Compile(template);
        }
    }
}
