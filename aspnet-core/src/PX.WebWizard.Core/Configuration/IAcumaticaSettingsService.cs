namespace PX.WebWizard.Configuration
{
    public interface IAcumaticaSettingsService
    {
        AcumaticaSettings Settings { get; }

        string GetPathForInstallation(string installationName);
    }
}