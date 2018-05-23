using Microsoft.AspNetCore.Antiforgery;
using PX.WebWizard.Controllers;

namespace PX.WebWizard.Web.Host.Controllers
{
    public class AntiForgeryController : WebWizardControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
