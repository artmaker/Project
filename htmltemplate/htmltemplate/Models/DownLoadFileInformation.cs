﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace htmltemplate.Models
{
    public class DownLoadFileInformation
    {

        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string uploadedby { get; set; }
    }
}