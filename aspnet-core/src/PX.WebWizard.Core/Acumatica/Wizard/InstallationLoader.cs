using PX.WebWizard.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace PX.WebWizard.Acumatica.Wizard
{
    public class InstallationLoader
    {
        public InstallationLoader(IOptionsSnapshot<AcumaticaSettings> acEnvironment)
        {
            Environment = acEnvironment.Value;
        }
        public AcumaticaSettings Environment { get; }

        /// <summary>
        ///     Find package with Acumatica version.
        /// </summary>
        /// <param name="version">Acumatica Version.</param>
        /// <returns>Path to Acumatica package.</returns>
        public string FindAcPackage(string version)
        {
            foreach (var packageLocation in Environment.InstallationLocations 
                ?? throw new InvalidOperationException($"{nameof(Environment.InstallationLocations)} is not specified."))
            {
                foreach (var dir in Directory.EnumerateDirectories(packageLocation, version, SearchOption.AllDirectories))
                {
                    var file = Directory.EnumerateFiles(dir, Environment.InstallationPackageName, SearchOption.AllDirectories).SingleOrDefault();
                    if (file != null) return file;               
                }
            }
            throw new FileNotFoundException("Acumatica package not found.", version);
        }

        public void CopyAndUnzipAcPackage(string packagePath, string resultPath, bool overwrite = true)
        {
            //todo: should it be temp folder?
            var tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            //this to not to unzip on remote server (very slow)
            File.Copy(packagePath, tmp, false);
            ZipFile.ExtractToDirectory(tmp, resultPath, overwrite);
            File.Delete(tmp);
        }

        public void FindCopyAndUnzipAcPackage(string version, string resultFolderPath, bool overwrite = true)
            => CopyAndUnzipAcPackage(FindAcPackage(version), resultFolderPath, overwrite);
    }
}
