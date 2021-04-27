using Microsoft.Extensions.Logging;
using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="DataInstancePage"/>
    /// </summary>
    public class DataInstancePageViewModel : BaseViewModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Command Flags

        #endregion

        #region Commands

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataInstancePageViewModel()
        {

        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public DataInstancePageViewModel(ILogger logger)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Command Methods

        #endregion
    }
}
