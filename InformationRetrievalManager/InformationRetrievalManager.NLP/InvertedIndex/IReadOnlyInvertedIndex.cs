using System.Collections.Generic;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Extended interface for inverted index to be able to put the vocabulary outside of the instance with no possibility of modification.
    /// </summary>
    public interface IReadOnlyInvertedIndex
    {
        /// <summary>
        /// Name that identifies the inverted index instance
        /// </summary>
        /// <remarks>
        ///     Should not be <see cref="null"/>.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Tranforms the index vocabulary to publicly manipulatable object.
        /// </summary>
        /// <returns>Index vocabulary</returns>
        /// <remarks>
        ///     <see cref="InvertedIndex._vocabulary"/>
        /// </remarks>
        IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> GetReadOnlyVocabulary();
    }
}
