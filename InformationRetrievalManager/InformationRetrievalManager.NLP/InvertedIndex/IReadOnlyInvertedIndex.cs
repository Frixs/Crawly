using System.Collections.Generic;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Extended interface for inverted index to be able to put the vocabulary outside of the instance with no possibility of modification.
    /// </summary>
    public interface IReadOnlyInvertedIndex
    {
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
