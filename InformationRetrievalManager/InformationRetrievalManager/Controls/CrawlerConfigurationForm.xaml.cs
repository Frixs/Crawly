using System.ComponentModel;
using System.Windows.Controls;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for CrawlerConfigurationForm.xaml
    /// </summary>
    public partial class CrawlerConfigurationForm : UserControl
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static CrawlerConfigurationForm DesignInstance => new CrawlerConfigurationForm();

        #endregion

        public CrawlerConfigurationForm()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-mode specific functionality
            }
        }
    }
}
