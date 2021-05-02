using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for QueryDataResult.xaml
    /// </summary>
    public partial class QueryDataResult : UserControl
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static QueryDataResult DesignInstance => new QueryDataResult();

        #endregion


        public QueryDataResult()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-mode specific functionality

                ResultContext = new QueryDataResultContext();
            }
        }

        public QueryDataResultContext ResultContext
        {
            get { return (QueryDataResultContext)GetValue(ResultContextProperty); }
            set { SetValue(ResultContextProperty, value); }
        }
        public static readonly DependencyProperty ResultContextProperty =
            DependencyProperty.Register(nameof(ResultContext), typeof(QueryDataResultContext), typeof(QueryDataResult));
    }
}
