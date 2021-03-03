using Ixs.DNA;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// The DI Service Lookup container for our application.
    /// </summary>
    public static class CoreDI
    {
        /// <summary>
        /// A shortcut to access the <see cref="IFileManager"/>.
        /// </summary>
        public static IFileManager File => Framework.Service<IFileManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ITaskManager"/>.
        /// </summary>
        public static ITaskManager Task => Framework.Service<ITaskManager>();
    }
}
