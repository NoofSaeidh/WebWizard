using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using PX.WebWizard.EntityFrameworkCore.Seed;

namespace PX.WebWizard.EntityFrameworkCore
{
    [DependsOn(
        typeof(WebWizardCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class WebWizardEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.UnitOfWork.IsTransactional = false;
                Configuration.Modules.AbpEfCore().AddDbContext<WebWizardDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        WebWizardDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        WebWizardDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WebWizardEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
