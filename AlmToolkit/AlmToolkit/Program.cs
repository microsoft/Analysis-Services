using BismNormalizer;
using Microsoft.AnalysisServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmToolkit
{
    static class Program
    {
        [DllImport(@"user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiAWarenessContext);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_UNAWARE = new IntPtr(-1);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                //Set current directory for CefSharp references
                Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Set DPI awareness context to unaware to prevent CefSharp from managing DPI scaling
            var dpiUnaware = SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_UNAWARE);
            if (!dpiUnaware)
            {
                // Display a warning if setting the DPI awareness context to unaware fails
                MessageBox.Show("Unable to apply DPI scaling. You might experience HDPI issues in the application.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            //If new install, see if need to migrate settings file from previous version
            try
            {
                if (Settings.Default.UpgradeRequired)
                {
                    Settings.Default.Upgrade();
                    Settings.Default.UpgradeRequired = false;
                    Settings.Default.Save();
                }
            }
            catch { }

            try
            {
                // Default web requests like AAD Auth to use windows credentials for proxy auth
                System.Net.WebRequest.DefaultWebProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                if (args != null && args.Length > 0)
                {
                    if (args.Length > 1)
                    //User opened from Desktop with server/db name
                    {
                        string serverName = args[0];
                        string databaseName = args[1];

                        ComparisonForm MainFrom = new ComparisonForm();
                        MainFrom.LoadFromDesktop(serverName, databaseName);
                        Application.Run(MainFrom);
                        return;
                    }
                    else
                    //User opened file with the program
                    {
                        string fileName = args[0];
                        //Check file exists, if not will run without args below
                        if (File.Exists(fileName))
                        {
                            ComparisonForm MainFrom = new ComparisonForm();
                            MainFrom.LoadFile(fileName);
                            Application.Run(MainFrom);
                            return;
                        }
                    }
                }

                //Without valid args
                Application.Run(new ComparisonForm());

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
