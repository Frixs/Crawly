using System;
using System.Collections.Generic;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Basic index processing
    /// </summary>
    public sealed class IndexProcessing
    {
        #region Constants

        /// <summary>
        /// Alphabet with diacritics
        /// </summary>
        private const string _withDiacritics = "áÁäÄčČćĆďĎéÉěĚíÍňŇńŃóÓöÖřŘŕŔšŠśŚťŤúÚůŮüÜýÝžŽźŹ";

        /// <summary>
        /// Alphabet without diacritics
        /// </summary>
        private const string _withoutDiacritics = "aAaAcCcCdDeEeEiInNnNoOoOrRrRsSsStTuUuUuUyYzZzZ";

        #endregion

        #region Private Members

        /// <summary>
        /// Tokenizer of this processing
        /// </summary>
        private Tokenizer _tokenizer;

        /// <summary>
        /// Stemmer of this processing
        /// </summary>
        private Stemmer _stemmer;

        /// <summary>
        /// StopWord remover of this processing
        /// </summary>
        private StopWordRemover _stopWordRemover;

        /// <summary>
        /// Inverted indexation
        /// </summary>
        private IInvertedIndex _invertedIndex;

        #endregion

        #region Public Properties

        /// <summary>
        /// Read-only reference to <see cref="_invertedIndex"/>
        /// </summary>
        public IReadOnlyInvertedIndex InvertedIndex => _invertedIndex;

        /// <summary>
        /// Indicates if this processing puts the document into the lower case
        /// </summary>
        public bool ToLowerCase { get; } //; ctor

        /// <summary>
        /// Indicates if the processing removes accents before stemming or not
        /// </summary>
        public bool RemoveAccentsBeforeStemming { get; } //; ctor

        /// <summary>
        /// Indicates if the processing removes accents after stemming or not
        /// </summary>
        public bool RemoveAccentsAfterStemming { get; } //; ctor

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="tokenizer">The tokenizer for this processing</param>
        /// <param name="stemmer">The stemmer for this processing</param>
        /// <param name="stopWordRemover">The stopword remover for this processing (leave <see langword="null"/> to ignore removing stopwords)</param>
        /// <param name="toLowerCase">Should the toknes be lowercased?</param>
        /// <param name="removeAccentsBeforeStemming">Should we remove accents from tokens before stemming?</param>
        /// <param name="removeAccentsAfterStemming">Should we remove accents from tokens after stemming?</param>
        public IndexProcessing(Tokenizer tokenizer, Stemmer stemmer, StopWordRemover stopWordRemover = null, bool toLowerCase = false, bool removeAccentsBeforeStemming = false, bool removeAccentsAfterStemming = false)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            _stemmer = stemmer ?? throw new ArgumentNullException(nameof(stemmer));
            _stopWordRemover = stopWordRemover;
            _invertedIndex = new InvertedIndex();

            ToLowerCase = toLowerCase;
            RemoveAccentsBeforeStemming = removeAccentsBeforeStemming;
            RemoveAccentsAfterStemming = removeAccentsAfterStemming;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Process the documents by the processing settings and indexate it
        /// </summary>
        /// <param name="documents">Array of the documents</param>
        /// <exception cref="ArgumentNullException">If the array is null</exception>
        public void IndexDocuments(IndexDocumentDataModel[] documents)
        {
            if (documents == null)
                throw new ArgumentNullException("Array of documents not specified!");

            for (int i = 0; i < documents.Length; ++i)
                IndexDocument(documents[i]);
        }

        /// <summary>
        /// Process the document by the processing settings and indexate it
        /// </summary>
        /// <param name="document">The document</param>
        /// <exception cref="ArgumentNullException">If the document is null</exception>
        public void IndexDocument(IndexDocumentDataModel document)
        {
            if (document == null)
                throw new ArgumentNullException("Document not specified!");

            // TODO
            string docContent = document.Content;

            // To lower
            if (ToLowerCase)
                docContent = docContent.ToLower();

            // Remove newlines from the document to prepare the doc for tokenization
            docContent = docContent.Replace(Environment.NewLine, " ");

            // Remove accents before stemming
            if (RemoveAccentsBeforeStemming)
                docContent = RemoveAccents(docContent);

            // Tokenize
            var terms = _tokenizer.Tokenize(docContent);

            // Remove stopwords
            if (_stopWordRemover != null)
                terms = _stopWordRemover.Process(terms);

            // Go through all terms
            for (int i = 0; i < terms.Length; ++i)
            {
                // Stemming
                terms[i] = _stemmer.Stem(terms[i]);

                // Remove accents after stemming
                if (RemoveAccentsAfterStemming)
                    terms[i] = RemoveAccents(terms[i]);

                // Indexate it
                _invertedIndex.Put(terms[i], document.Id);
            }
        }

        /// <summary>
        /// Process the word (no sentence is expected - just a word) by the processing settings
        /// </summary>
        /// <returns>Processed word</returns>
        public string ProcessWord(string word)
        {
            // To lower
            if (ToLowerCase)
                word = word.ToLower();

            // Remove accents before stemming
            if (RemoveAccentsBeforeStemming)
                word = RemoveAccents(word);

            // Stemming
            word = _stemmer.Stem(word);

            // Remove accents after stemming
            if (RemoveAccentsAfterStemming)
                word = RemoveAccents(word);

            return word;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Remove accents from text
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>Text without accents</returns>
        private string RemoveAccents(string text)
        {
            for (int i = 0; i < _withDiacritics.Length; ++i)
                text = text.Replace(_withDiacritics[i], _withoutDiacritics[i]);
            return text;
        }

        #endregion
    }
}
