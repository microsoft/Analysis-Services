using BismNormalizer;
using Microsoft.AnalysisServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmToolkit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
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
    }
}
