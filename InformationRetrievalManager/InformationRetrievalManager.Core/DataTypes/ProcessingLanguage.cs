
using System.ComponentModel;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Supported languages for NLP processing
    /// </summary>
    public enum ProcessingLanguage
    {
        [Description("English")]
        EN = 0,

        [Description("Czech")]
        CZ = 1
    }
}
