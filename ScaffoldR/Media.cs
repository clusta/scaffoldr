using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Media
    {
        public string Source { get; set; }
        public string Uri { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Video Video { get; set; }
        public Action Action { get; set; }
    }
}
