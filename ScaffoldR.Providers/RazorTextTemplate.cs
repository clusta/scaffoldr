using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class RazorTextTemplate : ITextTemplate
    {
        private string path;
        
        public string RenderTemplate(object model)
        {
            var template = File.ReadAllText(path);

            return Razor.Parse(template, model);
        }

        public RazorTextTemplate(string path)
        {
            this.path = path;
        }
    }
}
