using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersonApi.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class
        where TKey : struct
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetByID(TKey id);

        Task<TEntity> GetByIDAsync(TKey id);

        void Insert(TEntity entity);

        void InsertRange(IEnumerable<TEntity> entities);

        void Delete(TKey id);

        Task DeleteAsync(TKey id);

        void Delete(TEntity entityToDelete);

        void DeleteAll();

        void Update(TEntity entityToUpdate);

        void UpdateRange(IEnumerable<TEntity> entities);

        Task<int> SaveChangesAsync();
    }
}