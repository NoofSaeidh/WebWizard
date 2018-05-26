using Abp.Domain.Uow;
using PX.WebWizard.Acumatica.Dto;
using PX.WebWizard.Acumatica.IisManagement;
using PX.WebWizard.Acumatica.Wizard;
using PX.WebWizard.LongRun;
using System;
using System.Collections.Generic;
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

        public AcumaticaMaintAppService(ILongRunBackgroundJobManager backgroundJobManager,
            IAcIisService acIisService)
        {
            _backgroundJobManager = backgroundJobManager;
            _acIisService = acIisService;
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
                    ResultPath = installation.Name
                });

            var result = ObjectMapper.Map<LongRunResultDto>(rawResult);
            // url service
            result.ResultUrl = "{url}/" + result.LongRunId;

            return result;
        }

        public async Task<LongRunResultDto> DeploySite(/*todo: input*/)
        {
            // todo: null checks??
            var rawResult = await _backgroundJobManager.EnqueueLongRunAsync<WizardRunnerJob, WizardRunnerJobArgs>(
                new WizardRunnerJobArgs
                {
                    AcExePath = @"..\..\..\test\PX.WebWizard.Tests.FakeAcExe\bin\Debug\AcWebTool.Tests.FakeAcExe.exe",
                    WizardArgs = new WizardArgs.NewInstanceArgs
                    {
                        DatabaseConnectionNewUser = true,
                        DatabaseUser = "someone"
                    }.ToArgs()
                });

            var result = ObjectMapper.Map<LongRunResultDto>(rawResult);
            // url service
            result.ResultUrl = "{url}/" + result.LongRunId;

            return result;
        }
    }
}
