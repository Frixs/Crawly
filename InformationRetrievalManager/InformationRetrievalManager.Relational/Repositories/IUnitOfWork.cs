using System.Threading.Tasks;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Unit-of-Work pattern interface with related repositories.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Application state repository
        /// </summary>
        IApplicationStateRepository ApplicationState { get; }

        /// <summary>
        /// Data instance repository
        /// </summary>
        IRepository<DataInstanceDataModel> DataInstances { get; }

        /// <summary>
        /// Crawler configuration repository
        /// </summary>
        IRepository<CrawlerConfigurationDataModel> CrawlerConfigurations { get; }

        /// <summary>
        /// Index processing configuration repository
        /// </summary>
        IRepository<IndexProcessingConfigurationDataModel> IndexProcessingConfigurations { get; }

        /// <summary>
        /// Indexed document repository
        /// </summary>
        IRepository<IndexedDocumentDataModel> IndexedDocuments { get; }

        /// <summary>
        /// Commit database context changes.
        /// </summary>
        void Commit();

        /// <summary>
        /// Begin database transaction.
        /// </summary>
        /// <remarks>
        ///     If the transaction already exists, this all will be ignored.
        /// </remarks>
        void BeginTransaction();

        /// <summary>
        /// Commit current database transaction.
        /// </summary>
        /// <remarks>
        ///     If the transaction does not exist, this all will be ignored.
        /// </remarks>
        void CommitTransaction();

        /// <summary>
        /// Rollback current database transaction.
        /// </summary>
        /// <remarks>
        ///     If the transaction does not exist, this all will be ignored.
        /// </remarks>
        void RollbackTransaction();

        /// <summary>
        /// Makes sure the database of the context is correctly set up
        /// </summary>
        /// <returns>Returns a task that will finish once setup is complete</returns>
        Task EnsureDatabaseCreatedAsync();
    }
}
