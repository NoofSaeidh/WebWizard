using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace PX.WebWizard.EntityFrameworkCore
{
    public static class WebWizardDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<WebWizardDbContext> builder, string connectionString)
        {
            builder.UseSqlite(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<WebWizardDbContext> builder, DbConnection connection)
        {
            builder.UseSqlite(connection);
        }
    }
}
