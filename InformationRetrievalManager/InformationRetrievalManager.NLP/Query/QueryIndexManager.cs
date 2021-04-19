using System;
using System.Collections.Generic;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Manager for querying data above idnexed data.
    /// </summary>
    public sealed class QueryIndexManager : IQueryIndexManager
    {
        #region Private Members (Models)

        /// <summary>
        /// TF-IDF model
        /// </summary>
        private readonly TfIdf _modelTfIdf = new TfIdf();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryIndexManager()
        {
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public int[] Query(string query, IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data, QueryModelType modelType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
