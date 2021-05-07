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
        /// Tranforms the index to publicly manipulatable object.
        /// </summary>
        /// <returns>Index data</returns>
        /// <remarks>
        ///     the method transforms <see cref="InvertedIndex._data"/> into read only version (the method is NOT O(1) time complexity).
        /// </remarks>
        InvertedIndex.ReadOnlyData GetReadOnlyData();
    }
}
