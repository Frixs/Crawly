using System.Threading.Tasks;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Stores and retrieves information about the application
    /// </summary>
    public interface IDataStoreProvider
    {
        /// <summary>
        /// Makes sure the data store is correctly set up
        /// </summary>
        /// <returns>Returns a task that will finish once setup is complete</returns>
        Task EnsureDataStoreCreatedAsync();

        /// <summary>
        /// Gets the stored app state for this client
        /// </summary>
        /// <returns>Returns the saved app state if they exist, or the one with default values of non exist</returns>
        Task<ApplicationStateDataModel> GetAppStateAsync();

        /// <summary>
        /// Stores the given app state to the backing data store
        /// </summary>
        /// <param name="data">The app state to save</param>
        /// <returns>Returns a task that will finish once the save is complete</returns>
        Task SaveAppStateAsync(ApplicationStateDataModel data);
    }
}
