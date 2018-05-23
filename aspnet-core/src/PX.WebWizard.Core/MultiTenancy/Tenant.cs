using Abp.MultiTenancy;
using PX.WebWizard.Authorization.Users;

namespace PX.WebWizard.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
