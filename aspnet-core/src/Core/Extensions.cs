using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AcWebTool.Core.AcIIS;
using AcWebTool.Core.AcIIS.IISManagement;
using AcWebTool.Core.AcExe;
using AcWebTool.Core.Queue;
using AcWebTool.Core.DataAccess;
using System.IO;

namespace AcWebTool.Core
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            //todo: where it should be?
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Storage");

            return services.AddFileWrapper()
                           .AddAcExeServices()
                           .AddAcIISServices()
                           .AddQueueServices()
                           .AddAcWTUnit()
                           .AddJsonStorageRepository<DownloadJobInfo>(Path.Combine(folder, nameof(DownloadJobInfo)))
                           .AddJsonStorageRepository<AcExeJobInfo>(Path.Combine(folder, nameof(AcExeJobInfo)))
                           .AddQueueServices();
        }


        public static IServiceCollection AddFileWrapper(this IServiceCollection services)
        {
            return services.AddSingleton<IFileWrapper, FileWrapper>();
        }

    }
}
