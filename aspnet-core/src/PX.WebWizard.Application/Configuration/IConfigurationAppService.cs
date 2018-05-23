using System.Threading.Tasks;
using PX.WebWizard.Configuration.Dto;

namespace PX.WebWizard.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
