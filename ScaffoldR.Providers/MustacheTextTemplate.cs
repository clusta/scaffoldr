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
        private string path;

        public string RenderTemplate(object page)
        {
            var template = File.ReadAllText(path);
            var compiler = new FormatCompiler();
            var generator = compiler.Compile(template);            
            
            return generator.Render(page);
        }

        public MustacheTextTemplate(string path) 
        {
            this.path = path;
        }
    }
}
