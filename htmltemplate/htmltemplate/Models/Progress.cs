using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace htmltemplate.Models
{
    public class Progress
    {
        public Nullable<float> Bar1 { get; set; }
        public Nullable<float> Bar2 { get; set; }
        public Nullable<float> Bar3 { get; set; }
        public Nullable<float> Bar4 { get; set; }
        public string IdeaId { get; set; }
        public string Teacher { get; set; }
    }
}