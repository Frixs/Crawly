using System.Windows;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The applications implementation of the <see cref="IUIManager"/>.
    /// </summary>
    public class UIManager : IUIManager
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UIManager()
        {
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public Vector GetMainWindowSize()
        {
            return new Vector(Application.Current.MainWindow.Width, Application.Current.MainWindow.Height);
        }

        /// <inheritdoc/>
        public void SetMainWindowSize(Vector size)
        {
            Application.Current.MainWindow.Width = size.X;
            Application.Current.MainWindow.Height = size.Y;
        }

        /// <inheritdoc/>
        public void ShowMainWindow()
        {
            // Activate window if it is visible in background.
            if (Application.Current.MainWindow.IsVisible)
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            // Open window if it is closed in tray.
            else
            {
                Application.Current.MainWindow.Show();
            }

            // Activate window.
            Application.Current.MainWindow.Activate();
            // Try to bring focus to the window.
            Application.Current.MainWindow.Focus();
        }

        #endregion
    }
}
