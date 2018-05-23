using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PX.WebWizard.Authorization;

namespace PX.WebWizard
{
    [DependsOn(
        typeof(WebWizardCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class WebWizardApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<WebWizardAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(WebWizardApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
