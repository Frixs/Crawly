using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

            // Log it.
            FrameworkDI.Logger.LogDebugSource("Application starting up" + /*(BaseDI.ViewModelApplication.AppAssembly.IsRunningAsAdministrator() ? " (As Administrator)" : "") +*/ "...");

            // Open the MainWindow.
            // TODO: remove startup url from app.xaml and make this work
            //BaseDI.UI.ShowMainWindow();
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
                .AddTheViewModels()
                .AddTheServices()
                .Build();

            // Add crawlers
            var crawler = new CrawlerEngine("bdo-sea");
            crawler.SetControls(
                "https://www.sea.playblackdesert.com",
                "/News/Notice?boardType=2&Page={0}",
                1, 12, 1,
                1000,
                "//article[@class='content']//ul[@class='thumb_nail_list']//a",
                "//div[@class='view_detail_area']",
                "//div[@class='view_detail_area']//strong[@class='title']",
                "//div[@class='view_detail_area']//span[@class='date']",
                new DatetimeParseData("yyyy-MM-dd HH:mm")
                );
            await Framework.Service<ICrawlerManager>().AddCrawlerAsync(crawler);

            // TODO: app setup
            // Ensure the client data store
            //await CoreDI.ClientDataStore.EnsureDataStoreAsync();

            // Init locally saved data structures after load of all modules.
            //CoreDI.LocalSettingsStorage.InitData();

            // App assembly.
            //BaseDI.ViewModelApplication.AppAssembly = new AppAssembly(
            //    Assembly.GetExecutingAssembly(),
            //    //ApplicationDeployment.IsNetworkDeployed
            //    //    ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
            //    //    : (args.ContainsKey(ApplicationArgument.Version.ToString())
            //    //        ? args[ApplicationArgument.Version.ToString()]
            //    //        : FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion),
            //    FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion,
            //    FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).LegalCopyright
            //    );
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
