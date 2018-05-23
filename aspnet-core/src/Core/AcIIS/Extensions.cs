using AcWebTool.Core.AcIIS.IISManagement;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.AcIIS
{
    public static class AcIISExtensions
    {
        public static IServiceCollection AddAcIISServices(this IServiceCollection services)
        {
            return services.AddScoped<IAcIISService, AcIISService>()
                           .AddScoped<IIISManager, IISManager>();

        }
    }
}
