namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Public repository interface for <see cref="ApplicationStateRepository"/>
    /// </summary>
    public interface IApplicationStateRepository
    {
        /// <summary>
        /// Get the application state information
        /// </summary>
        /// <returns>The application state entity model</returns>
        ApplicationStateDataModel Get();

        /// <summary>
        /// Insert a new copy of application state into database. 
        /// <para>
        ///     The method always keep the database with 1 record only. 
        ///     It clears the database before each insert.
        /// </para>
        /// </summary>
        /// <param name="entity">The application state entity</param>
        void Insert(ApplicationStateDataModel entity);
    }
}
