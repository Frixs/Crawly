using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public ICommand CrawlerConfigurationUpdateCommand
        {
            get { return (ICommand)GetValue(CrawlerConfigurationUpdateCommandProperty); }
            set { SetValue(CrawlerConfigurationUpdateCommandProperty, value); }
        }
        public static readonly DependencyProperty CrawlerConfigurationUpdateCommandProperty =
            DependencyProperty.Register(nameof(CrawlerConfigurationUpdateCommand), typeof(ICommand), typeof(DataInstanceConfigurationView));

        public ICommand ProcessingConfigurationUpdateCommand
        {
            get { return (ICommand)GetValue(ProcessingConfigurationUpdateCommandProperty); }
            set { SetValue(ProcessingConfigurationUpdateCommandProperty, value); }
        }
        public static readonly DependencyProperty ProcessingConfigurationUpdateCommandProperty =
            DependencyProperty.Register(nameof(ProcessingConfigurationUpdateCommand), typeof(ICommand), typeof(DataInstanceConfigurationView));

        public ICommand DataInstanceNameUpdateCommand
        {
            get { return (ICommand)GetValue(DataInstanceNameUpdateCommandProperty); }
            set { SetValue(DataInstanceNameUpdateCommandProperty, value); }
        }
        public static readonly DependencyProperty DataInstanceNameUpdateCommandProperty =
            DependencyProperty.Register(nameof(DataInstanceNameUpdateCommand), typeof(ICommand), typeof(DataInstanceConfigurationView));
    }
}
