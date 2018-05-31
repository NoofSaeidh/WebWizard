using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica.Wizard
{
    [Serializable]
    public class WizardArgs : Args
    {

        [Argument("configmode", "cm")]
        public ConfigurationMode ConfigurationMode { get; set; }

        [Argument("dbsrvname", "s")]
        public string DatabaseServerName { get; set; }

        [Argument("dbname", "d")]
        public string DatabaseName { get; set; }

        [Argument("company", "c", ArgumentType = ArgumentType.Enumerable | ArgumentType.Object)]
        public List<Company> Companies { get; set; } = new List<Company>();

        [Argument("iname", "i")]
        public string InstanceName { get; set; }

        [Argument("ipath", "h")]
        public string InstancePath { get; set; }

        [Argument("swebsite", "w")]
        public string WebSiteName { get; set; }

        [Argument("svirtdir", "v")]
        public string VirtualDirectoryName { get; set; }

        [Argument("spool", "po")]
        public string ApplicationPoolName { get; set; }

        [Argument("saasmode")]
        public bool? SaasMode { get; set; }

        [Argument("vstemplates")]
        public bool? InstallVisualStudioTemplates { get; set; }

        [Argument("dbsrvtype")]
        public string DatabaseServerType { get; set; }

        [Argument("dbsrvwinauth")]
        public bool? DatabaseServerWinAuthentication { get; set; }

        [Argument("dbsrvtimeout")]
        public int? DatabaseServerTimeout { get; set; }

        [Argument("dbnew")]
        public bool? DatabaseNew { get; set; }

        [Argument("dbupdate")]
        public string DatabaseUpdate { get; set; }

        [Argument("dbmode")]
        public string DatabaseMode { get; set; }

        [Argument("dbsize")]
        public int? DatabaseSize { get; set; }

        [Argument("dbskip")]
        public bool? DatabaseSkipSetup { get; set; }

        [Argument("dbshrink")]
        public bool? DatabaseShrink { get; set; }

        [Argument("dboptimize")]
        public bool? DatabaseOptimize { get; set; }

        [Argument("dbuser")]
        public string DatabaseUser { get; set; }

        [Argument("dbpass")]
        public string DatabasePassword { get; set; }

        [Argument("dbcreatecolumnnotnull")]
        public bool? DatabaseCreateColumnNotNull { get; set; }

        [Argument("icount")]
        public int? InstanceSslCerificateTrumbprint { get; set; }

        [Argument("vmsize")]
        public string VirtualMachineSize { get; set; }

        [Argument("webserver")]
        public string WebServerName { get; set; }

        [Argument("sactions")]
        public string AspNetApplicationAccount { get; set; }

        [Argument("spoolmode")]
        public string ApplicationPoolPipelineMode { get; set; }

        [Argument("spoolauth")]
        public string ApplicationPoolAuthinticationType { get; set; }

        [Argument("dbwinauth")]
        public bool? DatabaseConnectionWinAuthintication { get; set; }

        [Argument("dbnewuser")]
        public bool? DatabaseConnectionNewUser { get; set; }

        [Argument("adminchange")]
        public bool? AdminMustChangePassword { get; set; }

        [Argument("portal")]
        public bool? DeployPortal { get; set; }

        [Argument("securemode")]
        public bool? SecureMode { get; set; }

        [Argument("file")]
        public string ConfigurationFile { get; set; }

        [Argument("instupgradebackup")]
        public bool? NoBackupInstance { get; set; }

        [Argument("logfile")]
        public string LogFile { get; set; }

        [Argument("output")]
        public string OutputMode { get; set; }

        [Argument("fulllog")]
        public bool? FullLogMode { get; set; }

        public class NewInstanceArgs
        {
            public string DatabaseServerName { get; set; }
            public string DatabaseName { get; set; }
            public bool? DatabaseConnectionWinAuthintication { get; set; }
            public bool? DatabaseConnectionNewUser { get; set; }
            public string DatabaseUser { get; set; }
            public string DatabasePassword { get; set; }
            public string InstanceName { get; set; }
            public string WebSiteName { get; set; }
            public string VirtualDirectoryName { get; set; }
            public string ApplicationPoolName { get; set; }
            public string InstancePath { get; set; }
            public List<Company> NewCompanies { get; } = new List<Company>{new Company
            {
                CompanyID = 1,
                CompanyType = "",
                LoginName = "",
                ParentID = null,
                Visible = null
            }};

            public WizardArgs ToArgs()
            {
                return new WizardArgs
                {
                    ConfigurationMode = ConfigurationMode.NewInstance,
                    DatabaseServerName = DatabaseServerName,
                    DatabaseName = DatabaseName,
                    DatabaseConnectionWinAuthintication = DatabaseConnectionWinAuthintication,
                    DatabaseConnectionNewUser = DatabaseConnectionNewUser,
                    DatabaseUser = DatabaseUser,
                    DatabasePassword = DatabasePassword,
                    InstanceName = InstanceName,
                    WebSiteName = WebSiteName,
                    VirtualDirectoryName = VirtualDirectoryName,
                    ApplicationPoolName = ApplicationPoolName,
                    InstancePath = InstancePath,
                    Companies = NewCompanies,
                };
            }
            public static explicit operator WizardArgs(NewInstanceArgs args) => args.ToArgs();
        }
    }
}
