using System.Collections.Generic;

namespace AcWebTool.Core.AcIIS
{
    public interface IAcIISService
    {
        IEnumerable<Application> GetApplications();
        IEnumerable<Installation> GetInstallations();
    }
}
