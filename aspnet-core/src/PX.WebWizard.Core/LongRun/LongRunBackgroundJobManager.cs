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

        [UnitOfWork]
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

        [UnitOfWork]
        public async Task<LongRunResult> EnqueueLongRunAsync<TJob, TArgs>(TArgs args,
            BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : LongRunBackgroundJob<TArgs>
        {
            LongRunInfo longRunInfo;
            var longRunResult = new LongRunResult();
            try
            {
                // try persist info about operation to db

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
                await _longRunInfoRepo.InsertAsync(longRunInfo);
            }
            catch (Exception e)
            {
                // if fail return info about error
                longRunResult.Queued = false;
                longRunResult.Error = e.ToString();
                return longRunResult;
            }

            var resultExceptions = new List<Exception>();
            try
            {
                // try to queue job
                var longRunArgs = new LongRunArgs<TArgs>
                {
                    Args = args,
                    LongRunInfoId = longRunInfo.Id
                };
                var jobId = await EnqueueAsync<TJob, LongRunArgs<TArgs>>(longRunArgs, priority, delay);
                // if succed return info with id's of db info and job id
                longRunResult.Queued = true;
                longRunResult.JobId = longRunInfo.JobId = jobId;
            }
            catch (Exception e)
            {
                // if failed return error
                longRunResult.Queued = false;
                resultExceptions.Add(e);
                longRunInfo.Error = e.ToString();
                longRunInfo.LongRunStatus = LongRunStatus.QueueFailed;

            }

            try
            {
                await _longRunInfoRepo.UpdateAsync(longRunInfo);
            }
            catch (Exception e)
            {
                resultExceptions.Add(e);
            }

            if (resultExceptions.Any())
                longRunResult.Error = new AggregateException(resultExceptions).ToString();

            return longRunResult;
        }
    }
}
