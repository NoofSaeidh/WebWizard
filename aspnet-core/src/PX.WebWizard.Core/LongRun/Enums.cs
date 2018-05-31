using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    public enum LongRunStatus
    {
        Queued,
        InProcess,
        Done,
        Aborted,
        QueueAborted,
        Failed,
        QueueFailed,
    }

    public enum LongRunAbortStatus
    {
        ExecutionAborted,
        QueueAborted,
        AbortFailed,
        NotAbortable,
    }
}
