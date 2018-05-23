using AcWebTool.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.Queue
{
    public abstract class JobInfo : Entity, IJobInfo
    {
        public JobStatus Status { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return $"Id = {Id}, Status = {Status}";
        }
    }
}
