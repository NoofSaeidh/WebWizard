using System.Collections.Generic;
using System.Threading;

namespace PX.WebWizard.LongRun
{
    public interface ICancellationWorker
    {
        IReadOnlyDictionary<object, CancellationToken> Tokens { get; }

        CancellationToken Add(object id);
        CancellationToken AddOrGet(object id);
        bool RequestCancellation(object id, bool throwOnFirstException = false);
        bool Remove(object id);
    }
}