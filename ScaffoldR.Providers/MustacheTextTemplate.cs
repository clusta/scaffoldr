using Mustache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class MustacheTextTemplate : ITextTemplate
    {
        private string basePath;
        private Generator generator;

        public string RenderTemplate(string path, object page)
        {
            var absolutePath = Path.Combine(basePath, path);
            var templateString = File.ReadAllText(absolutePath);
            var compiler = new FormatCompiler();

            generator = compiler.Compile(templateString);            
            
            return generator.Render(page);
        }

        public MustacheTextTemplate(string basePath) 
        {
            this.basePath = basePath;
        }
    }
}
