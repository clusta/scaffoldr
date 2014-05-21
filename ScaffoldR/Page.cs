using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Page
    {
        public string Path { get; set; }
        public MetaData MetaData { get; set; }
        public IDictionary<string, Section> Sections { get; set; }
        public string[] Tags { get; set; }
    }
}
