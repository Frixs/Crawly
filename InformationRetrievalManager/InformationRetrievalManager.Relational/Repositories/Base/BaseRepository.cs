using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Database repository with general control.
    /// </summary>
    /// <typeparam name="TEntity">Data model representing data set of the repository</typeparam>
    internal abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        #region Protected Members

        /// <summary>
        /// The database context
        /// </summary>
        protected ApplicationDbContext _dbContext;

        /// <summary>
        /// The database set of this repository
        /// </summary>
        protected DbSet<TEntity> _dbSet;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database context</param>
        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);

            _dbSet.Remove(entityToDelete);
        }

        /// <inheritdoc/>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            params string[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply filters (if any)...
            if (filter != null)
                query = query.Where(filter);

            // Apply includes (if any)...
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                    query = query.Include(includeProperty);
            }

            // Apply order (if set) and return the results...
            if (orderBy != null)
                return orderBy(query).ToList();
            // Otherwise, just return the results...
            else
                return query.ToList();
        }

        /// <inheritdoc/>
        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        /// <inheritdoc/>
        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        /// <inheritdoc/>
        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Use raw SQL to query a database.
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="parameters">Parameters of the query</param>
        /// <returns>The query results</returns>
        protected IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return _dbSet.FromSqlRaw(query, parameters).ToList();
        }

        #endregion
    }
}
