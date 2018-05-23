using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.AcExe
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyType { get; set; }
        public int? ParentID { get; set; } = 1;
        public bool? Visible { get; set; } = true;
        public string LoginName { get; set; }
    }
}
