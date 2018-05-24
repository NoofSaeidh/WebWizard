using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public class IisApplication
    {
        public string Path { get; set; }
        public IList<string> Uris { get; set; }
        public string Version { get; set; }
    }
}
