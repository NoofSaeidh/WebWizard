using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PX.WebWizard.Configuration;

namespace PX.WebWizard.Web.Host.Startup
{
    [DependsOn(
       typeof(WebWizardWebCoreModule))]
    public class WebWizardWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public WebWizardWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WebWizardWebHostModule).GetAssembly());
        }
    }
}
