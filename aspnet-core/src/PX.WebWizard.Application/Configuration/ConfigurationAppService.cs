using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using PX.WebWizard.Configuration.Dto;

namespace PX.WebWizard.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : WebWizardAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
