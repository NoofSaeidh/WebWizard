using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcWebTool.Core.Queue;
using Microsoft.Extensions.DependencyInjection;

namespace AcWebTool.Core.DataAccess
{
    public class AcWTUnit : IAcWTUnit
    {
        private readonly Dictionary<Type, object> _repos;
        public AcWTUnit(IServiceProvider provider)
        {
            //todo: autofill with reflection
            _repos = new Dictionary<Type, object>
            {
                [typeof(DownloadJobInfo)] = DownloadJobRepository = provider.GetService<IRepository<DownloadJobInfo>>(),
                [typeof(AcExeJobInfo)] = AcExeJobRepository = provider.GetService<IRepository<AcExeJobInfo>>(),
            };
        }

        public IRepository<DownloadJobInfo> DownloadJobRepository { get; }
        public IRepository<AcExeJobInfo> AcExeJobRepository { get; }

        public IRepository<T> GetRepository<T>() where T : IEntity
        {
            if (_repos.TryGetValue(typeof(T), out var value))
                return (IRepository<T>)value;
            throw new InvalidOperationException($"{typeof(T)} repository is not initialized.");
        }


    }
}
