
using System;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Crawler Engine specific event arguments
    /// </summary>
    public class CrawlerEngineEventArgs : EventArgs
    {
        /// <summary>
        /// Event argument for <see cref="ICrawlerEngine.CrawlingProgressPct"/>
        /// </summary>
        public short CrawlingProgressPct { get; set; }
    }
}
