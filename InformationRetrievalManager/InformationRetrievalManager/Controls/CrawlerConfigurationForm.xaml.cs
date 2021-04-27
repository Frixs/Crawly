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

        public string SiteAddress
        {
            get { return (string)GetValue(SiteAddressProperty); }
            set { SetValue(SiteAddressProperty, value); }
        }
        public static readonly DependencyProperty SiteAddressProperty =
            DependencyProperty.Register(nameof(SiteAddress), typeof(string), typeof(CrawlerConfigurationForm));

        public string SiteSuffix
        {
            get { return (string)GetValue(SiteSuffixProperty); }
            set { SetValue(SiteSuffixProperty, value); }
        }
        public static readonly DependencyProperty SiteSuffixProperty =
            DependencyProperty.Register(nameof(SiteSuffix), typeof(string), typeof(CrawlerConfigurationForm));

        public int StartPageNo
        {
            get { return (int)GetValue(StartPageNoProperty); }
            set { SetValue(StartPageNoProperty, value); }
        }
        public static readonly DependencyProperty StartPageNoProperty =
            DependencyProperty.Register(nameof(StartPageNo), typeof(int), typeof(CrawlerConfigurationForm));

        public int MaxPageNo
        {
            get { return (int)GetValue(MaxPageNoProperty); }
            set { SetValue(MaxPageNoProperty, value); }
        }
        public static readonly DependencyProperty MaxPageNoProperty =
            DependencyProperty.Register(nameof(MaxPageNo), typeof(int), typeof(CrawlerConfigurationForm));

        public int PageNoModifier
        {
            get { return (int)GetValue(PageNoModifierProperty); }
            set { SetValue(PageNoModifierProperty, value); }
        }
        public static readonly DependencyProperty PageNoModifierProperty =
            DependencyProperty.Register(nameof(PageNoModifier), typeof(int), typeof(CrawlerConfigurationForm));

        public int SearchInterval
        {
            get { return (int)GetValue(SearchIntervalProperty); }
            set { SetValue(SearchIntervalProperty, value); }
        }
        public static readonly DependencyProperty SearchIntervalProperty =
            DependencyProperty.Register(nameof(SearchInterval), typeof(int), typeof(CrawlerConfigurationForm));

        public string SiteUrlArticlesXPath
        {
            get { return (string)GetValue(SiteUrlArticlesXPathProperty); }
            set { SetValue(SiteUrlArticlesXPathProperty, value); }
        }
        public static readonly DependencyProperty SiteUrlArticlesXPathProperty =
            DependencyProperty.Register(nameof(SiteUrlArticlesXPath), typeof(string), typeof(CrawlerConfigurationForm));

        public string SiteArticleContentAreaXPath
        {
            get { return (string)GetValue(SiteArticleContentAreaXPathProperty); }
            set { SetValue(SiteArticleContentAreaXPathProperty, value); }
        }
        public static readonly DependencyProperty SiteArticleContentAreaXPathProperty =
            DependencyProperty.Register(nameof(SiteArticleContentAreaXPath), typeof(string), typeof(CrawlerConfigurationForm));

        public string SiteArticleTitleAreaXPath
        {
            get { return (string)GetValue(SiteArticleTitleAreaXPathProperty); }
            set { SetValue(SiteArticleTitleAreaXPathProperty, value); }
        }
        public static readonly DependencyProperty SiteArticleTitleAreaXPathProperty =
            DependencyProperty.Register(nameof(SiteArticleTitleAreaXPath), typeof(string), typeof(CrawlerConfigurationForm));

        public string SiteArticleCategoryAreaXPath
        {
            get { return (string)GetValue(SiteArticleCategoryAreaXPathProperty); }
            set { SetValue(SiteArticleCategoryAreaXPathProperty, value); }
        }
        public static readonly DependencyProperty SiteArticleCategoryAreaXPathProperty =
            DependencyProperty.Register(nameof(SiteArticleCategoryAreaXPath), typeof(string), typeof(CrawlerConfigurationForm));

        public string SiteArticleDateTimeAreaXPath
        {
            get { return (string)GetValue(SiteArticleDateTimeAreaXPathProperty); }
            set { SetValue(SiteArticleDateTimeAreaXPathProperty, value); }
        }
        public static readonly DependencyProperty SiteArticleDateTimeAreaXPathProperty =
            DependencyProperty.Register(nameof(SiteArticleDateTimeAreaXPath), typeof(string), typeof(CrawlerConfigurationForm));

        public DatetimeParseData SiteArticleDateTimeParseData
        {
            get { return (DatetimeParseData)GetValue(SiteArticleDateTimeParseDataProperty); }
            set { SetValue(SiteArticleDateTimeParseDataProperty, value); }
        }
        public static readonly DependencyProperty SiteArticleDateTimeParseDataProperty =
            DependencyProperty.Register(nameof(SiteArticleDateTimeParseData), typeof(DatetimeParseData), typeof(CrawlerConfigurationForm));

        public ICommand CreateCommand
        {
            get { return (ICommand)GetValue(CreateCommandProperty); }
            set { SetValue(CreateCommandProperty, value); }
        }
        public static readonly DependencyProperty CreateCommandProperty =
            DependencyProperty.Register(nameof(CreateCommand), typeof(ICommand), typeof(CrawlerConfigurationForm));
    }
}
