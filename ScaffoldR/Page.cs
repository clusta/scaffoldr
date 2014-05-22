﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Page<TMetaData>
    {
        public string Slug { get; set; }
        public TMetaData MetaData { get; set; }
        public IDictionary<string, Section> Sections { get; set; }
        public string Thumbnail { get; set; }
    }
}
