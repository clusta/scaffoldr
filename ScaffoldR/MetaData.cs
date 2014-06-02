using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Metadata
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public DateTime Published { get; set; }
        public string[] Tags { get; set; }
        public Author Author { get; set; }
        public string Source { get; set; }
    }
}
