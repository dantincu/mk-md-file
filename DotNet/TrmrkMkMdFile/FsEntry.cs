﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrmrkMkMdFile
{
    public class FsEntry
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public bool? IsFolder { get; set; }
    }
}
