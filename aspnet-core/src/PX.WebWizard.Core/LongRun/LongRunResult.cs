using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    public class LongRunResult
    {
        public string Error { get; set; }
        public bool Queued { get; set; }
        public string LongRunId { get; set; }
        public string JobId { get; set; }
        // for working process. all just queued should be abortable
        public bool Abortable { get; set; }
    }
}
