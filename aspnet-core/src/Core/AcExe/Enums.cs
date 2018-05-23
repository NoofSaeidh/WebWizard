using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.AcExe
{
    public enum ConfigurationMode
    {
        NewInstance,
        DBMaint,
        DBConection,
        CompanyConfig,
        ToolsInstall,
        NewCompanyPortal,
        DeleteSite,
        RenameSite,
        UpgradeSite
    }

    [Flags]
    public enum ArgumentType
    {
        Value = 0,
        Enumerable = 1,
        Object = 2
    }
}
