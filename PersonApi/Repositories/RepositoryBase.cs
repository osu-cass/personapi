using PersonApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PersonApi.Repositories
{
    /// <summary>
    /// Base repository implementation for working with entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which the repository is responsible.</typeparam>
    public class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
        where TKey : struct
    {
        internal PersonContext _context;
        internal DbSet<TEntity> _dbSet;

        /// <summary>
        /// Initializes a new instance of the repository.
        /// </summary>
        /// <param name="context">The context upon which the repository is based.</param>
        public RepositoryBase(PersonContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets a filtered list of the entities for which the repository is responsible.
        /// </summary>
        /// <param name="filter">An optional expression by which the entities will be filtered.</param>
        /// <param name="orderBy">An optional expression by which the entities will be ordered.</param>
        /// <param name="includeProperties">
        /// An optional comma-separated list of properties to include in the results; 
        /// otherwise null to include all properties.</param>
        /// <returns>
        /// An enumerable of the entities that match the supplied filter if provided; 
        /// otherwise returns all the entities for which the repository is responsible.</returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }


        /// <summary>
        /// Asynchronously gets a filtered list of the entities for which the repository is responsible.
        /// </summary>
        /// <param name="filter">An optional expression by which the entities will be filtered.</param>
        /// <param name="orderBy">An optional expression by which the entities will be ordered.</param>
        /// <param name="includeProperties">
        /// An optional comma-separated list of properties to include in the results; 
        /// otherwise null to include all properties.</param>
        /// <returns>
        /// An enumerable of the entities that match the supplied filter if provided; 
        /// otherwise returns all the entities for which the repository is responsible.</returns>
        public virtual Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToListAsync();
            }
            else
            {
                return query.ToListAsync();
            }
        }

        /// <summary>
        /// Gets an entity based on the supplied id.
        /// </summary>
        /// <param name="id">The id/key of the entity to be returned.</param>
        /// <returns>The entity with the matching id if one exists; otherwise null.</returns>
        public virtual TEntity GetByID(TKey id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// Asynchronously gets an entity based on the supplied id.
        /// </summary>
        /// <param name="id">The id/key of the entity to be returned.</param>
        /// <returns>The entity with the matching id if one exists; otherwise null.</returns>
        public virtual Task<TEntity> GetByIDAsync(TKey id)
        {
            return _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Inserts a new entity into the context.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        /// <summary>
        /// Deletes an entity with the matching ID if it exists.
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(TKey id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Deletes an entity with the matching ID if it exists.
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task DeleteAsync(TKey id)
        {
            TEntity entityToDelete = await _dbSet.FindAsync(id).ConfigureAwait(false);
            Delete(entityToDelete);
        }

        public virtual void DeleteAll()
        {
            _dbSet.RemoveRange(_dbSet);
        }

        /// <summary>
        /// Deletes the provided entity from the context.
        /// </summary>
        /// <param name="entityToDelete">The entity to be deleted.</param>
        /// <returns>The EntityEntry for the entity to be deleted.</returns>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Updates the provided entity in the context.
        /// </summary>
        /// <param name="entityToUpdate">The entity to be updated.</param>
        public virtual void Update(TEntity entityToUpdate)
        {
            EntityEntry<TEntity> updatedEntry = _dbSet.Attach(entityToUpdate);
            updatedEntry.State = EntityState.Modified;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}