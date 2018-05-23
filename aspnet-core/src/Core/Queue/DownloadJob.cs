using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcWebTool.Core.AcExe;
using AcWebTool.Core.DataAccess;
using Quartz;

namespace AcWebTool.Core.Queue
{
    public class DownloadJob : QueueJob<DownloadJobInfo>
    {
        public DownloadJob(IRepository<DownloadJobInfo> repository
            , InstallationLoader loader) : base(repository)
        {
            Loader = loader;
        }

        protected InstallationLoader Loader { get; }


        protected override Task ExecuteUnhandled(IJobExecutionContext context, DownloadJobInfo info)
        {
            Loader.FindCopyAndUnzipAcPackage(info.Version, info.Folder);
            return Task.FromResult("Download successed.");

        }
    }
    public class DownloadJobBuilder : QueueJobBuilder<DownloadJobInfo, DownloadJob>
    {
        private readonly string _folder;
        private readonly string _version;
        //todo: should use not constructor for this?
        //.WithVersion(..) as example
        public DownloadJobBuilder(string version, string folder)
        {
            _version = version;
            _folder = folder;
        }

        public override DownloadJobInfo GetInfo()
        {
            var res = base.GetInfo();
            res.Version = _version;
            res.Folder = _folder;
            return res;
        }
    }

    public class DownloadJobInfo : JobInfo, IJobInfo
    {
        public string Version { get; set; }
        public string Folder { get; set; }
    }
}
