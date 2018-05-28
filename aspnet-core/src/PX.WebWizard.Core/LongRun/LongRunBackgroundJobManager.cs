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
        private readonly ICancellationWorker _cancellationWorker;

        public LongRunBackgroundJobManager(IIocResolver iocResolver,
            IBackgroundJobStore store, AbpTimer timer,
            IRepository<LongRunInfo, string> longRunInfoRepo,
            IKeyGenerator keyGenerator,
            ICancellationWorker cancellationWorker)
            : base(iocResolver, store, timer)
        {
            _longRunInfoRepo = longRunInfoRepo;
            _keyGenerator = keyGenerator;
            _cancellationWorker = cancellationWorker;
        }

        public async Task<LongRunAbortResult> AbortLongRunAsync(string longRunId)
        {
            LongRunInfo longRunInfo;
            longRunInfo = await _longRunInfoRepo.GetAsync(longRunId);


            // check if background worker didn't start operation and try to delete from queue 
            if (!string.IsNullOrEmpty(longRunInfo.JobId)
                && longRunInfo.LongRunStatus == LongRunStatus.Queued)
            {
                var aborted = await DeleteAsync(longRunInfo.JobId);
                if (aborted)
                {
                    using (var unitOfWork = UnitOfWorkManager.Begin())
                    {
                        longRunInfo.LongRunStatus = LongRunStatus.QueueAborted;
                        await unitOfWork.CompleteAsync();
                    }

                    return new LongRunAbortResult
                    {
                        AbortStatus = LongRunAbortStatus.QueueAborted
                    };
                }
            }

            if (!longRunInfo.Abortable)
            {
                return new LongRunAbortResult
                {
                    AbortStatus = LongRunAbortStatus.NotAbortable
                };
            }

            var result = _cancellationWorker.RequestCancellation(longRunId);
            if (result)
            {
                using (var unitOfWork = UnitOfWorkManager.Begin())
                {
                    longRunInfo.LongRunStatus = LongRunStatus.Aborted;
                    await unitOfWork.CompleteAsync();
                }

                return new LongRunAbortResult
                {
                    AbortStatus = LongRunAbortStatus.QueueAborted
                };
            }
            return new LongRunAbortResult
            {
                AbortStatus = LongRunAbortStatus.AbortFailed
            };
        }

        public async Task<LongRunResult> EnqueueLongRunAsync<TJob, TArgs>(TArgs args,
            BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : LongRunBackgroundJob<TArgs>
        {
            LongRunInfo longRunInfo;
            string longRunId;
            // i removed try catches for persisting.
            // let it be handled by unit of work and thorwing exceptions
            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                longRunId = _keyGenerator.Generate<string>();
                longRunInfo = new LongRunInfo
                {
                    Id = longRunId,
                    //todo: serializer? or json repo?
                    Args = JsonConvert.SerializeObject(args),
                    LongRunStatus = LongRunStatus.Queued,
                    Type = typeof(TJob).ToString(),
                };
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
                    LongRunInfoId = longRunId
                };
                jobId = await EnqueueAsync<TJob, LongRunArgs<TArgs>>(longRunArgs, priority, delay);
            }
            catch (Exception e)
            {
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
                throw;
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

            return new LongRunResult
            {
                LongRunId = longRunId,
                JobId = jobId
            };
        }
    }
}
