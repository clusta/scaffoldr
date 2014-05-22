using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScaffoldR
{
    public class Section
    {
        public string Content { get; set; }
        public IList<Media> Media { get; set; }
    }
}
