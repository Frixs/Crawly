namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Stores and retrieves processing configuration data
    /// </summary>
    internal sealed class IndexProcessingConfigurationRepository : BaseRepository<IndexProcessingConfigurationDataModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public IndexProcessingConfigurationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}
