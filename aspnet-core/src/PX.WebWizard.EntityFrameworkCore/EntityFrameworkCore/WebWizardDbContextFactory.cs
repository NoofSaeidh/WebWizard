using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PX.WebWizard.Configuration;
using PX.WebWizard.Web;

namespace PX.WebWizard.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class WebWizardDbContextFactory : IDesignTimeDbContextFactory<WebWizardDbContext>
    {
        public WebWizardDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<WebWizardDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            WebWizardDbContextConfigurer.Configure(builder, configuration.GetConnectionString(WebWizardConsts.ConnectionStringName));

            return new WebWizardDbContext(builder.Options);
        }
    }
}
