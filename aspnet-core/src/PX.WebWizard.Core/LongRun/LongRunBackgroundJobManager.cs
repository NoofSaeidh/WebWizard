using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading;
using Abp.Threading.Timers;
using Newtonsoft.Json;
using PX.WebWizard.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    [UnitOfWork(IsDisabled = true)]
    public class LongRunBackgroundJobManager : BackgroundJobManager, ILongRunBackgroundJobManager, ISingletonDependency
    {
        private readonly IRepository<LongRunInfo, string> _longRunInfoRepo;
        private readonly IKeyGenerator _keyGenerator;

        public LongRunBackgroundJobManager(IIocResolver iocResolver,
            IBackgroundJobStore store, AbpTimer timer,
            IRepository<LongRunInfo, string> longRunInfoRepo,
            IKeyGenerator keyGenerator)
            : base(iocResolver, store, timer)
        {
            _longRunInfoRepo = longRunInfoRepo;
            _keyGenerator = keyGenerator;
        }

        public async Task<LongRunAbortResult> AbortLongRunAsync(string longRunId)
        {
            LongRunInfo longRunInfo;
            try
            {
                longRunInfo = await _longRunInfoRepo.GetAsync(longRunId);
            }
            catch (Exception e)
            {
                return new LongRunAbortResult
                {
                    AbortStatus = LongRunAbortStatus.AbortFailed,
                    Error = e.ToString()
                };
            }

            // check if background worker didn't start operation and try to delete from queue 
            if (!string.IsNullOrEmpty(longRunInfo.JobId)
                && longRunInfo.LongRunStatus == LongRunStatus.Queued)
            {
                bool aborted;
                try
                {
                    aborted = await DeleteAsync(longRunInfo.JobId);
                }
                catch (Exception)
                {
                    aborted = false;
                }
                if (aborted)
                {
                    longRunInfo.LongRunStatus = LongRunStatus.QueueAborted;
                    var abortResult = new LongRunAbortResult
                    {
                        AbortStatus = LongRunAbortStatus.Unqueued
                    };
                    try
                    {
                        await _longRunInfoRepo.UpdateAsync(longRunInfo);
                    }
                    catch (Exception e)
                    {
                        abortResult.Error = e.ToString();
                    }
                    return abortResult;
                }
            }

            if (!longRunInfo.Abortable)
            {
                return new LongRunAbortResult
                {
                    AbortStatus = LongRunAbortStatus.CannotAbort
                };
            }

            try
            {
                longRunInfo.LongRunStatus = LongRunStatus.Aborted;
                await _longRunInfoRepo.UpdateAsync(longRunInfo);
                return new LongRunAbortResult
                {
                    AbortStatus = LongRunAbortStatus.Aborted
                };
            }
            catch (Exception e)
            {
                return new LongRunAbortResult
                {
                    AbortStatus = LongRunAbortStatus.AbortFailed,
                    Error = e.ToString()
                };
            }
        }

        public async Task<LongRunResult> EnqueueLongRunAsync<TJob, TArgs>(TArgs args,
            BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : LongRunBackgroundJob<TArgs>
        {
            LongRunInfo longRunInfo;
            var longRunResult = new LongRunResult();

            // i removed try catches for persisting.
            // let it be handled by unit of work and thorwing exceptions
            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                var longRunId = _keyGenerator.Generate<string>();
                longRunInfo = new LongRunInfo
                {
                    Id = longRunId,
                    //todo: serializer? or json repo?
                    Args = JsonConvert.SerializeObject(args),
                    LongRunStatus = LongRunStatus.Queued,
                    Type = typeof(TJob).ToString(),
                };
                longRunResult.LongRunId = longRunId;
                longRunResult.Queued = true;
                await _longRunInfoRepo.InsertAsync(longRunInfo);

                await unitOfWork.CompleteAsync();
            }


            string jobId;
            try
            {
                // try to queue job
                var longRunArgs = new LongRunArgs<TArgs>
                {
                    Args = args,
                    LongRunInfoId = longRunInfo.Id
                };
                jobId = await EnqueueAsync<TJob, LongRunArgs<TArgs>>(longRunArgs, priority, delay);
                longRunResult.JobId = jobId;
            }
            catch (Exception e)
            {
                longRunResult.Queued = false;
                longRunResult.Error = e.ToString();
                try
                {
                    using (var unitOfWork = UnitOfWorkManager.Begin())
                    {
                        longRunInfo = await _longRunInfoRepo.GetAsync(longRunInfo.Id);
                        longRunInfo.LongRunStatus = LongRunStatus.QueueFailed;
                        longRunInfo.Error = e.ToString();
                        await unitOfWork.CompleteAsync();
                    }
                }
                catch (Exception)
                {
                    //todo: additional handler?
                }
                return longRunResult;
            }

            try
            {
                using (var unitOfWork = UnitOfWorkManager.Begin())
                {
                    longRunInfo = await _longRunInfoRepo.GetAsync(longRunInfo.Id);
                    longRunInfo.JobId = jobId;
                    await unitOfWork.CompleteAsync();
                }
            }
            catch (Exception)
            {
                //todo: additional handler?
            }
            
            return longRunResult;
        }
    }
}
