using System;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Stores and retrieves data instances created by user
    /// </summary>
    internal class DataInstanceRepository : BaseRepository<DataInstanceDataModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public DataInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}
