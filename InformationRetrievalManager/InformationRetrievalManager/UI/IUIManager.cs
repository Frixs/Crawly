using System.Windows;

namespace InformationRetrievalManager
{
    /// <summary>
    /// UI interactions in the application.
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Get Application's main window size.
        /// </summary>
        Vector GetMainWindowSize();

        /// <summary>
        /// Set Application's main window size.
        /// </summary>
        /// <param name="size">X=Width, Y=Height</param>
        void SetMainWindowSize(Vector size);

        /// <summary>
        /// Show main window.
        /// </summary>
        void ShowMainWindow();
    }
}
