using System;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Public repository interface for <see cref="IndexedDocumentRepository"/>
    /// </summary>
    public interface IIndexedDocumentRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets the highest (latest) document timestamp.
        /// </summary>
        /// <returns>Latest indexed document timestamp.</returns>
        DateTime GetMaxTimestamp();
    }
}
