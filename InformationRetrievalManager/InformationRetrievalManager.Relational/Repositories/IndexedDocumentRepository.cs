using System;
using System.Linq;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Stores and retrieves data about indexed documents
    /// </summary>
    internal sealed class IndexedDocumentRepository : BaseRepository<IndexedDocumentDataModel>, IIndexedDocumentRepository<IndexedDocumentDataModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public IndexedDocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public DateTime GetMaxTimestamp()
        {
            return _dbSet.Max(o => o.Timestamp);
        }

        #endregion
    }
}
