using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// Stores and retrieves information about the application
    /// </summary>
    internal sealed class ApplicationStateRepository : BaseRepository<ApplicationStateDataModel>, IApplicationStateRepository
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public ApplicationStateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public ApplicationStateDataModel Get()
        {
            // Get the first column in the app state table, or the one with default values if none exist
            return _dbSet.FirstOrDefault() ?? new ApplicationStateDataModel();
        }

        /// <inheritdoc cref="IApplicationStateRepository.Insert"/>
        public override void Insert(ApplicationStateDataModel entity)
        {
            // Clear all entries
            _dbSet.RemoveRange(_dbSet);

            // Add new one
            _dbSet.Add(entity);
        }

        #endregion

        #region Override Methods (NotSupported)

        /// <summary>
        /// <para>This method is not allowed for this class!</para>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void Delete(ApplicationStateDataModel entityToDelete)
             => throw new NotSupportedException();

        /// <summary>
        /// <para>This method is not allowed for this class!</para>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void Delete(object id)
            => throw new NotSupportedException();

        /// <summary>
        /// <para>This method is not allowed for this class!</para>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override IEnumerable<ApplicationStateDataModel> Get(
            Expression<Func<ApplicationStateDataModel, bool>> filter = null,
            Func<IQueryable<ApplicationStateDataModel>, IOrderedQueryable<ApplicationStateDataModel>> orderBy = null,
            string includeProperties = "")
            => throw new NotSupportedException();

        /// <summary>
        /// <para>This method is not allowed for this class!</para>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override ApplicationStateDataModel GetByID(object id)
            => throw new NotSupportedException();

        /// <summary>
        /// <para>This method is not allowed for this class!</para>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override void Update(ApplicationStateDataModel entityToUpdate)
            => throw new NotSupportedException();

        #endregion
    }
}
