using Microsoft.EntityFrameworkCore;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The database context for the application data store
    /// "This is our database representation"
    /// </summary>
    sealed class DataStoreDbContext : DbContext
    {
        #region DbSets (Tables)

        /// <summary>
        /// The state data of the application
        /// </summary>
        public DbSet<ApplicationStateDataModel> ApplicationState { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataStoreDbContext(DbContextOptions<DataStoreDbContext> options) : base(options)
        {
        }

        #endregion

        #region Model Creating

        /// <summary>
        /// Configures the database structure and relationships
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API

            // Configure App State
            // ------------------------------
            //
            // Set Id as primary key
            modelBuilder.Entity<ApplicationStateDataModel>().HasKey(a => a.Id);
        }

        #endregion
    }
}
