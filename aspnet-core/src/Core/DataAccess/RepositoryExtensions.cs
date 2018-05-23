using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.DataAccess
{
    public static class RepositoryExtensions
    {
        public static void Delete<T>(this IRepository<T> repository, T entity) where T : IEntity
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            repository.Delete(entity.Id);
        }
        public static void UpdateWith<T>(this IRepository<T> repository, string id, Action<T> update) where T :IEntity
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            var entity = repository.Get(id);
            update(entity);
            repository.Update(entity);
        }
    }
}
