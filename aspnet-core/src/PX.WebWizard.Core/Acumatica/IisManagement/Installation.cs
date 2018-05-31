using System.Collections.Generic;
using System.IO;


namespace PX.WebWizard.Acumatica.IisManagement
{
    public class Installation
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string PhysicalPath { get; set; }
        public string AcExePath { get; set; }
        public string WizardPath { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(AcExePath)
            && !string.IsNullOrEmpty(WizardPath)
            && !string.IsNullOrEmpty(Version)
            && !string.IsNullOrEmpty(PhysicalPath)
            && !string.IsNullOrEmpty(Name);

        public override string ToString() => Name;
    }
}
