using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.Queue
{
    public enum JobStatus
    {
        Queued,
        InProcess,
        Done,
        Aborted,
        Failed,
    }
}
