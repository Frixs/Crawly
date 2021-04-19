using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.NLP;

namespace InformationRetrievalManager
{
    public static class CrawlerExtensions
    {
        /// <summary>
        /// Transform crawler data <see cref="CrawlerDataModel"/> into <see cref="IndexDocumentDataModel"/>
        /// </summary>
        /// <param name="model">The crawler model <see cref="CrawlerDataModel"/></param>
        /// <returns>Index data model with initialized ID</returns>
        public static IndexDocumentDataModel ToIndexDocument(this CrawlerDataModel model, int id)
        {
            return new IndexDocumentDataModel(id, model.Title, model.SourceUrl, model.Category, model.Timestamp, model.Content);
        }
    }
}
