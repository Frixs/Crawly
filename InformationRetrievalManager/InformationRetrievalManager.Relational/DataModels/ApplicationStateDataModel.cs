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

        /// <summary>
        /// Limit property for this data model property
        /// </summary>
        public static readonly double MainWindowSizeX_MinValue = 800;

        /// <summary>
        /// Limit property for this data model property
        /// </summary>
        public static readonly double MainWindowSizeX_MaxValue = int.MaxValue;

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
        [ConstraintRange(800, int.MaxValue)]
        [ValidateDouble(nameof(MainWindowSizeX), typeof(ApplicationStateDataModel),
            pMinValue: nameof(MainWindowSizeX_MinValue),
            pMaxValue: nameof(MainWindowSizeX_MaxValue))]
        public double MainWindowSizeX { get; set; } = 803;

        /// <summary>
        /// MainWindow size size on Y axis
        /// </summary>
        [ConstraintRange(450, int.MaxValue)]
        public double MainWindowSizeY { get; set; } = 450;

        #endregion
    }
}
