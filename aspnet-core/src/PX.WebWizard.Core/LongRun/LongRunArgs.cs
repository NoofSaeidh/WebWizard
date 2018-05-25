using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    [Serializable]
    public class LongRunArgs<TArg>
    {
        public TArg Args { get; set; }
        public string LongRunInfoId { get; set; }
    }
}
