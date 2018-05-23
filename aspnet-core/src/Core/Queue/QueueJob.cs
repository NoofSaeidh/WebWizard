using AcWebTool.Core.DataAccess;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.Queue
{
    public abstract class QueueJob<T> : IJob where T : IJobInfo
    {
        protected QueueJob(IRepository<T> repository)
        {
            Repository = repository;
        }

        protected IRepository<T> Repository { get; }
        protected string Id { get; private set; }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Id = context.JobDetail.Key.Name;
                Repository.UpdateWith(Id, e => e.Status = JobStatus.InProcess);

                await ExecuteUnhandled(context, Repository.Get(Id));

                Repository.UpdateWith(Id, e => e.Status = JobStatus.Done);

            }
            catch (Exception ex)
            {
                Repository.UpdateWith(Id, e => { e.Status = JobStatus.Done; e.Error = ex.ToString(); });

                if (ex is JobExecutionException) throw;
                throw new JobExecutionException(ex);
            }
        }

        protected abstract Task ExecuteUnhandled(IJobExecutionContext context, T info);
    }
}
