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
    [UnitOfWork(IsDisabled = true)]
    public abstract class LongRunBackgroundJob<T> : BackgroundJob<LongRunArgs<T>>
    {
        public abstract bool Abortable { get; }

        public IRepository<LongRunInfo, string> LongRunInfoRepository { get; set; }

        public ICancellationWorker CancellationWorker { get; set; }

        protected void PersistProgress(PersistProgressArgs args)
        {
            if (args == null || args.LongRunId == null)
                throw new ArgumentException(nameof(args));
            if (args.Data == null && args.Error == null)
                return;

            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                var longRunInfo = LongRunInfoRepository.Get(args.LongRunId);

                if (args.Data != null)
                    longRunInfo.Message += args.Data;
                if (args.Error != null)
                    longRunInfo.Error += 
                        string.IsNullOrEmpty(longRunInfo.Error)
                        ? args.Error
                        : Environment.NewLine + args.Error;

                unitOfWork.Complete();
            }
        }

        public override void Execute(LongRunArgs<T> args)
        {
            LongRunInfo longRunInfo;
            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                longRunInfo = LongRunInfoRepository.Get(args.LongRunInfoId);
                if (longRunInfo.LongRunStatus != LongRunStatus.Queued)
                    return;

                longRunInfo.LongRunStatus = LongRunStatus.InProcess;
                longRunInfo.Abortable = Abortable;
                unitOfWork.Complete();
            }
            LongRunStatus? result = null;
            CancellationToken token;
            if (Abortable)
            {
                token = CancellationWorker.AddOrGet(longRunInfo.Id);
            }
            else
            {
                token = default;
            }

            string error = null;
            try
            {
                result = ExecuteRaw(args.Args, longRunInfo, token);
            }
            catch (Exception e)
            {
                error = e.ToString();
            }
            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                longRunInfo = LongRunInfoRepository.Get(longRunInfo.Id);
                if (error != null)
                    longRunInfo.Error += 
                        string.IsNullOrEmpty(longRunInfo.Error)
                        ? error
                        : Environment.NewLine + error;
                if (result != null)
                    longRunInfo.LongRunStatus = result.Value;
                unitOfWork.Complete();
            }
            CancellationWorker.Remove(longRunInfo.Id);
        }

        // token only for Abortable Jobs
        protected abstract LongRunStatus? ExecuteRaw(T args, LongRunInfo info, CancellationToken cancellationToken);

        protected class PersistProgressArgs
        {
            public string LongRunId { get; set; }
            public string Data { get; set; }
            public string Error { get; set; }
        }
    }
}
