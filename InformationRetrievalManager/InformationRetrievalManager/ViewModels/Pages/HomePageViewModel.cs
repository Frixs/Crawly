using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="HomePage"/>
    /// </summary>
    public class HomePageViewModel : BaseViewModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly ITaskManager _taskManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the data are already loaded into the VM (once the values changes to <see langword="true"/>).
        /// </summary>
        public bool DataLoaded { get; set; }

        /// <summary>
        /// Collection of data instances created in this app by the user
        /// </summary>
        public ObservableCollection<DataInstanceDataModel> DataInstances { get; set; } = new ObservableCollection<DataInstanceDataModel>();

        #endregion

        #region Command Flags

        /// <summary>
        /// Command flag for opening process (e.g. files or for opening web pages)
        /// </summary>
        public bool ProcessFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHowToPageCommand { get; set; }

        /// <summary>
        /// Command to open page with the specific data instance
        /// </summary>
        public ICommand GoToDataInstanceCommand { get; set; }

        /// <summary>
        /// The command to go to donate website.
        /// </summary>
        public ICommand DonateCommand { get; set; }

        /// <summary>
        /// Command to create a new data instance
        /// </summary>
        public ICommand CreateNewDataInstanceCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomePageViewModel()
        {
            // Create commands.
            GoToHowToPageCommand = new RelayCommand(GoToHowToPageCommandRoutine);
            GoToDataInstanceCommand = new RelayParameterizedCommand((parameter) => GoToDataInstancePageCommandRoutine(parameter));
            DonateCommand = new RelayCommand(async () => await DonateCommandRoutineAsync());
            CreateNewDataInstanceCommand = new RelayCommand(GoToCreateDataInstancePageCommandRoutine);
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public HomePageViewModel(ILogger logger, IUnitOfWork uow, ITaskManager taskManager)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));

            // Load data into the VM
            _taskManager.RunAndForget(LoadAsync);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Command Routine : Go To Page
        /// </summary>
        private void GoToHowToPageCommandRoutine()
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.HowTo);
        }

        /// <summary>
        /// Command Routine : Go To Page
        /// </summary>
        private void GoToDataInstancePageCommandRoutine(object parameter)
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.DataInstance,
                Framework.Service<DataInstancePageViewModel>().Init(long.Parse(parameter.ToString()))
                );
        }

        /// <summary>
        /// Command Routine : Go To Donate website
        /// </summary>
        /// <returns></returns>
        private async Task DonateCommandRoutineAsync()
        {
            await RunCommandAsync(() => ProcessFlag, async () =>
            {
                string url = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=QE2V3BNQJVG5W&source=url";

                if (!string.IsNullOrEmpty(url) && url.IsURL())
                    System.Diagnostics.Process.Start(url);

                await Task.Delay(1);
            });
        }

        /// <summary>
        /// Command Routine : Go To Page
        /// </summary>
        private void GoToCreateDataInstancePageCommandRoutine()
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.CreateDataInstance);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load initial configuration/data
        /// </summary>
        private async Task LoadAsync()
        {
            // Load data instances
            DataInstances = new ObservableCollection<DataInstanceDataModel>(
                _uow.DataInstances.Get(null, q => q.OrderBy(o => o.Name))
                );

            DataLoaded = true;
            await Task.Delay(1);
        }

        #endregion
    }
}
