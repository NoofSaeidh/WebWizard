using AcWebTool.Core.Queue;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AcWebTool.Core.DataAccess
{
    public interface IRepository<T> where T : IEntity
    {
        T Get(string id);

        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetAll();

        void Insert(T entity);

        void Update(T entity);

        //true if new created
        bool Upsert(T entity);

        //false if not exist
        bool Delete(string id);

        //false if empty
        bool Clear();
    }

    public interface IRepositoryUnit
    {
        IRepository<T> GetRepository<T>() where T : IEntity;
    }

    public interface IAcWTUnit : IRepositoryUnit
    {
        IRepository<DownloadJobInfo> DownloadJobRepository { get; }
        IRepository<AcExeJobInfo> AcExeJobRepository { get; }
    }

    public interface IEntity
    {
        string Id { get; set; }
    }
}