using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// General database repository interface
    /// </summary>
    /// <typeparam name="TEntity">Data model representing data set of the repository</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Delete the entity by the entity instance
        /// </summary>
        /// <param name="entityToDelete">The entity</param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Delete entity by ID
        /// </summary>
        /// <param name="id">The id</param>
        void Delete(object id);

        /// <summary>
        /// Get entity(ies) by query parameters
        /// </summary>
        /// <param name="filter">Filter query</param>
        /// <param name="orderBy">Order query</param>
        /// <param name="includeProperties">Include properties separated with space (',')</param>
        /// <returns>The entities</returns>
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "");

        /// <summary>
        /// Get an entity by ID
        /// </summary>
        /// <param name="id">The ID</param>
        /// <returns>The entity</returns>
        TEntity GetByID(object id);

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity">The entity</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entityToUpdate">The updated entity</param>
        void Update(TEntity entityToUpdate);
    }
}
