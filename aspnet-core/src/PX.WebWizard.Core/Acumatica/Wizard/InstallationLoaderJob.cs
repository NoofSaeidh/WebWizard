using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using PX.WebWizard.LongRun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica.Wizard
{
    [Serializable]
    public class InstallationLoaderJobArgs
    {
        public string Version { get; set; }
        public string ResultPath { get; set; }
    }

    public class InstallationLoaderJob : LongRunBackgroundJob<InstallationLoaderJobArgs>, ITransientDependency
    {
        private readonly IInstallationLoader _installationLoader;

        public InstallationLoaderJob(IInstallationLoader installationLoader)
        {
            _installationLoader = installationLoader;
        }

        public override bool Abortable => false;

        protected override LongRunStatus? ExecuteRaw(InstallationLoaderJobArgs args, LongRunInfo info, CancellationToken cancellationToken)
        {
            if (!_installationLoader.TryFindInstallationPackage(args.Version, out var path))
            {
                info.Error = $"Cannot find specified version: {args.Version}.";
                return LongRunStatus.Failed;
            }

            _installationLoader.UnpackInstallationPackage(path, args.ResultPath);
            // todo: messages
            return LongRunStatus.Done;
        }
    }
}
