using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.Configuration
{
    public abstract class AcumaticaBaseSettings
    {
        public virtual string WizardFileName => "Acumatica Wizard.exe";
        public virtual string AcExeFileName => "ac.exe";
        public virtual string DataFolderName => "Data";

    }

    [JsonObject(ItemRequired = Required.Always)]
    public class AcumaticaSettings : AcumaticaBaseSettings
    {
        public string GlobalPath { get; set; }
        public string DefaultSiteName { get; set; }
        public IList<string> InstallationLocations { get; set; }
        public string InstallationPackageNamePattern { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string ServerName { get; set; }
        public string DatabasePrefix { get; set; }
        public string ApplicationPool { get; set; }

        [JsonProperty(Required = Required.Default)]
        public bool? DatabaseWinAuth { get; set; }
    }
}
