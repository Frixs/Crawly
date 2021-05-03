using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Defines the structure of the document data
    /// </summary>
    public class IndexDocument
    {
        #region Public Properties

        /// <summary>
        /// Document ID reference
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Document title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Document category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Document timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Document content (body)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Document's source URL
        /// </summary>
        public string SourceUrl { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor to initialize the whole structure
        /// </summary>
        /// <param name="id">Id of the document</param>
        /// <param name="title">Title of the document</param>
        /// <param name="sourceUrl">Source URL of the document</param>
        /// <param name="category">Category of the document</param>
        /// <param name="timestamp">Timestamp of the document</param>
        /// <param name="content">Content of the document</param>
        /// <exception cref="ArgumentNullException">
        ///     - <paramref name="id"/> is a negative number. 
        ///     - <paramref name="title"/> is null.
        ///     - <paramref name="sourceUrl"/> is null.
        ///     - <paramref name="content"/> is null.
        /// </exception>
        public IndexDocument(long id, string title, string sourceUrl, string category, DateTime timestamp, string content)
        {
            if (id < 0) throw new ArgumentNullException("Invalid document ID!");

            Id = id;
            Title = title ?? throw new ArgumentNullException("Invalid document title!");
            SourceUrl = sourceUrl ?? throw new ArgumentNullException("Invalid document source URL!");
            Category = category;
            Timestamp = timestamp;
            Content = content ?? throw new ArgumentNullException("Invalid document content!");
        }

        #endregion
    }
}
