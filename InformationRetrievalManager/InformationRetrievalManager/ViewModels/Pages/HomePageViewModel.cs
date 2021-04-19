﻿using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.NLP;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for Home page
    /// </summary>
    public class HomePageViewModel : BaseViewModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly ICrawlerManager _crawlerManager;
        private readonly ITaskManager _taskManager;
        private readonly IFileManager _fileManager;
        private readonly ICrawlerStorage _crawlerStorage;

        #endregion

        #region Private Members

        public ICrawlerEngine _crawler;

        #endregion

        #region Public Properties

        public bool IsCurrentlyCrawlingFlag { get; set; }

        public string CrawlerProcessProgress { get; set; }

        public string DataProcessingStatus { get; set; }

        public string Query { get; set; }

        public string QueryStatus { get; set; }

        #endregion

        #region Command Flags

        public bool ProcessingCommandFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHowToPageCommand { get; set; }

        public ICommand StartCrawlerCommand { get; set; }

        public ICommand CancelCrawlerCommand { get; set; }

        public ICommand StartProcessingCommand { get; set; }

        public ICommand RunQueryCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomePageViewModel()
        {
            // Create commands.
            GoToHowToPageCommand = new RelayCommand(GoToHowToPageCommandRoutine);
            StartCrawlerCommand = new RelayCommand(async () => await StartCrawlerCommandRoutineAsync());
            CancelCrawlerCommand = new RelayCommand(async () => await CancelCrawlerCommandRoutineAsync());
            StartProcessingCommand = new RelayCommand(async () => await StartProcessingCommandRoutineAsync());
            RunQueryCommand = new RelayCommand(async () => await RunQueryCommandRoutineAsync());
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public HomePageViewModel(ILogger logger, ICrawlerManager crawlerManager, ITaskManager taskManager, IFileManager fileManager, ICrawlerStorage crawlerStorage) : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _crawlerManager = crawlerManager ?? throw new ArgumentNullException(nameof(crawlerManager));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _crawlerStorage = crawlerStorage ?? throw new ArgumentNullException(nameof(crawlerStorage));

            // HACK: crawler starter
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

        private async Task StartCrawlerCommandRoutineAsync()
        {
            if (_crawler == null)
                return;

            //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
            await LoadAsync();
            _crawler.Start();
            IsCurrentlyCrawlingFlag = true;

            await Task.Delay(1);
        }

        private async Task CancelCrawlerCommandRoutineAsync()
        {
            if (_crawler == null)
                return;

            //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
            _crawler.Cancel();
            IsCurrentlyCrawlingFlag = false;

            await Task.Delay(1);
        }

        private async Task StartProcessingCommandRoutineAsync()
        {
            await RunCommandAsync(() => ProcessingCommandFlag, async () =>
            {
                Console.WriteLine("XX");
                var filePaths = _crawlerStorage.GetDataFiles(_crawler);
                if (filePaths != null)
                {
                    var dataFilePath = filePaths.FirstOrDefault(o => o.Contains(".json"));
                    if (dataFilePath != null)
                    {
                        if (!_crawler.IsCurrentlyCrawling)
                        {
                            // Deserialize JSON directly from a file
                            using (StreamReader file = File.OpenText(dataFilePath))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                CrawlerDataModel[] data = (CrawlerDataModel[])serializer.Deserialize(file, typeof(CrawlerDataModel[]));

                                List<IndexDocumentDataModel> docs = new List<IndexDocumentDataModel>();
                                for (int i = 0; i < data.Length; ++i)
                                    docs.Add(new IndexDocumentDataModel(i, data[i].Title, data[i].Category, data[i].Timestamp, data[i].Content));

                                // HACK - start index processing
                                DataProcessingStatus = "Indexing...";
                                var processing = new IndexProcessing("my_index", new Tokenizer(), new Stemmer(), new StopWordRemover(), _fileManager);
                                await _taskManager.Run(() =>
                                {
                                    processing.IndexDocuments(docs.ToArray(), true);
                                    _logger.LogDebugSource("Index processing done!");
                                    DataProcessingStatus = "Done! Data has been indexed into a binary file.";
                                });
                            }
                        }
                        else
                        {
                            DataProcessingStatus = "Cannot process data during crawling!";
                        }
                    }
                    else
                    {
                        DataProcessingStatus = "No data found! Use crawler to get data first.";
                    }
                }

                await Task.Delay(1);
            });
        }

        private async Task RunQueryCommandRoutineAsync()
        {
            // TODO : we need to have a currently processing index name list to be able to say when we are able to touch the data 
            // (as a feature update while multiple processing will run and we want to query only the instances that are not indexing/processing atm and visa/versa).

            await RunCommandAsync(() => ProcessingCommandFlag, async () =>
            {
                Console.WriteLine(Query);
                await Task.Delay(1);
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load data
        /// </summary>
        private async Task LoadAsync()
        {
            _crawler = await _crawlerManager.GetCrawlerAsync("bdo-naeu");
            // Set the events
            //     - Raise the property changed in the UI thread (crawler is running in a different assembly on a separate thread)
            _crawler.OnStartProcessEvent += (s, e) =>
            {
                CrawlerProcessProgress = "Starting...";
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            };
            _crawler.OnProcessProgressEvent += (s, e) =>
            {
                CrawlerProcessProgress = $"{e.CrawlingProgressPct}%";
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            };
            _crawler.OnFinishProcessEvent += (s, e) =>
            {
                CrawlerProcessProgress = "Done!";
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            };
        }

        #endregion
    }
}
