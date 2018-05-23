using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AcWebTool.Core.DataAccess
{
    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private static ConcurrentDictionary<string, T> items = new ConcurrentDictionary<string, T>();

        private static ConcurrentDictionary<string, object> locks = new ConcurrentDictionary<string, object>();

        public InMemoryRepository()
        {

        }

        public bool Delete(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return items.TryRemove(id, out _);
        }

        public T Get(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return items[id];
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) return GetAll();

            return items.Values.Where(x => predicate.Compile()(x));
        }

        public IEnumerable<T> GetAll() => items.Values.Select(x => x);

        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //todo: do not change existing entity?
            if (string.IsNullOrEmpty(entity.Id)) entity.Id = GenerateId();
            if (!items.TryAdd(entity.Id, entity))
            {
                throw new DuplicateKeyException();

            }
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidIdException();

            if (!items.TryGetValue(entity.Id, out var value))
                throw new KeyNotFoundException();

            if (!items.TryUpdate(entity.Id, entity, value))
                throw new UpdateException();
        }

        public bool Upsert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrEmpty(entity.Id)) entity.Id = GenerateId();
            var newvalue = true;
            items.AddOrUpdate(entity.Id, entity, (id, v) => { newvalue = false; return entity; });
            return newvalue;
        }

        public bool Clear()
        {
            if (items.IsEmpty) return false;
            items.Clear();
            return true;
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
