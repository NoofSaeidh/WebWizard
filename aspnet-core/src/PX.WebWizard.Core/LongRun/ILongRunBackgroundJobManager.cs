using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;

namespace PX.WebWizard.LongRun
{
    public interface ILongRunBackgroundJobManager
    {
        Task<LongRunAbortResult> AbortLongRunAsync(string longRunId);

        Task<LongRunResult> EnqueueLongRunAsync<TJob, TArgs>(TArgs args,
            BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : LongRunBackgroundJob<TArgs>;

    }
}