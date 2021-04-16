using InformationRetrievalManager.Core;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model for the state of the application data that needs to be rembembered for the next run.
    /// "Client app's memory"
    /// </summary>
    public class ApplicationStateDataModel
    {
        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        public long Id { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// MainWindow size size on X axis
        /// </summary>
        [ConstraintRange(800, int.MaxValue)]
        public double MainWindowSizeX { get; set; } = 803;

        /// <summary>
        /// MainWindow size size on Y axis
        /// </summary>
        [ConstraintRange(450, int.MaxValue)]
        public double MainWindowSizeY { get; set; } = 450;

        #endregion
    }
}
