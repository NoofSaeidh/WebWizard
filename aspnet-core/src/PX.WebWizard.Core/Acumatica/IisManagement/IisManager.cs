using Abp.Dependency;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PX.WebWizard.Acumatica.IisManagement
{
    public class IisManager : IIisManager, ISingletonDependency
    {
        public IisSite GetIisSite(string siteName)
        {
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites[siteName];

                var uris = site.Bindings
                               .Select(b => new Uri($"{b.Protocol}://{System.Environment.MachineName}:{b.EndPoint.Port}"));

                var apps = new List<IisApplication>(site.Applications.Count);

                foreach (var app in site.Applications)
                {
                    var resApp = new IisApplication
                    {
                        Path = app.VirtualDirectories.FirstOrDefault()?.PhysicalPath,
                        Uris = uris.Select(u => new Uri(u, app.Path).ToString()).ToList()
                    };

                    try
                    {
                        /* Get Version in Web.Config from appSettings section */
                        resApp.Version = serverManager
                            .GetWebConfiguration(siteName, app.Path)
                            .GetSection("appSettings")
                            .GetCollection()
                            .Select(e => e.Attributes)
                            .Select(a => new
                            {
                                key = a.FirstOrDefault(_a => _a.Name == "key").Value?.ToString()
                                ,
                                value = a.FirstOrDefault(_a => _a.Name == "value").Value?.ToString()
                            })
                            .FirstOrDefault(pair => pair.key == "Version")
                            ?.value;
                    }
                    catch
                    {
                        resApp.Version = null;
                    }

                    apps.Add(resApp);
                }

                return new IisSite
                {
                    Uris = uris.Select(x => x.ToString()),
                    IisApplications = apps
                };
            }
        }
    }
}
