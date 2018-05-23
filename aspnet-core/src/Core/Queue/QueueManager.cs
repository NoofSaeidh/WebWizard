using AcWebTool.Core.AcExe;
using AcWebTool.Core.DataAccess;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.Queue
{
    public class QueueManager : IQueueManager
    {
        private readonly IScheduler _scheduler;
        private readonly IAcWTUnit _unit;

        public QueueManager(IScheduler scheduler, IAcWTUnit unit)
        {
            _scheduler = scheduler;
            _unit = unit;
        }

        public async Task Queue<T>(IQueueJobBuilder<T> builder) where T : IJobInfo
        {
            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();

            var job = builder.Build();

            var repo = _unit.GetRepository<T>();

            var info = builder.GetInfo();
            info.Status = JobStatus.Queued;
            repo.Insert(info);
            await _scheduler.ScheduleJob(job, trigger);
        }
    }

    public static class QueueManagerExtensions
    {
        public static async Task QueueDownload(this IQueueManager manager, string version, string folder)
        {
            await manager.Queue(new DownloadJobBuilder(version, folder));
        }

        //todo: remove or rework
        //public static async Task QueueAcExe(this IQueueManager manager, string path, AcExeArgs args)
        //{
        //    await manager.Queue(AcExeJobBuilder.FromArgs(path, args));
        //}
        //public static async Task QueueAcExe(this IQueueManager manager, string path, string args)
        //{
        //    await manager.Queue(new AcExeJobBuilder(path, args));
        //}
    }
}
