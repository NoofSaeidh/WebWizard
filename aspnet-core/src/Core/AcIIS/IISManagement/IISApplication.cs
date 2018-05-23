using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AcWebTool.Core.AcIIS.IISManagement
{
    public class IISApplication
    {
        public string Path { get; set; }
        public IList<string> Uris { get; set; }
        public string Version { get; set; }
    }
}
