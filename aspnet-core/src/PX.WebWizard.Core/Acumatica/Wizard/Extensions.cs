using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica.Wizard
{
    public static class AcExeExtensions
    {
        public static IServiceCollection AddAcExeServices(this IServiceCollection services)
        {
            return services.AddScoped<InstallationLoader>()
                           .AddSingleton<WizardRunner>();

        }
    }
}
