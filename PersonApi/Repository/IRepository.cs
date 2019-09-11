using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Repository
{
    public interface IRepository<T>
    {
        T GetById(Int64 id);
        void Create(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
