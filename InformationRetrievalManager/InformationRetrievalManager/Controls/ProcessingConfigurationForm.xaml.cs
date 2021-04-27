using System.ComponentModel;
using System.Windows;
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

                FormContext = new ProcessingConfigurationFormContext();
            }
        }

        public ProcessingConfigurationFormContext FormContext
        {
            get { return (ProcessingConfigurationFormContext)GetValue(FormContextProperty); }
            set { SetValue(FormContextProperty, value); }
        }
        public static readonly DependencyProperty FormContextProperty =
            DependencyProperty.Register(nameof(FormContext), typeof(ProcessingConfigurationFormContext), typeof(CrawlerConfigurationForm));
    }
}
