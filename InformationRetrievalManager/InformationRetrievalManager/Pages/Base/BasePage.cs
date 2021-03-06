using Ixs.DNA;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The Base page for all pages to gain base functionality.
    /// </summary>
    public class BasePage : Page
    {
        #region Private Members

        /// <summary>
        /// The View Model associated with this page.
        /// </summary>
        private object _viewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// Distance of slide animation.
        /// </summary>
        public int SlideDistance { get; set; } = 25; //this.WindowHeight;

        /// <summary>
        /// The time any slide animation takes to complete.
        /// </summary>
        public float SlideSeconds { get; set; } = 0.2f;

        /// <summary>
        /// The animation to play when the page is first loaded.
        /// </summary>
        public PageAnimation PageLoadAnimation { get; set; } = PageAnimation.SlideAndFadeInFromBottom;

        /// <summary>
        /// The animation to play when the page is first unloaded.
        /// </summary>
        public PageAnimation PageUnloadAnimation { get; set; } = PageAnimation.SlideAndFadeOutToBottom;

        /// <summary>
        /// Indicates if lazy unload is requiested (= 0), otherwise lazy-unload is set (> 0)
        /// Unit: ms
        /// </summary>
        public float LazyUnload { get; set; } = 0;

        /// <summary>
        /// A flag to indicate if this page should animate out on load.
        /// Useful for when we are moving the page to another frame.
        /// </summary>
        public bool ShouldAnimateOut { get; set; }

        /// <summary>
        /// The View Model associated with this page.
        /// </summary>
        public object ViewModelObject
        {
            get
            {
                return _viewModel;
            }
            set
            {
                // If nothing has changed, return.
                if (_viewModel == value)
                    return;

                _viewModel = value;

                // Set the data context for this page.
                DataContext = _viewModel;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasePage()
        {
            // If we are animation in, hide to begin with.
            if (PageLoadAnimation != PageAnimation.None)
            {
                Visibility = Visibility.Collapsed;
            }

            // Listen out for the page loading.
            Loaded += BasePage_LoadedAsync;
        }

        #endregion

        #region Animation Load / Unload

        /// <summary>
        /// Once the page is loaded, perform any required animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BasePage_LoadedAsync(object sender, RoutedEventArgs e)
        {
            // If we are setup to animate out on load.
            if (ShouldAnimateOut)
            {
                // Animate out the page.
                await AnimateOutAsync();
            }
            // Otherwise...
            else
            {
                // animate the page in.
                await AnimateInAsync();
            }
        }

        /// <summary>
        /// Animates the page in.
        /// </summary>
        /// <returns></returns>
        public async Task AnimateInAsync()
        {
            // Make sure we have something to do.
            if (PageLoadAnimation == PageAnimation.None)
            {
                return;
            }

            switch (PageLoadAnimation)
            {
                case PageAnimation.SlideAndFadeInFromBottom:

                    // Start the animation.
                    await this.SlideAndFadeInAsync(AnimationSlideInDirection.Bottom, false, SlideSeconds, size: SlideDistance);

                    break;
            }
        }

        /// <summary>
        /// Animates the page out.
        /// </summary>
        /// <returns></returns>
        public async Task AnimateOutAsync()
        {
            // Make sure we have something to do.
            if (PageUnloadAnimation == PageAnimation.None)
            {
                return;
            }

            switch (PageUnloadAnimation)
            {
                case PageAnimation.SlideAndFadeOutToBottom:

                    if (LazyUnload > 0)
                        await UnloadProcessAsync();

                    // Start the animation.
                    await this.SlideAndFadeOutAsync(AnimationSlideInDirection.Bottom, SlideSeconds, size: SlideDistance);

                    break;
            }
        }

        /// <summary>
        /// Do the unload process of specific page
        /// </summary>
        public virtual async Task UnloadProcessAsync()
        {
            // Do the job in the specific pages
            await Task.CompletedTask;
        }

        #endregion
    }

    /// <summary>
    /// A base page for all pages to gain base functionality.
    /// </summary>
    public class BasePage<VM> : BasePage
        where VM : BaseViewModel, new()
    {
        #region Public Properties

        /// <summary>
        /// The View Model associated with this page.
        /// </summary>
        public VM ViewModel
        {
            get => (VM)ViewModelObject;
            set => ViewModelObject = value;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="specificViewModel">The specific view model to use, if any.</param>
        public BasePage() : base()
        {
            // If in design time mode...
            if (DesignerProperties.GetIsInDesignMode(this))
                // Just use a new instance of the VM
                ViewModel = new VM();
            else
                // Create a default view model
                ViewModel = Framework.Service<VM>() ?? new VM();
        }

        /// <summary>
        /// Constructor with specific view model.
        /// </summary>
        /// <param name="specificViewModel">The specific view model to use, if any.</param>
        public BasePage(VM specificViewModel = null) : base()
        {
            // Set specific view model
            if (specificViewModel != null)
                ViewModel = specificViewModel;
            else
            {
                // If in design time mode...
                if (DesignerProperties.GetIsInDesignMode(this))
                    // Just use a new instance of the VM
                    ViewModel = new VM();
                else
                {
                    // Create a default view model
                    ViewModel = Framework.Service<VM>() ?? new VM();
                }
            }
        }

        #endregion
    }
}
