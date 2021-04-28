namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for DataInstancePage.xaml
    /// </summary>
    public partial class DataInstancePage : BasePage<DataInstancePageViewModel>
    {
        public DataInstancePage()
        {
            InitializeComponent();
        }

        public DataInstancePage(DataInstancePageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}
