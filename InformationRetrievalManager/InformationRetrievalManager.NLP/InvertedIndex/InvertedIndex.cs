using InformationRetrievalManager.Core;
using Ixs.DNA;
using MessagePack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Functionality to manage data as a inverted index.
    /// </summary>
    public sealed class InvertedIndex : IInvertedIndex
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        #endregion

        #region Private Members

        /// <summary>
        /// Index data structure
        /// </summary>
        private readonly Data _data = new Data();

        /// <summary>
        /// Timestamp as an additional identifier for indexation.
        /// </summary>
        private readonly DateTime _timestamp; //; ctor

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public InvertedIndex(string name, DateTime timestamp, IFileManager fileManager = null, ILogger logger = null)
        {
            Name = name;
            _timestamp = timestamp;

            _fileManager = fileManager;
            _logger = logger;
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public ReadOnlyData GetReadOnlyData()
        {
            return new ReadOnlyData(_data.Vocabulary, _data.Documents);
        }

        /// <inheritdoc/>
        public void Put(string term, IndexDocument document)
        {
            if (string.IsNullOrEmpty(term) || document == null)
                throw new ArgumentNullException("Invalid parameters for indexing!");
            if (document.Id < 0)
                throw new ArgumentException("Document ID is invalid.");

            // Check if the term already exists...
            if (_data.Vocabulary.ContainsKey(term))
            {
                // If the document is already registered for the term...
                if (_data.Vocabulary[term].ContainsKey(document.Id))
                {
                    _data.Vocabulary[term][document.Id].Frequency++;
                }
                // Otherwise, create a new record for the document...
                else
                {
                    _data.Vocabulary[term].Add(document.Id, new TermInfo(frequency: 1));
                    if (!_data.Documents.ContainsKey(document.Id)) // Add only if the document does not exist in the list yet
                        _data.Documents.Add(document.Id, new DocumentInfo(document.Timestamp));
                }
            }
            // Otherwise, create a new record...
            else
            {
                _data.Vocabulary.Add(term, new Dictionary<long, TermInfo>()
                {
                    { document.Id, new TermInfo(frequency: 1) }
                });
                if (!_data.Documents.ContainsKey(document.Id)) // Add only if the document does not exist in the list yet
                    _data.Documents.Add(document.Id, new DocumentInfo(document.Timestamp));
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     <see cref="_fileManager"/> must exists, otherwise <see langword="true"/> on return.
        /// </remarks>
        public bool Load()
        {
            if (_fileManager == null)
                return true;

            bool ok = false;

            // Deserialize
            var result = _fileManager.DeserializeObjectFromBinFileAsync<Data>($"{Constants.IndexDataStorageDir}/{Name}/{MakeFilename()}").Result;
            short status = result.ResultStatus;
            object obj = result.ResultData;

            // Check deserialization result
            if (status == 2)
                _logger?.LogWarningSource($"Failed to load index '{Name}'. Corrupted data.");
            else if (status != 0)
                _logger?.LogWarningSource($"Failed to load index '{Name}'. The index does not exist.");

            if (status == 0 && obj != null)
            {
                // Try to load index data
                var data = obj as Data;
                if (data != null)
                {
                    ok = true;

                    // Clear the previous data first
                    _data.Clear();
                    // Load new data
                    _data.Load(data.Vocabulary, data.Documents);
                }
                else
                    _logger?.LogWarningSource($"Failed to load index '{Name}'. Inconsistent data.");
            }
            else
                _logger?.LogWarningSource($"Failed to load index '{Name}'. Data does not exist.");

            return ok;
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     <see cref="_fileManager"/> must exists, otherwise <see langword="true"/> on return.
        /// </remarks>
        public bool Save(Action<string> setProgressMessage = null)
        {
            if (_fileManager == null)
                return true;

            bool ok = false;

            // Serialize
            short status = _fileManager.SerializeObjectToBinFileAsync(_data, $"{Constants.IndexDataStorageDir}/{Name}/{MakeFilename()}", setProgressMessage).Result;

            // Check serialization result
            if (status == 0)
                ok = true;
            else if (status == 2)
                _logger?.LogWarningSource($"Failed to save index '{Name}'. Invalid data.");
            else
                _logger?.LogWarningSource($"Failed to save index '{Name}'. The index failed to write data in a file.");

            // Clear vocabulary from memory if it is saved successfully
            if (ok)
                _data.Clear();

            return ok;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates filename specific for this index instance.
        /// </summary>
        /// <returns>The filename</returns>
        private string MakeFilename()
        {
            return $"{Name}_{_timestamp.Year}_{_timestamp.Month}_{_timestamp.Day}_{_timestamp.Hour}_{_timestamp.Minute}_{_timestamp.Second}.idx";
        }

        #endregion

        #region Data Class

        /// <summary>
        /// Index data structure definition
        /// </summary>
        [MessagePackObject]
        public sealed class Data
        {
            #region Public Properties

            /// <summary>
            /// Vocabulary of the inverted index data.
            /// </summary>
            /// <remarks>
            ///     SortedDictionary { Key=term, Value=posting list }, 
            ///     (posting list) Dictionary { Key=DocumentId, Value=TermInfo obj }
            /// </remarks>
            [Key(nameof(Data) + nameof(Vocabulary))]
            public SortedDictionary<string, Dictionary<long, TermInfo>> Vocabulary { get; set; } = new SortedDictionary<string, Dictionary<long, TermInfo>>();

            /// <summary>
            /// Documents of the inverted index.
            /// </summary>
            /// <remarks>
            ///     Dictionary { Key=DocumentId, Value=DocumentInfo obj }
            /// </remarks>
            [Key(nameof(Data) + nameof(Documents))]
            public Dictionary<long, DocumentInfo> Documents { get; set; } = new Dictionary<long, DocumentInfo>();

            #endregion

            #region Public Methods

            /// <summary>
            /// Load data into the data structure.
            /// </summary>
            public void Load(SortedDictionary<string, Dictionary<long, TermInfo>> vocabulary, Dictionary<long, DocumentInfo> documents)
            {
                Vocabulary = vocabulary;
                Documents = documents;
            }

            /// <summary>
            /// Clear data the data.
            /// </summary>
            public void Clear()
            {
                Vocabulary.Clear();
                Documents.Clear();
            }

            #endregion
        }

        /// <summary>
        /// Read-only wrapper for <see cref="Data"/>.
        /// </summary>
        public sealed class ReadOnlyData
        {
            #region Public Properties

            public IReadOnlyDictionary<string, IReadOnlyDictionary<long, IReadOnlyTermInfo>> Vocabulary { get; }
            public IReadOnlyDictionary<long, IReadOnlyDocumentInfo> Documents { get; } = null;

            #endregion

            #region Constructor

            /// <summary>
            /// Default constructor (internal)
            /// </summary>
            internal ReadOnlyData(SortedDictionary<string, Dictionary<long, TermInfo>> vocabulary, Dictionary<long, DocumentInfo> documents)
            {
                Vocabulary = vocabulary.ToDictionary(o => o.Key, o => (IReadOnlyDictionary<long, IReadOnlyTermInfo>)o.Value.ToDictionary(x => x.Key, x => (IReadOnlyTermInfo)x.Value));
                Documents = documents.ToDictionary(o => o.Key, o => (IReadOnlyDocumentInfo)o.Value);
            }

            /// <summary>
            /// Constructor (internal) with setting the vocabulary only.
            /// </summary>
            internal ReadOnlyData(SortedDictionary<string, Dictionary<long, TermInfo>> vocabulary, IReadOnlyDictionary<long, IReadOnlyDocumentInfo> documents)
            {
                Vocabulary = vocabulary.ToDictionary(o => o.Key, o => (IReadOnlyDictionary<long, IReadOnlyTermInfo>)o.Value.ToDictionary(x => x.Key, x => (IReadOnlyTermInfo)x.Value));
                Documents = documents;
            }

            #endregion
        }

        #endregion
    }
}
