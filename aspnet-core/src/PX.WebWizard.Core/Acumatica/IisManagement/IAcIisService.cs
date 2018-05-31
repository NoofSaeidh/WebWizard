using System.Collections.Generic;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public interface IAcIisService
    {
        IEnumerable<Application> GetApplications();
        IEnumerable<Installation> GetInstallations();
    }
}
