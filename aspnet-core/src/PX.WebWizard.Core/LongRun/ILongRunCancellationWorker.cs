using System.Collections.Generic;
using System.Threading;

namespace PX.WebWizard.LongRun
{
    public interface ILongRunCancellationWorker
    {
        IReadOnlyDictionary<string, CancellationToken> Tokens { get; }

        void PushLongRun(string longRunId);
    }
}