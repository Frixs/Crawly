using Ixs.DNA;
using System.Diagnostics;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page.
    /// </summary>
    public static class ApplicationPageHelpers
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page.
            switch (page)
            {
                case ApplicationPage.Home:
                    return new HomePage();

                case ApplicationPage.HowTo:
                    return new HowToPage();

                case ApplicationPage.DataInstance:
                    return new DataInstancePage();

                case ApplicationPage.CreateDataInstance:
                    return new CreateDataInstancePage();

                default:
                    // Log it.
                    FrameworkDI.Logger.LogErrorSource("A selected application page value is out of box!");
                    Debugger.Break();
                    return new HomePage();
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="ApplicationPage"/> that is for that type of page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            // Find application page that patches the base page.
            if (page is HomePage)
                return ApplicationPage.Home;

            if (page is HowToPage)
                return ApplicationPage.HowTo;

            if (page is DataInstancePage)
                return ApplicationPage.DataInstance;

            if (page is CreateDataInstancePage)
                return ApplicationPage.CreateDataInstance;

            // Log it.
            FrameworkDI.Logger.LogErrorSource("A selected base page value is out of box!");
            // Alert developer of issue.
            Debugger.Break();
            return ApplicationPage.Home;
        }
    }
}
