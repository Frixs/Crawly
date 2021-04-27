namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Stores and retrieves crawler configuration data
    /// </summary>
    internal sealed class CrawlerConfigurationRepository : BaseRepository<CrawlerConfigurationDataModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public CrawlerConfigurationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}
