using System;
using System.Collections.Generic;
using System.Text;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public class IisSite
    {
        public IEnumerable<IisApplication> IISApplications { get; set; }
        public IEnumerable<string> Uris { get; set; }
    }
}
