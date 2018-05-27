using Abp.Dependency;
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
    public class WizardRunnerJobArgs
    {
        public string WizardArgs { get; set; }
        public string AcExePath { get; set; }
    }

    public class WizardRunnerJob : LongRunBackgroundJob<WizardRunnerJobArgs>, ITransientDependency
    {
        private readonly IWizardRunner _wizardRunner;

        public WizardRunnerJob(IWizardRunner wizardRunner)
        {
            _wizardRunner = wizardRunner;
        }

        public override bool Abortable => true;

        protected override LongRunStatus? ExecuteRaw(WizardRunnerJobArgs args, LongRunInfo info, CancellationToken cancellationToken)
        {
            LongRunStatus result = LongRunStatus.Done;
            _wizardRunner.RunAcExeAsync(args.AcExePath, args.WizardArgs,
                (s, e) =>
                {
                    PersistProgress(new PersistProgressArgs { LongRunId = info.Id, Data = e.Data });
                },
                (s, e) =>
                {
                    PersistProgress(new PersistProgressArgs { LongRunId = info.Id, Error = e.Data });
                    result = LongRunStatus.Failed;
                },
                cancellationToken)
                .Wait();
            return result;
        }
    }
}
