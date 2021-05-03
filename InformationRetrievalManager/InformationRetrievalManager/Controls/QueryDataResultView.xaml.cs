using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for QueryDataResultView.xaml
    /// </summary>
    public partial class QueryDataResultView : UserControl
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static QueryDataResultView DesignInstance => new QueryDataResultView();

        #endregion

        public QueryDataResultView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-mode specific functionality

                var ctx = new QueryDataResultViewContext();
                ctx.Data.Add(new QueryDataResultViewContext.Result
                {
                    Title = "Nam ipsum diam, blandit vitae sapien et, interdum luctus sapien",
                    Category = "My Category",
                    Timestamp = "3.5. 2021",
                    SourceUrl = "https://www.lipsum.com/feed/html",
                    Content = " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent vitae enim turpis. Sed fringilla malesuada quam, " +
                    "eget euismod enim ullamcorper et. In sodales, ipsum eu ornare eleifend, leo velit consectetur lectus, sed facilisis augue " +
                    "magna vel justo. Nam quis eros quis sem semper porttitor eu nec ex. Sed iaculis a erat et sodales..."
                });
                ctx.Data.Add(new QueryDataResultViewContext.Result
                {
                    Title = "Phasellus cursus eu velit sed euismod",
                    Category = null,
                    Timestamp = "3.5. 2021",
                    SourceUrl = "https://www.lipsum.com/feed/html",
                    Content = " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc efficitur sollicitudin magna vel tempus. Aenean ac " +
                    "placerat tellus, ut interdum turpis. Nullam vitae diam lectus. Vivamus sit amet bibendum tellus. Maecenas finibus ex sed" +
                    " dolor consectetur pretium. Nunc sit amet lobortis tortor..."
                });
                ctx.Data.Add(new QueryDataResultViewContext.Result
                {
                    Title = "Mauris quis mi augue. Sed vel rhoncus nunc. Fusce tristique eros ac nisi porttitor scelerisque",
                    Category = "EVENT",
                    Timestamp = null,
                    SourceUrl = "https://www.lipsum.com/feed/html",
                    Content = " Curabitur faucibus tortor mauris, sit amet luctus tellus egestas non. Curabitur cursus dolor neque, " +
                    "quis consectetur purus congue eu. Suspendisse purus metus, aliquam condimentum dui eu, blandit gravida orci. Vivamus " +
                    "nunc metus, sodales et sodales ut, lacinia sed odio. Proin cursus at neque in hendrerit. Proin in sem id ipsum..."
                });
                ResultContext = ctx;
            }
        }

        public QueryDataResultViewContext ResultContext
        {
            get { return (QueryDataResultViewContext)GetValue(ResultContextProperty); }
            set { SetValue(ResultContextProperty, value); }
        }
        public static readonly DependencyProperty ResultContextProperty =
            DependencyProperty.Register(nameof(ResultContext), typeof(QueryDataResultViewContext), typeof(QueryDataResultView));

        public ICommand OpenSourceUrlCommand
        {
            get { return (ICommand)GetValue(OpenSourceUrlCommandProperty); }
            set { SetValue(OpenSourceUrlCommandProperty, value); }
        }
        public static readonly DependencyProperty OpenSourceUrlCommandProperty =
            DependencyProperty.Register(nameof(OpenSourceUrlCommand), typeof(ICommand), typeof(QueryDataResultView));
    }
}
