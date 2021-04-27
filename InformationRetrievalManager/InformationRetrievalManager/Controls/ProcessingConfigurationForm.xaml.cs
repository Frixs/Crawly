using System.ComponentModel;
using System.Windows.Controls;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for ProcessingConfigurationForm.xaml
    /// </summary>
    public partial class ProcessingConfigurationForm : UserControl
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static ProcessingConfigurationForm DesignInstance => new ProcessingConfigurationForm();

        #endregion

        public ProcessingConfigurationForm()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-mode specific functionality
            }
        }
    }
}
