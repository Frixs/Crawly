using Ixs.DNA;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.NLP;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        /// <summary>
        /// Injects the view models needed for our application
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction AddTheViewModels(this FrameworkConstruction construction)
        {
            // Bind to a single instance of Application view model
            construction.Services.AddSingleton<ApplicationViewModel>(provider =>
            {
                return new ApplicationViewModel(true);
            });

            // Bind page view models
            construction.Services.AddTransient<HomePageViewModel>();
            construction.Services.AddTransient<HowToPageViewModel>();

            //// Bind settings view model
            //construction.Services.AddSingleton<SettingsViewModel>();

            // Return the construction for chaining
            return construction;
        }

        /// <summary>
        /// Injects the client application services needed for the application
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction AddTheServices(this FrameworkConstruction construction)
        {
            // Rewrite the default logger from DNA Framework to our own
            construction.Services.AddTransient(provider => provider.GetService<ILoggerFactory>().CreateLogger(typeof(App).Namespace));

            // Bind a UI manager
            construction.Services.AddSingleton<IUIManager, UIManager>();

            // Bind a task manager
            construction.Services.AddTransient<ITaskManager, BaseTaskManager>();

            // Bind a file manager
            construction.Services.AddTransient<IFileManager, BaseFileManager>();

            // Bind a crawler manager
            construction.Services.AddSingleton<ICrawlerManager, CrawlerManager>();

            // Bind a crawler storage
            construction.Services.AddSingleton<ICrawlerStorage, CrawlerStorage>();

            // Bind a query index manager
            construction.Services.AddSingleton<IQueryIndexManager, QueryIndexManager>();

            // Return the construction for chaining
            return construction;
        }
    }
}
