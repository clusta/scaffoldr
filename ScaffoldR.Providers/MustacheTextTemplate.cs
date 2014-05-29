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
        private Generator generator;

        public string RenderTemplate(object page)
        {
            return generator.Render(page);
        }

        public MustacheTextTemplate(string template) 
        {
            var compiler = new FormatCompiler();

            generator = compiler.Compile(template);
        }
    }
}
