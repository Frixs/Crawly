using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for a radio button entry
    /// <summary>
    public class RadioEntryViewModel : BaseEntryViewModel<bool>
    {
        #region Protected Members

        /// <inheritdoc/>
        protected override Func<bool, bool> ValueCustomSetterProcess => null;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name this entry for simplier recognition on check event.
        /// </summary>
        public string RadioName { get; set; }

        /// <summary>
        /// The radio group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The action to run on check.
        /// </summary>
        public Func<Task> CheckAction { get; set; }

        #endregion

        #region Command Flags

        /// <summary>
        /// On Check Command flag
        /// </summary>
        public bool OnCheckCommandFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command fired on check
        /// </summary>
        public ICommand OnCheckCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RadioEntryViewModel() : base()
        {
            OnCheckCommand = new RelayCommand(async () => await OnCheckCommandRoutineAsync());
        }

        #endregion

        #region Command Methods

        private async Task OnCheckCommandRoutineAsync()
        {
            await RunCommandAsync(() => OnCheckCommandFlag, async () =>
            {
                if (OnCheckCommand != null)
                    await CheckAction();
            });
        }

        #endregion
    }
}
