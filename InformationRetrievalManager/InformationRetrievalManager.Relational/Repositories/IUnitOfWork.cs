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
        /// Commit database context changes
        /// </summary>
        void Commit();

        /// <summary>
        /// Makes sure the database of the context is correctly set up
        /// </summary>
        /// <returns>Returns a task that will finish once setup is complete</returns>
        Task EnsureDatabaseCreatedAsync();
    }
}
