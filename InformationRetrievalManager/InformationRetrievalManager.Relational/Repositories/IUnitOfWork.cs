using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        /// Indexed file reference repository
        /// </summary>
        IRepository<IndexedFileReferenceDataModel> IndexedFileReferences { get; }

        /// <summary>
        /// Indexed document repository
        /// </summary>
        IIndexedDocumentRepository<IndexedDocumentDataModel> IndexedDocuments { get; }

        /// <summary>
        /// Commit database context changes.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Undo database context changes.
        /// </summary>
        void UndoChanges();

        /// <summary>
        /// Undo database specific entity changes.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void UndoEntityChanges(object entity);

        /// <summary>
        /// Begin database transaction.
        /// </summary>
        /// <remarks>
        ///     If the transaction already exists, this all will no be proceeded.
        /// </remarks>
        void BeginTransaction();

        /// <summary>
        /// Commit current database transaction.
        /// </summary>
        /// <remarks>
        ///     If the transaction does not exist, this all will no be proceeded.
        /// </remarks>
        void CommitTransaction();

        /// <summary>
        /// Rollback current database transaction.
        /// </summary>
        /// <remarks>
        ///     If the transaction does not exist, this all will no be proceeded.
        /// </remarks>
        void RollbackTransaction();

        /// <summary>
        /// Method turns ON (re-enable) <see cref="ChangeTracker.AutoDetectChangesEnabled"/>! 
        /// </summary>
        void TurnOnAutoDetectChanges();

        /// <summary>
        /// Method turns OFF <see cref="ChangeTracker.AutoDetectChangesEnabled"/>! 
        /// </summary>
        void TurnOffAutoDetectChanges();

        /// <summary>
        /// Makes sure the database of the context is correctly set up
        /// </summary>
        /// <returns>Returns a task that will finish once setup is complete</returns>
        Task EnsureDatabaseCreatedAsync();
    }
}
