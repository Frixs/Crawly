using System.Linq;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Provider for the application database
    /// </summary>
    class DataStoreProvider : IDataStoreProvider
    {
        #region Protected Members

        /// <summary>
        /// The database context for the client data store
        /// </summary>
        protected DataStoreDbContext mDbContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public DataStoreProvider(DataStoreDbContext dbContext)
        {
            mDbContext = dbContext;
        }

        #endregion

        #region Interface Methods

        #region Base

        /// <inheritdoc/>
        public async Task EnsureDataStoreCreatedAsync()
        {
            // Make sure the database exists and is created
            await mDbContext.Database.EnsureCreatedAsync();
        }

        #endregion

        /// <inheritdoc/>
        public Task<ApplicationStateDataModel> GetAppStateAsync()
        {
            // Get the first column in the app state table, or the one with default values if none exist
            return Task.FromResult(
                mDbContext.ApplicationState.FirstOrDefault() ?? new ApplicationStateDataModel()
                );
        }

        /// <inheritdoc/>
        public async Task SaveAppStateAsync(ApplicationStateDataModel data)
        {
            // Clear all entries
            mDbContext.ApplicationState.RemoveRange(mDbContext.ApplicationState);

            // Add new one
            mDbContext.ApplicationState.Add(data);

            // Save changes
            await mDbContext.SaveChangesAsync();
        }

        #endregion
    }
}
