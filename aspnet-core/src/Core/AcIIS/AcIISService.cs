using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AcWebTool.Core.Configuration;
using AcWebTool.Core.AcIIS.IISManagement;
using Microsoft.Extensions.Options;

namespace AcWebTool.Core.AcIIS
{
    public class AcIISService : IAcIISService
    {
        private readonly AcEnvironment _environment;
        private readonly IIISManager _iisManager;
        private readonly IFileWrapper _fileWrapper;
        public AcIISService(IOptionsSnapshot<AcEnvironment> acEnvironment, IIISManager iisManager, IFileWrapper fileWrapper)
        {
            _environment = acEnvironment.Value;
            _iisManager = iisManager;
            _fileWrapper = fileWrapper;
        }

        public IEnumerable<Application> GetAllApplications()
        {
            var site = _iisManager.GetIISSite(_environment.DefaultSiteName);

            var installations = GetInstallations().ToList();

            foreach (var app in site.IISApplications)
            {
                /* Skip apps that in different directory from Ac installation's folder in config */
                if (!app.Path.StartsWith(_environment.GlobalPath, StringComparison.OrdinalIgnoreCase))
                    continue;

                var acapp = new Application
                {
                    Name = _fileWrapper.GetFileName(app.Path),
                    Uris = app.Uris,
                    PhysicalPath = app.Path,
                    Version = app.Version,
                    InstallationName = installations.FirstOrDefault(i => _fileWrapper.GetParentPath(app.Path).Equals(i?.PhysicalPath, StringComparison.OrdinalIgnoreCase))?.Name
                };

                yield return acapp;
            }
        }

        public IEnumerable<Application> GetApplications()
        {
            foreach (var item in GetAllApplications())
            {
                if (item.IsValid) yield return item;
            }
        }

        public IEnumerable<Installation> GetAllInstallations()
        {
            foreach (var dir in _fileWrapper.GetChilds(_environment.GlobalPath))
            {
                var name = _fileWrapper.GetFileName(dir);
                if (!_fileWrapper.IsExists(dir) || !_fileWrapper.IsFolder(dir)) continue;

                var acexe = Path.Combine(dir, _environment.DataFolderName, _environment.AcExeFileName);
                var wizard = Path.Combine(dir, _environment.DataFolderName, _environment.WizardFileName);


                yield return new Installation
                {
                    AcExePath = _fileWrapper.IsExists(acexe) ? acexe : null,
                    WizardPath = _fileWrapper.IsExists(wizard) ? wizard : null,
                    PhysicalPath = dir,
                    Name = name,
                    Version = _fileWrapper.GetVersion(acexe)
                };
            }
        }

        public IEnumerable<Installation> GetInstallations()
        {
            foreach (var item in GetAllInstallations())
            {
                if (item.IsValid) yield return item;
            }
        }
    }
}
