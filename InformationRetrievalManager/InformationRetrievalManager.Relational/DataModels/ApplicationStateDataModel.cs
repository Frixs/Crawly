using InformationRetrievalManager.Core;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model for the state of the application data that needs to be rembembered for the next run.
    /// "App's memory" on load from the previous closure.
    /// </summary>
    [ValidableModel(typeof(ApplicationStateDataModel))]
    public class ApplicationStateDataModel
    {
        #region Limit Constants

        public static readonly double MainWindowSizeX_MinValue = 960;
        public static readonly double MainWindowSizeX_MaxValue = int.MaxValue;
        public static readonly double MainWindowSizeX_DefaultValue = MainWindowSizeX_MinValue;

        public static readonly double MainWindowSizeY_MinValue = 560;
        public static readonly double MainWindowSizeY_MaxValue = int.MaxValue;
        public static readonly double MainWindowSizeY_DefaultValue = MainWindowSizeY_MinValue;

        #endregion

        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        [ValidateIgnore]
        public long Id { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// MainWindow size size on X axis
        /// </summary>
        [ValidateDouble(nameof(MainWindowSizeX), typeof(ApplicationStateDataModel),
            pMinValue: nameof(MainWindowSizeX_MinValue),
            pMaxValue: nameof(MainWindowSizeX_MaxValue))]
        public double MainWindowSizeX { get; set; } = MainWindowSizeX_DefaultValue;

        /// <summary>
        /// MainWindow size size on Y axis
        /// </summary>
        [ValidateDouble(nameof(MainWindowSizeY), typeof(ApplicationStateDataModel),
            pMinValue: nameof(MainWindowSizeY_MinValue),
            pMaxValue: nameof(MainWindowSizeY_MaxValue))]
        public double MainWindowSizeY { get; set; } = MainWindowSizeY_DefaultValue;

        #endregion
    }
}
