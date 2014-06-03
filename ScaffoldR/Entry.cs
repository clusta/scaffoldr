using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Entry
    {
        public string Kind { get; set; }
        public DateTime Timestamp { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Uri { get; set; }
        public string[] Tags { get; set; }
    }
}
