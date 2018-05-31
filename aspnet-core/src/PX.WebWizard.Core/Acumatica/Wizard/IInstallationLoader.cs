namespace PX.WebWizard.Acumatica.Wizard
{
    public interface IInstallationLoader
    {
        void UnpackInstallationPackage(string packagePath, string resultPath, bool overwrite = true);
        void FindAndUnpackInstallationPackage(string version, string resultPath, bool overwrite = true);
        string FindInstallationPackage(string version);
        bool TryFindInstallationPackage(string version, out string path);
        bool TryFindAndUnpackInstallationPackage(string version, string resultPath, bool overwrite = true);
    }
}