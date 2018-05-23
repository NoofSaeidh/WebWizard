using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PX.WebWizard.Roles.Dto;
using PX.WebWizard.Users.Dto;

namespace PX.WebWizard.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
