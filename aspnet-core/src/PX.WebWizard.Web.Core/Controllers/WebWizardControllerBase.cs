using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace PX.WebWizard.Controllers
{
    public abstract class WebWizardControllerBase: AbpController
    {
        protected WebWizardControllerBase()
        {
            LocalizationSourceName = WebWizardConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
