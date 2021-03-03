using Ixs.DNA;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The DI Service Lookup container for our application.
    /// </summary>
    public static class DI
    {
        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>.
        /// </summary>
        public static ApplicationViewModel ViewModelApplication => Framework.Service<ApplicationViewModel>();
    }
}
