using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using PX.WebWizard.Authorization.Roles;
using PX.WebWizard.Authorization.Users;
using PX.WebWizard.MultiTenancy;
using PX.WebWizard.LongRun;

namespace PX.WebWizard.EntityFrameworkCore
{
    public class WebWizardDbContext : AbpZeroDbContext<Tenant, Role, User, WebWizardDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public WebWizardDbContext(DbContextOptions<WebWizardDbContext> options)
            : base(options)
        {
        }

        public DbSet<LongRunInfo> LongRunInfos { get; set; }
    }
}
