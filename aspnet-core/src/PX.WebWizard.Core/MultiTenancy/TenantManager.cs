using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using PX.WebWizard.Authorization.Users;
using PX.WebWizard.Editions;

namespace PX.WebWizard.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore)
        {
        }
    }
}
