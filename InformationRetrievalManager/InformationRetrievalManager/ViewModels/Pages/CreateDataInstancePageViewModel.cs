using Microsoft.Extensions.Logging;
using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="CreateDataInstancePage"/>
    /// </summary>
    public class CreateDataInstancePageViewModel : BaseViewModel
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
        public CreateDataInstancePageViewModel()
        {

        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public CreateDataInstancePageViewModel(ILogger logger)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Command Methods

        #endregion
    }
}
