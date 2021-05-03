using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for DataInstanceConfigurationView.xaml
    /// </summary>
    public partial class DataInstanceConfigurationView : UserControl
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static DataInstanceConfigurationView DesignInstance => new DataInstanceConfigurationView();

        #endregion

        public DataInstanceConfigurationView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-mode specific functionality
            }
        }

        public DataInstanceConfigurationViewContext ConfigurationContext
        {
            get { return (DataInstanceConfigurationViewContext)GetValue(ConfigurationContextProperty); }
            set { SetValue(ConfigurationContextProperty, value); }
        }
        public static readonly DependencyProperty ConfigurationContextProperty =
            DependencyProperty.Register(nameof(ConfigurationContext), typeof(DataInstanceConfigurationViewContext), typeof(DataInstanceConfigurationView));
    }
}
