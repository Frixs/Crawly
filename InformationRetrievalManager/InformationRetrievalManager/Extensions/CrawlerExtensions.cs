using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.NLP;

namespace InformationRetrievalManager
{
    public static class CrawlerExtensions
    {
        /// <summary>
        /// Transform crawler data <see cref="CrawlerDataModel"/> into <see cref="IndexDocument"/>
        /// </summary>
        /// <param name="model">The crawler model <see cref="CrawlerDataModel"/></param>
        /// <returns>Index data model with initialized ID</returns>
        public static IndexDocument ToIndexDocument(this CrawlerDataModel model, long id)
        {
            return new IndexDocument(id, model.Title, model.SourceUrl, model.Category, model.Timestamp, model.Content);
        }
    }
}
