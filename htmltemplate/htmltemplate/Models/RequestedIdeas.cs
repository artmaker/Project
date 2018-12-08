using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace htmltemplate.Models
{
    public class RequestedIdeas
    {
        public string IdeaId { get; set; }
        public string IdeaTitle { get; set; }
        public string AppliedBy { get; set; }
        public string RequestTo { get; set; }
    }
}