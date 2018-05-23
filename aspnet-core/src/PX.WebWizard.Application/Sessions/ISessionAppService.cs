using System.Threading.Tasks;
using Abp.Application.Services;
using PX.WebWizard.Sessions.Dto;

namespace PX.WebWizard.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
