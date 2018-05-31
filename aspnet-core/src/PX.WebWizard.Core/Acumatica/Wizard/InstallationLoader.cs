    using PX.WebWizard.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Abp.Dependency;

namespace PX.WebWizard.Acumatica.Wizard
{
    //todo: abortable unzips (and copying)
    public class InstallationLoader : IInstallationLoader, ITransientDependency
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
        public string FindInstallationPackage(string version)
        {
            if (TryFindInstallationPackage(version, out var result))
                return result;
            throw new FileNotFoundException("Acumatica package not found.", version);
        }

        public void UnpackInstallationPackage(string packagePath, string resultPath, bool overwrite = true)
        {
            //todo: should it be temp folder?
            var tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            //this to not to unzip on remote server (very slow)
            File.Copy(packagePath, tmp, false);
            try
            {
                ZipFile.ExtractToDirectory(tmp, resultPath, overwrite);
            }
            finally
            {
                File.Delete(tmp);
            }
        }

        public void FindAndUnpackInstallationPackage(string version, string resultFolderPath, bool overwrite = true)
            => UnpackInstallationPackage(FindInstallationPackage(version), resultFolderPath, overwrite);

        public bool TryFindInstallationPackage(string version, out string path)
        {
            string result = null;
            foreach (var packageLocation in Environment.InstallationLocations
                ?? throw new InvalidOperationException($"{nameof(Environment.InstallationLocations)} is not specified."))
            {
                foreach (var dir in Directory.EnumerateDirectories(packageLocation, version, SearchOption.AllDirectories))
                {
                    var file = Directory.EnumerateFiles(dir, Environment.InstallationPackageNamePattern, SearchOption.AllDirectories).SingleOrDefault();
                    if (file != null)
                    {
                        result = file;
                        break;
                    }
                }
            }
            if(result != null)
            {
                path = null;
                return false;
            }
            path = result;
            return true;
        }

        public bool TryFindAndUnpackInstallationPackage(string version, string resultPath, bool overwrite = true)
        {
            if (!TryFindInstallationPackage(version, out var path))
                return false;
            try
            {
                UnpackInstallationPackage(path, resultPath, overwrite);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
