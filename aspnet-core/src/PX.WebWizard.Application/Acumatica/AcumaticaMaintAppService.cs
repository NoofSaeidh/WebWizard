using Abp.Domain.Uow;
using Microsoft.Extensions.Options;
using PX.WebWizard.Acumatica.Dto;
using PX.WebWizard.Acumatica.IisManagement;
using PX.WebWizard.Acumatica.Wizard;
using PX.WebWizard.Configuration;
using PX.WebWizard.LongRun;
using PX.WebWizard.LongRun.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica
{
    [UnitOfWork(IsDisabled = true)]
    public class AcumaticaMaintAppService : WebWizardAppServiceBase
    {
        private readonly ILongRunBackgroundJobManager _backgroundJobManager;
        private readonly IAcIisService _acIisService;
        private readonly AcumaticaSettings _acumaticaSettings;
        private readonly IAcumaticaSettingsService _acumaticaSettingsService;

        public AcumaticaMaintAppService(ILongRunBackgroundJobManager backgroundJobManager,
            IAcIisService acIisService, IAcumaticaSettingsService acumaticaSettingsService)
        {
            _backgroundJobManager = backgroundJobManager;
            _acIisService = acIisService;
            _acumaticaSettingsService = acumaticaSettingsService;
            _acumaticaSettings = _acumaticaSettingsService.Settings;
        }

        public Task<IEnumerable<Application>> GetApplications()
        {
            // todo: add async to AcIisService
            return Task.FromResult(_acIisService.GetApplications());
        }

        public Task<IEnumerable<Installation>> GetInstallations()
        {
            return Task.FromResult(_acIisService.GetInstallations());
        }
        
        public async Task<LongRunResultDto> DownloadInstallation(InstallationDownloadDto installation)
        {
            // todo: null checks??
            var rawResult = await _backgroundJobManager.EnqueueLongRunAsync<InstallationLoaderJob, InstallationLoaderJobArgs>(
                new InstallationLoaderJobArgs
                {
                    Version = installation.Version,
                    //todo: path
                    ResultPath = _acumaticaSettingsService.GetPathForInstallation(installation.Name)
                });

            var result = ObjectMapper.Map<LongRunResultDto>(rawResult);
            // url service
            result.ResultUrl = "{url}/" + result.LongRunId;

            return result;
        }

        public async Task<LongRunResultDto> DeploySite(string arguments)
        {
            // todo: null checks??
            var rawResult = await _backgroundJobManager.EnqueueLongRunAsync<WizardRunnerJob, WizardRunnerJobArgs>(
                new WizardRunnerJobArgs
                {
                    AcExePath = @"..\\..\\test\\PX.WebWizard.Tests.FakeAcExe\\bin\\Debug\\PX.WebWizard.Tests.FakeAcExe.exe",
                    WizardArgs = arguments
                });

            var result = ObjectMapper.Map<LongRunResultDto>(rawResult);
            // url service
            result.ResultUrl = "{url}/" + result.LongRunId;

            return result;
        }
    }
}
