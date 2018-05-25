using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PX.WebWizard.Configuration;
using Microsoft.Extensions.Options;

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

            // todo: just a temp hack for DI
            IocManager.Register<IOptionsSnapshot<AcumaticaSettings>, AcumaticaSettingsSnapshot>();
        }

        private class AcumaticaSettingsSnapshot : IOptionsSnapshot<AcumaticaSettings>
        {
            public AcumaticaSettings Value => new AcumaticaSettings
            {
                GlobalPath = "somewhere"
            };

            public AcumaticaSettings Get(string name)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
