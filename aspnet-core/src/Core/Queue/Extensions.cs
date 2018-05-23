using AcWebTool.Core.AcIIS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.Queue
{
    public static class ScheduleExtensions
    {
        public static IServiceCollection AddQueueServices(this IServiceCollection services)
        {
            services.AddSchedulerServices();
            services.AddSingleton<IQueueManager, QueueManager>();
            services.AddJobs();

            return services;
        }

        public static IServiceCollection AddJobs(this IServiceCollection services)
        {
            return services
                .AddTransient<DownloadJob>()
                .AddTransient<AcExeJob>();
        }

        public static IServiceCollection AddSchedulerServices(this IServiceCollection services)
        {
            //Todo: tmp
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };

            services.AddSingleton<IScheduler>(factory =>
            {
                var schFactory = new StdSchedulerFactory(props);
                var sch = schFactory.GetScheduler().Result;
                sch.JobFactory = new DiJobFactory(factory.GetService<IServiceProvider>());
                sch.Start();
                var lifitime = factory.GetService<IApplicationLifetime>();
                lifitime.ApplicationStopping.Register(() => sch.Shutdown());
                return sch;
            });

            return services;
        }
    }

    //Todo: tmp
    class ConsoleLogProvider : ILogProvider
    {
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (level >= LogLevel.Info && func != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                }
                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
