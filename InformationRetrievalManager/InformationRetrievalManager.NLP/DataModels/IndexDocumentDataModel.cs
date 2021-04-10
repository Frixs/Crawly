using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Defines the structure of the document data
    /// </summary>
    public class IndexDocumentDataModel
    {
        #region Public Properties

        /// <summary>
        /// Document ID
        /// </summary>
        public int Id { get; set; }

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


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor to initialize the whole structure
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     - <paramref name="id"/> is a negative number. 
        ///     - <paramref name="title"/> is null.
        ///     - <paramref name="content"/> is null.
        /// </exception>
        public IndexDocumentDataModel(int id, string title, string category, DateTime timestamp, string content)
        {
            if (id < 0) throw new ArgumentNullException("Invalid document ID!");
            if (title == null) throw new ArgumentNullException("Invalid document title!");
            if (content == null) throw new ArgumentNullException("Invalid document content!");

            Id = id;
            Title = title;
            Category = category;
            Timestamp = timestamp;
            Content = content;
        }

        #endregion
    }
}
