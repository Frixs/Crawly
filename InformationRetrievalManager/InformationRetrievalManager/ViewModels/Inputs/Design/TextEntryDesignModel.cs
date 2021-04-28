namespace InformationRetrievalManager
{
    public class TextEntryDesignModel : TextEntryViewModel
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static TextEntryDesignModel Instance => new TextEntryDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TextEntryDesignModel()
        {
            Label = "This is my input";
            Description = "It makes it very difficult to debug unless design models exist and let this description be shown.";
            Value = "random value";
        }

        #endregion
    }
}
