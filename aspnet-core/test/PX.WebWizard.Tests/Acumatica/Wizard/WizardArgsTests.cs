using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PX.WebWizard.Acumatica.Wizard;

namespace PX.WebWizard.Tests.Acumatica.Wizard
{
    public class WizardArgsTests
    {
        [Fact]
        public void NewInstance_SerializeRight()
        {
            // Arange
            var args = new WizardArgs.NewInstanceArgs
            {
                //todo
            };
                
            //    .NewInstance("NUFF", "demo", "demo", @"C:\acumatica\demo\\"
            //    , "Default Web Site", "demo", "DefaultAppPool"
            //    , new Company { CompanyID = 2, CompanyType = "SalesDemo", ParentID = 1, Visible = true, LoginName = "Demo" });
            //// Act
            //var result = args.Serialize();
            //// Assert
            //Assert.Contains("-configmode:\"NewInstance\"", result);
            //Assert.Contains("-dbsrvname:\"NUFF\"", result);
            //Assert.Contains("-dbname:\"demo\"", result);
            //Assert.Contains("-iname:\"demo\"", result);
            //Assert.Contains("-ipath:\"C:\\acumatica\\demo\\\\\"", result);
            //Assert.Contains("-swebsite:\"Default Web Site\"", result);
            //Assert.Contains("-svirtdir:\"demo\"", result);
            //Assert.Contains("-spool:\"DefaultAppPool\"", result);
            //Assert.Contains("-company:\"CompanyID=1;CompanyType=;LoginName=;\"", result);
            //Assert.Contains("-company:\"CompanyID=2;CompanyType=SalesDemo;ParentID=1;Visible=True;LoginName=Demo;\"", result);
        }
    }
}
