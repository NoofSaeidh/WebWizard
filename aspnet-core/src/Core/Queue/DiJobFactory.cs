using Quartz;
using Quartz.Spi;
using System;
using System.Reflection;

namespace AcWebTool.Core.Queue
{
    public class DiJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DiJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;

            ////todo: remove?
            //foreach (var property in job.GetType()
            //    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
            //{
            //    //todo: should check camelCase?
            //    if(bundle.JobDetail.JobDataMap.ContainsKey(property.Name))
            //    {
            //        try
            //        {
            //            property.SetValue(job, bundle.JobDetail.JobDataMap[property.Name]);
            //        }
            //        catch
            //        {
            //            continue;
            //        }
            //    }
            //}

            return job;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
