
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

        /// <summary>
        /// Event argument for progress text feedback message.
        /// </summary>
        public string CrawlingProgressMsg { get; set; }

        /// <summary>
        /// Event argument for progress URL of currently processed web page.
        /// </summary>
        public string CrawlingProgressUrl { get; set; }
    }
}
