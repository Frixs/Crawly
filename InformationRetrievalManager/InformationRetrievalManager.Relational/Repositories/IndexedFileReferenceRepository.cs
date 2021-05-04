namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Stores and retrieves data about indexed file references
    /// </summary>
    internal sealed class IndexedFileReferenceRepository : BaseRepository<IndexedFileReferenceDataModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public IndexedFileReferenceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}
