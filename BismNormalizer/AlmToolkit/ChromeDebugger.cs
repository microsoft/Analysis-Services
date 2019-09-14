namespace AlmToolkit
{
    using CefSharp;
    using CefSharp.WinForms;

    class ChromeDebugger
    {
        // Declare a local instance of chromium and the main form in order to execute things from here in the main thread
        private static ChromiumWebBrowser _instanceBrowser = null;
        // The form class needs to be changed according to yours
        private static ComparisonForm _instanceMainForm = null;


        public ChromeDebugger(ChromiumWebBrowser originalBrowser, ComparisonForm mainForm)
        {
            _instanceBrowser = originalBrowser;
            _instanceMainForm = mainForm;
        }

        /// <summary>
        /// Used for debugging chrome application embedded in Form
        /// </summary>
        public void showDevTools()
        {
            _instanceBrowser.ShowDevTools();
        }

    }
}
