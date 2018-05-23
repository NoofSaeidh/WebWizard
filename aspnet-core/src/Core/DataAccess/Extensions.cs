using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddInMemoryRepository<T>(this IServiceCollection services) where T : IEntity
        {
            return services.AddScoped<IRepository<T>>(_ => new InMemoryRepository<T>());
        }

        public static IServiceCollection AddAllInMemoryRepositories(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepository<>), typeof(InMemoryRepository<>));
        }

        public static IServiceCollection AddJsonStorageRepository<T>(this IServiceCollection services, string location) where T : IEntity
        {
            return services.AddScoped<IRepository<T>>(_ => new JsonStorageRepository<T>(location));
        }

        public static IServiceCollection AddAllJsonStorageRepositories(this IServiceCollection services, string baseLocation)
        {
            //todo: cannot be resolved???
            //TODO: ask github!
            return services.AddScoped(typeof(IRepository<>), provider =>
            {
                
                return null;
            });
        }

        public static IServiceCollection AddAcWTUnit(this IServiceCollection services)
        {
            return services.AddScoped<IAcWTUnit, AcWTUnit>();
        }
    }
}
