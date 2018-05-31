using Abp.Dependency;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.Configuration
{
    public class AcumaticaSettingsService : ITransientDependency, IAcumaticaSettingsService
    {
        public AcumaticaSettingsService(IOptionsSnapshot<AcumaticaSettings> settings)
        {
            Settings = settings.Value;
        }

        public AcumaticaSettings Settings { get; }

        public string GetPathForInstallation(string installationName)
        {
            return Path.Combine(Settings.GlobalPath, installationName);
        }
    }
}
