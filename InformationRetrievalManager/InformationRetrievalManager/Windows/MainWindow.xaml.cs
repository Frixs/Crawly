using System.Windows;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// The exit call is moved into here until custom window style will be made.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DI.ViewModelApplication.Exit();
        }
    }
}
