using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    public abstract class LongRunBackgroundJob<T> : BackgroundJob<LongRunArgs<T>>, ITransientDependency
    {
        public abstract bool Abortable { get; }

        public IRepository<LongRunInfo, string> LongRunInfoRepository { get; set; }

        public ILongRunCancellationWorker LongRunCancellationWorker { get; set; }

        [UnitOfWork]
        public override void Execute(LongRunArgs<T> args)
        {
            var longRunInfo = LongRunInfoRepository.Get(args.LongRunInfoId);
            if (longRunInfo.LongRunStatus == LongRunStatus.QueueAborted
                || longRunInfo.LongRunStatus == LongRunStatus.Aborted)
                return;

            longRunInfo.LongRunStatus = LongRunStatus.InProcess;
            longRunInfo.Abortable = Abortable;
            LongRunInfoRepository.Update(longRunInfo);
            CurrentUnitOfWork.SaveChanges();

            LongRunStatus? result = null;
            CancellationToken token;
            if(Abortable)
            {
                LongRunCancellationWorker.PushLongRun(longRunInfo.Id);
                token = LongRunCancellationWorker.Tokens[longRunInfo.Id];
            }
            else
            {
                token = default;
            }

            try
            {
                result = ExecuteRaw(args.Args, longRunInfo, token);
            }
            catch(Exception e)
            {
                longRunInfo.LongRunStatus = LongRunStatus.Failed;
                longRunInfo.Error = e.ToString();
                LongRunInfoRepository.Update(longRunInfo);
                return;
            }
            if(result != null)
            {
                longRunInfo.LongRunStatus = result.Value;
                LongRunInfoRepository.Update(longRunInfo);
            }

        }

        // token only for Abortable Jobs
        protected abstract LongRunStatus? ExecuteRaw(T args, LongRunInfo info, CancellationToken cancellationToken);
    }
}
