using System.ComponentModel;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Abstract page of the application.
    /// ---
    /// Range:    0 - 99 = Login is not required for this page
    ///        1000 +    = Cannot be loaded from reopening the application (pages with view model - like forms etc)
    /// Range specification in <see cref="ApplicationViewModel.GoToPage(ApplicationPage, BaseViewModel)"/>
    /// </summary>
    public enum ApplicationPage
    {
        /// <summary>
        /// The home page
        /// </summary>
        [Description("Home")]
        Home = 0,

        /// <summary>
        /// The how-to page
        /// </summary>
        [Description("How To")]
        HowTo = 1,

        // ----------

    }
}
