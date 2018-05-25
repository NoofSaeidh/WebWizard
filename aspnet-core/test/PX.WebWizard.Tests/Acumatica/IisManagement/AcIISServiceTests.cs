using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PX.WebWizard.Acumatica.IisManagement;
using PX.WebWizard.Configuration;
using Xunit;

namespace PX.WebWizard.Tests.Acumatica.IisManagement
{
    public class AcIisServiceTests
    {
        private AcumaticaSettings _environment = new AcumaticaSettings
        {
            DefaultSiteName = "Default Web Site",
            GlobalPath = "c:\\acumatica",
            DataFolderName = "Data"
        };

        [Fact]
        public void GetApplications_ReturnsValidApplication()
        {
            // Arrange
            var ipath = _environment.GlobalPath + "\\Feature";
            var snapshot = Mock.Of<IOptionsSnapshot<AcumaticaSettings>>(p => p.Value == _environment);
            var iapp = new IisApplication
            {
                Path = ipath + "\\App",
                Uris = new List<string>
                {
                    "http://machine/app",
                    "https://machine:533/app"
                },
                Version = "7.00.0001"
            };
            var site = new IisSite
            {
                IisApplications = Enumerable.Repeat(iapp, 1),
                Uris = new List<string>
                {
                    "http://machine/",
                    "https://machine:533/"
                }
            };
            var iismanager = Mock.Of<IIisManager>(m => m.GetIisSite(_environment.DefaultSiteName) == site);

            var filewrapper = new Mock<IFileWrapper>();
            filewrapper.Setup(f => f.GetChilds(_environment.GlobalPath)).Returns(Enumerable.Repeat(ipath, 1));
            filewrapper.Setup(f => f.GetFileName(It.IsAny<string>())).Returns<string>(Path.GetFileNameWithoutExtension);
            filewrapper.Setup(f => f.GetParentPath(It.IsAny<string>())).Returns<string>(v => Directory.GetParent(v).FullName);
            filewrapper.Setup(f => f.GetVersion(It.IsAny<string>())).Returns(iapp.Version);
            filewrapper.Setup(f => f.IsExists(It.IsAny<string>())).Returns(true);
            filewrapper.Setup(f => f.IsFolder(It.IsAny<string>())).Returns(true);


            var service = new AcIisService(snapshot, iismanager, filewrapper.Object);
            // Act

            var result = service.GetApplications().ToList();

            // Assert
            Assert.True(result.Count == 1);
            var app = result.First();

            Assert.Equal("Feature", app.InstallationName);
            Assert.Equal("App", app.Name);
            Assert.Equal(iapp.Path, app.PhysicalPath);
            Assert.Equal(iapp.Version, app.Version);
            Assert.Equal(iapp.Uris, app.Uris);
        }

        [Fact]
        public void GetApplications_DoesntReturnInvalidApplication()
        {
            // Arrange
            var snapshot = Mock.Of<IOptionsSnapshot<AcumaticaSettings>>(p => p.Value == _environment);
            var iapp = new IisApplication
            {
                Path = "C:\\site\\App",
                Uris = new List<string>
                {
                    "http://machine/app",
                    "https://machine:533/app"
                },
                Version = "7.00.0001"
            };
            var site = new IisSite
            {
                IisApplications = Enumerable.Repeat(iapp, 1),
                Uris = new List<string>
                {
                    "http://machine/",
                    "https://machine:533/"
                }
            };
            var iismanager = Mock.Of<IIisManager>(m => m.GetIisSite(_environment.DefaultSiteName) == site);

            var filewrapper = new Mock<IFileWrapper>();
            filewrapper.Setup(f => f.GetChilds(_environment.GlobalPath)).Returns(Enumerable.Empty<string>());
            filewrapper.Setup(f => f.GetFileName(It.IsAny<string>())).Returns<string>(Path.GetFileNameWithoutExtension);
            filewrapper.Setup(f => f.GetParentPath(It.IsAny<string>())).Returns<string>(v => Directory.GetParent(v).FullName);
            filewrapper.Setup(f => f.GetVersion(It.IsAny<string>())).Returns(iapp.Version);
            filewrapper.Setup(f => f.IsExists(It.IsAny<string>())).Returns(true);
            filewrapper.Setup(f => f.IsFolder(It.IsAny<string>())).Returns(true);


            var service = new AcIisService(snapshot, iismanager, filewrapper.Object);
            // Act

            var result = service.GetApplications().ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetInstallations_ReturnsValidInstallations()
        {
            // Arrange
            var snapshot = Mock.Of<IOptionsSnapshot<AcumaticaSettings>>(p => p.Value == _environment);
            var ipath = _environment.GlobalPath + "\\Feature";

            const string version = "7.00.0001";
            var iismanager = Mock.Of<IIisManager>();

            var filewrapper = new Mock<IFileWrapper>();
            filewrapper.Setup(f => f.GetChilds(_environment.GlobalPath)).Returns(Enumerable.Repeat(ipath, 1));
            filewrapper.Setup(f => f.GetFileName(It.IsAny<string>())).Returns<string>(Path.GetFileNameWithoutExtension);
            filewrapper.Setup(f => f.GetParentPath(It.IsAny<string>())).Returns<string>(v => Directory.GetParent(v).FullName);
            filewrapper.Setup(f => f.GetVersion(It.IsAny<string>())).Returns(version);
            filewrapper.Setup(f => f.IsExists(It.IsAny<string>())).Returns(true);
            filewrapper.Setup(f => f.IsFolder(It.IsAny<string>())).Returns(true);


            var service = new AcIisService(snapshot, iismanager, filewrapper.Object);
            // Act

            var result = service.GetInstallations().ToList();

            // Assert
            Assert.True(result.Count == 1);
            var inst = result.First();

            Assert.Equal("Feature", inst.Name);
            Assert.Equal(ipath, inst.PhysicalPath);
            Assert.Equal(version, inst.Version);
            Assert.Equal(Path.Combine(ipath, _environment.DataFolderName, _environment.WizardFileName), inst.WizardPath);
            Assert.Equal(Path.Combine(ipath, _environment.DataFolderName, _environment.AcExeFileName), inst.AcExePath);
        }

        [Fact]
        public void GetInstallations_DoesntReturnInvalidInstallations()
        {
            // Arrange
            var snapshot = Mock.Of<IOptionsSnapshot<AcumaticaSettings>>(p => p.Value == _environment);
            var ipath1 = _environment.GlobalPath + "\\Feature";

            var ipath2 = "C:\\otherspace";

            var iismanager = Mock.Of<IIisManager>();

            var filewrapper = new Mock<IFileWrapper>();
            filewrapper.Setup(f => f.GetChilds(_environment.GlobalPath)).Returns(new List<string> { ipath1, ipath2 });
            filewrapper.Setup(f => f.GetFileName(It.IsAny<string>())).Returns<string>(Path.GetFileNameWithoutExtension);
            filewrapper.Setup(f => f.GetParentPath(It.IsAny<string>())).Returns<string>(v => Directory.GetParent(v).FullName);
            filewrapper.Setup(f => f.GetVersion(It.IsAny<string>())).Returns((string)null);
            filewrapper.Setup(f => f.IsExists(It.IsAny<string>())).Returns(false);
            filewrapper.Setup(f => f.IsFolder(It.IsAny<string>())).Returns(false);


            var service = new AcIisService(snapshot, iismanager, filewrapper.Object);
            // Act

            var result = service.GetInstallations().ToList();

            // Assert
            Assert.Empty(result);
 
        }
    }
}
