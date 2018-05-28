using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using PX.WebWizard.LongRun.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    public class LongRunAppService : WebWizardAppServiceBase
    {
        private readonly IRepository<LongRunInfo, string> _longRunInfoRepo;
        private readonly ILongRunBackgroundJobManager _longRunBackgroundJobManager;

        public LongRunAppService(IRepository<LongRunInfo, string> longRunInfoRepo,
            ILongRunBackgroundJobManager longRunBackgroundJobManager)
        {
            _longRunInfoRepo = longRunInfoRepo;
            _longRunBackgroundJobManager = longRunBackgroundJobManager;
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<LongRunInfoDto> GetLongRun(string id)
        {
            var info = await _longRunInfoRepo.GetAsync(id);
            return ObjectMapper.Map<LongRunInfoDto>(info);
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<IList<LongRunInfoDto>> GetAllLongRuns()
        {
            //todo: should add paged list
            var infos = await _longRunInfoRepo.GetAllListAsync();
            return ObjectMapper.Map<List<LongRunInfoDto>>(infos);
        }

        public async Task<LongRunAbortResultDto> AbortLongRun(string id)
        {
            var result = await _longRunBackgroundJobManager.AbortLongRunAsync(id);
            return ObjectMapper.Map<LongRunAbortResultDto>(result);
        }
    }
}
