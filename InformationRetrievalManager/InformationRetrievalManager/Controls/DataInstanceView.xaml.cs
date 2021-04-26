using InformationRetrievalManager.Relational;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for DataInstanceView.xaml
    /// </summary>
    public partial class DataInstanceView : UserControl
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static DataInstanceView DesignInstance => new DataInstanceView();

        #endregion

        public DataInstanceView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-mode specific functionality

                Items = HomePageDesignModel.Instance.DataInstances;
            }
        }

        public ObservableCollection<DataInstanceDataModel> Items
        {
            get { return (ObservableCollection<DataInstanceDataModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Text1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<DataInstanceDataModel>), typeof(DataInstanceView));
    }
}
