using Ixs.DNA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        /// <summary>
        /// Injects the data store into the application construction.
        /// </summary>
        /// <param name="construction">The construction</param>
        /// <returns>The construction reference for further chaining</returns>
        public static FrameworkConstruction AddDataStore(this FrameworkConstruction construction)
        {
            // Inject our SQLite EF data store
            construction.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Setup connection string
                options.UseSqlite("Data Source=data.db");
            }, contextLifetime: ServiceLifetime.Transient);

            // Add data store for easy access/use of the backing data store
            // Make it scoped so we can inject the scoped DbContext
            construction.Services.AddTransient<IUnitOfWork>(provider => new UnitOfWork(provider.GetService<ApplicationDbContext>()));

            // Return framewrok for chaining
            return construction;
        }
    }
}
