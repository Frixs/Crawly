
using System;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Defines the structure for storaged crawler data in organized foramt (e.g. JSON)
    /// </summary>
    public class CrawlerDataModel
    {
        /// <summary>
        /// Title (of specific article)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Category (of specific article)
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Timestamp (of specific article)
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Content (of specific article)
        /// </summary>
        public string Content { get; set; }
    }
}
