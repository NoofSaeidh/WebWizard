using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace AcWebTool.Core.Queue
{
    public abstract class QueueJobBuilder<T, U> : IQueueJobBuilder<T> 
        where T : IJobInfo, new()
        where U : IJob
    {
        private readonly string _id = Guid.NewGuid().ToString();
        public IJobDetail Build()
        {
            return JobBuilder.Create<U>()
                .WithIdentity(_id)
                .Build();
        }
        public virtual T GetInfo()
        {
            return new T
            {
                Id = _id
            };
        }
    }
}
