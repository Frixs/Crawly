using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.Relational;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Dispatcher Unhandled Exception

        /// <summary>
        /// Unhandled exception event handler.
        /// </summary>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log error.
            if (FrameworkDI.Logger != null)
                FrameworkDI.Logger.LogCriticalSource($"An unhandled exception occurred: {e.Exception.Message}", exception: e.Exception);
            MessageBox.Show($"An unhandled exception just occurred: {e.Exception.Message}.{Environment.NewLine}Please, contact the developers to fix the issue.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Warning);

            e.Handled = true;
        }

        #endregion

        #region Startup / Exit

        /// <summary>
        /// Custom startup so we load our IoC immediately before anything else.
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Let the base application do what it needs.
            base.OnStartup(e);

            // Compile app's arguments.
            var argsCompiled = CompileArguments(e.Args);
            // Set up our application.
            await ApplicationSetupAsync(argsCompiled);

            // Set application's main window.
            Current.MainWindow = new MainWindow();

            // Load state data
            LoadStateData();

            // Log it
            FrameworkDI.Logger.LogDebugSource("Application starting up" + /*(BaseDI.ViewModelApplication.AppAssembly.IsRunningAsAdministrator() ? " (As Administrator)" : "") +*/ "...");

            // Open the MainWindow
            Framework.Service<IUIManager>().ShowMainWindow();
        }

        /// <summary>
        /// Perform tasks at application exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }

        #endregion

        #region Application Setup

        /// <summary>
        /// Set up our application
        /// </summary>
        private async Task ApplicationSetupAsync(Dictionary<string, string> args)
        {
            // Setup the DNA Framework
            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger(
                    logPath: Framework.Construction.Environment.IsDevelopment ? "logs/debug.log" : "logs/InformationRetrievalManager.log",
                    logLevel: (LogLevel)Enum.Parse(typeof(LogLevel), Framework.Construction.Configuration.GetSection("Logging:LogLevel:Default")?.Value ?? LogLevel.Information.ToString(), true),
                    trimSize: 50000000) // 50MB limit
                .AddDataStore()
                .AddTheViewModels()
                .AddTheServices()
                .Build();

            // Ensure the database is set up
            await Framework.Service<IUnitOfWork>().EnsureDatabaseCreatedAsync();

            // HECK: crawler creation
            // Add crawlers
            var crawler = new CrawlerEngine("bdo-naeu");
            crawler.SetControls(
                siteAddress: "https://www.naeu.playblackdesert.com",
                siteSuffix: "/en-US/News/Notice?boardType=0&Page={0}",
                startPageNo: 1, maxPageNo: 8, pageNoModifier: 1,
                searchInterval: 1000,
                siteUrlArticlesXPath: "//article[@class='content']//ul[@class='thumb_nail_list']//a",
                siteArticleContentAreaXPath: "//div[@class='view_detail_area']//div[contains(@class, 'contents_area')]",
                siteArticleTitleXPath: "//div[@class='view_detail_area']//div[@class='title_area']//strong[@class='title']",
                siteArticleCategoryXPath: "//div[@class='view_detail_area']//div[@class='title_area']//span[contains(@class, 'tag_label')]",
                siteArticleDateTimeXPath: "//div[@class='view_detail_area']//div[@class='title_area']//span[@class='date']",
                siteArticleDateTimeParseData: new DatetimeParseData("MMM d, yyyy, HH:mm (UTC)", CultureInfo.CreateSpecificCulture("en-US"))
                );
            await Framework.Service<ICrawlerManager>().AddCrawlerAsync(crawler);
        }

        /// <summary>
        /// Load the state data
        /// </summary>
        private void LoadStateData()
        {
            // Get state data
            var data = Framework.Service<IUnitOfWork>().ApplicationState.Get();

            // Set main window size
            Framework.Service<IUIManager>().SetMainWindowSize(new Vector(data.MainWindowSizeX, data.MainWindowSizeY));
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Compile arguments into Dictionary.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Dictionary<string, string> CompileArguments(string[] args)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            if (args.Length == 0)
                return d;

            string[] sParts;
            foreach (string s in args)
            {
                sParts = s.Split('=');

                // We want only named arguments.
                if (sParts.Length <= 1)
                    continue;

                d.Add(
                    sParts[0].Trim(),
                    sParts[1].Trim('"')
                    );
            }

            return d;
        }

        #endregion
    }
}
