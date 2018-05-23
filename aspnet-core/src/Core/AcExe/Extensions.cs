using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.AcExe
{
    public static class AcExeExtensions
    {
        public static IServiceCollection AddAcExeServices(this IServiceCollection services)
        {
            return services.AddScoped<InstallationLoader>()
                           .AddSingleton<AcExeRunner>();

        }
    }
}
