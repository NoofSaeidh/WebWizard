using AcWebTool.Core.DataAccess;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.Queue
{
    public interface IQueueJobBuilder<T> where T : IJobInfo
    {
        IJobDetail Build();
        T GetInfo();
    }
    public interface IQueueManager
    {
        Task Queue<T>(IQueueJobBuilder<T> builder) where T : IJobInfo;
    }
    public interface IJobInfo : IEntity
    {
        JobStatus Status { get; set; }
        string Message { get; set; }
        string Error { get; set; }
    }
}
