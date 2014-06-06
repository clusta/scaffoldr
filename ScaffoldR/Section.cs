using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScaffoldR
{
    public class Section
    {
        public string Title { get; set; }
        public string Description { get; set; }        
        public string Content { get; set; }
        public string Source { get; set; }
        public IList<Media> Media { get; set; }
    }
}
