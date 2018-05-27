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

        public ILongRunCancellationWorker LongRunCancellationWorker { get; set; }

        protected void PersistProgress(PersistProgressArgs args)
        {
            if (args == null || args.LongRunId == null)
                throw new ArgumentException(nameof(args));

            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                var longRunInfo = LongRunInfoRepository.Get(args.LongRunId);

                if (args.Data != null)
                    longRunInfo.Message += Environment.NewLine + args.Data;
                if (args.Error != null)
                    longRunInfo.Error += Environment.NewLine + args.Error;

                unitOfWork.Complete();
            }
        }

        public override void Execute(LongRunArgs<T> args)
        {
            LongRunInfo longRunInfo;
            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                longRunInfo = LongRunInfoRepository.Get(args.LongRunInfoId);
                if (longRunInfo.LongRunStatus == LongRunStatus.QueueAborted
                    || longRunInfo.LongRunStatus == LongRunStatus.Aborted)
                    return;

                longRunInfo.LongRunStatus = LongRunStatus.InProcess;
                longRunInfo.Abortable = Abortable;
                unitOfWork.Complete();
            }
            LongRunStatus? result = null;
            CancellationToken token;
            if (Abortable)
            {
                LongRunCancellationWorker.PushLongRun(longRunInfo.Id);
                token = LongRunCancellationWorker.Tokens[longRunInfo.Id];
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
                if(error != null)
                {
                    longRunInfo.Error += error;
                }
                if (result != null)
                {
                    longRunInfo.LongRunStatus = result.Value;
                }
                unitOfWork.Complete();
            }
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
