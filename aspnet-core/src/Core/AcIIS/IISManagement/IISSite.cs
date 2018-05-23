using System;
using System.Collections.Generic;
using System.Text;

namespace AcWebTool.Core.AcIIS.IISManagement
{
    public class IISSite
    {
        public IEnumerable<IISApplication> IISApplications { get; set; }
        public IEnumerable<string> Uris { get; set; }
    }
}
