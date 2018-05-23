using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace AcWebTool.Core.DataAccess
{
    public abstract class FileStorageRepository<T> : IRepository<T> where T : IEntity
    {
        private static object lockObject = new object();

        protected FileStorageRepository(string fileLocation)
        {
            Location = fileLocation ?? throw new ArgumentNullException(nameof(fileLocation));
            lock (lockObject)
            {
                if (!Directory.Exists(Location))
                    Directory.CreateDirectory(Location);
            }
        }

        protected string Location { get; }

        public bool Clear()
        {
            lock (lockObject)
            {
                if (!Directory.EnumerateFiles(Location).Any())
                    return false;

                Directory.Delete(Location, true);
                Directory.CreateDirectory(Location);
                return true;
            }
        }

        public bool Delete(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            lock (lockObject)
            {
                if (!File.Exists(GetFullFileName(id)))
                    return false;
                File.Delete(GetFullFileName(id));
                return true;
            }
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) return GetAll();
            return GetAll().Where(predicate.Compile());
        }

        public IEnumerable<T> GetAll()
        {
            string[] files;
            lock (lockObject)
            {
                files = Directory.GetFiles(Location);
            }

            foreach (var item in files)
            {
                yield return Parse(File.ReadAllText(item));
            }
        }

        public T Get(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var filename = GetFullFileName(id);
            lock (lockObject)
            {
                if (!File.Exists(filename))
                    return default;
                var value = File.ReadAllText(filename);
                return Parse(value);
            }
        }

        public void Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            lock (lockObject)
            {
                if (entity.Id == null) entity.Id = GenerateId();
                else if (File.Exists(GetFullFileName(entity.Id)))
                    throw new InvalidOperationException("Entity already exists in storage.");

                File.WriteAllText(GetFullFileName(entity.Id), Serialize(entity));
            }
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            lock (lockObject)
            {
                if (entity.Id == null)
                    throw new ArgumentException($"{nameof(entity.Id)} property of argument {nameof(entity)} is not specified");
                else if (!File.Exists(GetFullFileName(entity.Id)))
                    throw new InvalidOperationException("Entity doesn't exist in storage.");

                File.WriteAllText(GetFullFileName(entity.Id), Serialize(entity));
            }
        }

        public bool Upsert(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            lock (lockObject)
            {
                if (entity.Id == null) entity.Id = GenerateId();

                var result = !File.Exists(GetFullFileName(entity.Id));

                File.WriteAllText(GetFullFileName(entity.Id), Serialize(entity));

                return result;
            }
        }

        protected virtual string GetFullFileName(string id) => Path.Combine(Location, GetFileName(id));

        protected virtual string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        protected abstract T Parse(string value);

        protected abstract string Serialize(T value);

        protected abstract string GetFileName(string id);
    }
}