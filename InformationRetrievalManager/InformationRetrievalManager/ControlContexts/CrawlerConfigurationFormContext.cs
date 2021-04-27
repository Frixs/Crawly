using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Context for control <see cref="CrawlerConfigurationForm"/>.
    /// </summary>
    public class CrawlerConfigurationFormContext
    {
        #region Public Properties

        public TextEntryViewModel SiteAddress { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrawlerConfigurationFormContext()
        {

        }

        #endregion
    }
}
