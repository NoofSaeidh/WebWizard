using AcWebTool.Core.AcExe;
using AcWebTool.Core.DataAccess;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcWebTool.Core.Queue
{
    public class AcExeJob : QueueJob<AcExeJobInfo>
    {
        public AcExeJob(IRepository<AcExeJobInfo> repository, AcExeRunner acExeRunner) : base(repository)
        {
            AcExeRunner = acExeRunner;
        }

        protected AcExeRunner AcExeRunner { get; }

        protected override async Task ExecuteUnhandled(IJobExecutionContext context, AcExeJobInfo info)
        {
            var tokenSource = new CancellationTokenSource();
            bool completed = false;
            var task = new Task(() =>
            {
                while (!tokenSource.IsCancellationRequested && !completed)
                {
                    var entity = Repository.Get(Id);
                    if (entity.Status == JobStatus.Aborted)
                        tokenSource.Cancel();
                    Thread.Sleep(16000);
                }
            });

            var result = AcExeRunner.RunAcExe(info.Path, info.Args
                , (o, e) => Repository.UpdateWith(Id, entity => entity.Message += e.Data?.Replace("\0", ""))
                , (o, e) => Repository.UpdateWith(Id, entity => entity.Error += e.Data?.Replace("\0", ""))
                , tokenSource.Token);

            task.Start();
            try
            {
                await result;
            }
            finally
            {
                completed = true;
            }
        }
    }

    public class AcExeJobBuilder : QueueJobBuilder<AcExeJobInfo, AcExeJob>
    {
        private readonly string _path;
        private readonly string _args;
        private string _installation;
        private string _application;

        public AcExeJobBuilder(string path, string args)
        {
            _path = path;
            _args = args;
        }

        public AcExeJobBuilder SetInstallation(string value)
        {
            _installation = value;
            return this;
        }
        public AcExeJobBuilder SetApplication(string value)
        {
            _application = value;
            return this;
        }

        public override AcExeJobInfo GetInfo()
        {
            var res = base.GetInfo();
            res.Args = _args;
            res.Path = _path;
            res.Installation = _installation;
            res.Application = _application;
            return res;
        }

        public static AcExeJobBuilder FromArgs(string path, AcExeArgs args)
        {
            return new AcExeJobBuilder(path, args.Serialize());
        }
    }

    public class AcExeJobInfo : JobInfo, IJobInfo
    {
        public string Path { get; set; }
        public string Args { get; set; }
        public string Installation { get; set; }
        public string Application { get; set; }
    }
}
