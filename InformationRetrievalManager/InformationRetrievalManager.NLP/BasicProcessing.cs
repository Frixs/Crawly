using System;
using System.Collections.Generic;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Basic processing (NLP)
    /// </summary>
    public class BasicProcessing
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
        /// Dictionary of everything indexed via this processing
        /// </summary>
        private Dictionary<string, int> _wordFrequencies = new Dictionary<string, int>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Read-only reference to <see cref="_wordFrequencies"/>
        /// </summary>
        public IReadOnlyDictionary<string, int> WordFrequencies => _wordFrequencies;

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
        public BasicProcessing(Tokenizer tokenizer, Stemmer stemmer, StopWordRemover stopWordRemover = null, bool toLowerCase = false, bool removeAccentsBeforeStemming = false, bool removeAccentsAfterStemming = false)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            _stemmer = stemmer ?? throw new ArgumentNullException(nameof(stemmer));
            _stopWordRemover = stopWordRemover;

            ToLowerCase = toLowerCase;
            RemoveAccentsBeforeStemming = removeAccentsBeforeStemming;
            RemoveAccentsAfterStemming = removeAccentsAfterStemming;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Process the document by the processing settings and indexate it
        /// </summary>
        /// <param name="document">The document</param>
        public void Index(string document)
        {
            // To lower
            if (ToLowerCase)
                document = document.ToLower();

            // Remove newlines from the document to prepare the doc for tokenization
            document = document.Replace(Environment.NewLine, " ");

            // Remove accents before stemming
            if (RemoveAccentsBeforeStemming)
                document = RemoveAccents(document);

            // Tokenize
            var tokens = _tokenizer.Tokenize(document);

            // Remove stopwords
            if (_stopWordRemover != null)
                tokens = _stopWordRemover.Process(tokens);

            // Go through all tokens
            for (int i = 0; i < tokens.Length; ++i)
            {
                // Stemming
                tokens[i] = _stemmer.Stem(tokens[i]);

                // Remove accents after stemming
                if (RemoveAccentsAfterStemming)
                    tokens[i] = RemoveAccents(tokens[i]);

                // Count frequency
                if (_wordFrequencies.ContainsKey(tokens[i]))
                    _wordFrequencies[tokens[i]] += 1;
                else
                    _wordFrequencies.Add(tokens[i], 1);
            }
        }

        /// <summary>
        /// Process the word by the processing settings
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
