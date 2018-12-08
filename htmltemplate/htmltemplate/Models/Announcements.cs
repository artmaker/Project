using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace htmltemplate.Models
{
    public class Announcements
    {
        
        public string Announce { get; set; }
        public string Cale { get; set; }
        public string IdeaId { get; set; }
    }
    public class DropDown
    {
        public IEnumerable<RequestedIdeas> Ideas { get; set; }
        public string idea { get; set; }
    }
}