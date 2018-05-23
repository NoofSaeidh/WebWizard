using System.Threading.Tasks;
using Abp.Application.Services;
using PX.WebWizard.Authorization.Accounts.Dto;

namespace PX.WebWizard.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
