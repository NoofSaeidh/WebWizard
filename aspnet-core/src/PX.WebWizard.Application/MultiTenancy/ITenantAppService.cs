using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PX.WebWizard.MultiTenancy.Dto;

namespace PX.WebWizard.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
