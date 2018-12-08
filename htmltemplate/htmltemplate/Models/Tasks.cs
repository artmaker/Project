using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace htmltemplate.Models
{
    public class Tasks
    {
        //public int Id { get; set; }
        public int TaskId { get; set; }
        public string TasksAssigned { get; set; }
        public string TasksCompleted { get; set; }
        public string IdeaId { get; set; }
        public string Teacher { get; set; }
        public string Title { get; set; }
    }
}