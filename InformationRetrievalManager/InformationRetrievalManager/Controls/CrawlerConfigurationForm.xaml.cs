using InformationRetrievalManager.Core;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public CrawlerConfigurationFormContext FormContext
        {
            get { return (CrawlerConfigurationFormContext)GetValue(FormContextProperty); }
            set { SetValue(FormContextProperty, value); }
        }
        public static readonly DependencyProperty FormContextProperty =
            DependencyProperty.Register(nameof(FormContext), typeof(CrawlerConfigurationFormContext), typeof(CrawlerConfigurationForm));
    }
}
