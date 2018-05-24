using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public class Application
    {
        public string Name { get; set; }

        public IList<string> Uris { get; set; }

        public string Version { get; set; }

        public string PhysicalPath { get; set; }

        public string Http
        {
            get
            {
                var https = Uris.Where(x => x.Contains("http://")).ToList();
                if (https.Count == 1) return https[0];
                if (https.Count == 0) return null;
                return https.FirstOrDefault(x => !x.Contains(":")) ?? https.First();
            }
        }

        public string InstallationName {get; set;}

        public bool IsValid => !string.IsNullOrEmpty(Name)
                && !string.IsNullOrEmpty(Version)
                && !string.IsNullOrEmpty(PhysicalPath)
                && !string.IsNullOrEmpty(InstallationName)
                && Uris != null;

        public override string ToString() => Name;
    }
}

