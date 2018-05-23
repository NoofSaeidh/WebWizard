using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AcWebTool.Core.Configuration
{
    public class AcEnvironment
    {
        public string GlobalPath { get; set; }

        public string WizardFileName { get; set; }
        public string AcExeFileName { get; set; }
        public string DefaultSiteName { get; set; }
        public string DataFolderName { get; set; }
        public IList<string> InstallationLocations { get; set; }
        public string InstallationPackageName { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string ServerName { get; set; }
        public string DatabasePrefix { get; set; }
        public bool? DatabaseWinAuth { get; set; }
        public string ApplicationPool { get; set; }
    }
}
