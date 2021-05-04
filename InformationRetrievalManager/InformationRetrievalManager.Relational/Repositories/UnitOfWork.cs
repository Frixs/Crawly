using Ixs.DNA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The unit of work class serves one purpose: to make sure that when you use multiple repositories, they share a single database context. 
    /// That way, when a unit of work is complete you can call the <see cref="SaveChanges"/> method on that instance of the context and be assured that all related changes will be coordinated.
    /// </summary>
    internal sealed class UnitOfWork : IUnitOfWork
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Private Members

        /// <summary>
        /// The database context (DI inject)
        /// </summary>
        private ApplicationDbContext _dbContext;

        /// <summary>
        /// Private reference of property <see cref="ApplicationState"/>
        /// </summary>
        private IApplicationStateRepository _applicationState;

        /// <summary>
        /// Private reference of property <see cref="DataInstances"/>
        /// </summary>
        private IRepository<DataInstanceDataModel> _dataInstances;

        /// <summary>
        /// Private reference of property <see cref="CrawlerConfigurations"/>
        /// </summary>
        private IRepository<CrawlerConfigurationDataModel> _crawlerConfigurations;

        /// <summary>
        /// Private reference of property <see cref="IndexProcessingConfigurations"/>
        /// </summary>
        private IRepository<IndexProcessingConfigurationDataModel> _indexProcessingConfigurations;

        /// <summary>
        /// Private reference of property <see cref="IndexedDocuments"/>
        /// </summary>
        private IRepository<IndexedDocumentDataModel> _indexedDocuments;

        #endregion

        #region Public Properties

        /// <inheritdoc/>
        public IApplicationStateRepository ApplicationState =>
            _applicationState ??
                (_applicationState = new ApplicationStateRepository(_dbContext));

        /// <inheritdoc/>
        public IRepository<DataInstanceDataModel> DataInstances =>
            _dataInstances ??
                (_dataInstances = new DataInstanceRepository(_dbContext));

        /// <inheritdoc/>
        public IRepository<CrawlerConfigurationDataModel> CrawlerConfigurations =>
            _crawlerConfigurations ??
                (_crawlerConfigurations = new CrawlerConfigurationRepository(_dbContext));

        /// <inheritdoc/>
        public IRepository<IndexProcessingConfigurationDataModel> IndexProcessingConfigurations =>
            _indexProcessingConfigurations ??
                (_indexProcessingConfigurations = new IndexProcessingConfigurationRepository(_dbContext));

        /// <inheritdoc/>
        public IRepository<IndexedDocumentDataModel> IndexedDocuments =>
            _indexedDocuments ??
                (_indexedDocuments = new IndexedDocumentRepository(_dbContext));

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UnitOfWork(ApplicationDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public void SaveChanges()
        {
            int n = _dbContext.SaveChanges();
            _logger.LogTraceSource($"Total of {n} database changes saved!");
        }

        /// <inheritdoc/>
        public void UndoChanges()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
            _logger.LogTraceSource($"Database changes rollbacked!");
        }

        /// <inheritdoc/>
        public void UndoEntityChanges(object entity)
        {
            var entry = _dbContext.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.Reload();
                    break;
                default: break;
            }
            _logger.LogTraceSource($"Database changes for '{entity.GetType()}' rollbacked!");
        }

        /// <inheritdoc/>
        public void BeginTransaction()
        {
            if (_dbContext.Database.CurrentTransaction != null)
                return;

            _dbContext.Database.BeginTransaction();
            _logger.LogDebugSource($"Database transaction has started!");
        }

        /// <inheritdoc/>
        public void CommitTransaction()
        {
            if (_dbContext.Database.CurrentTransaction == null)
                return;

            _dbContext.Database.CommitTransaction();
            _logger.LogDebugSource($"Database transaction committed!");
        }

        /// <inheritdoc/>
        public void RollbackTransaction()
        {
            if (_dbContext.Database.CurrentTransaction == null)
                return;

            _dbContext.Database.RollbackTransaction();
            _logger.LogDebugSource($"Database transaction roll-backed!");
        }

        /// <inheritdoc/>
        public async Task EnsureDatabaseCreatedAsync()
        {
            // Make sure the database exists and is created
            await _dbContext.Database.EnsureCreatedAsync();
        }

        #endregion
    }
}
