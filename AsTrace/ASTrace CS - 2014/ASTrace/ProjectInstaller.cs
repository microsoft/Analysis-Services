using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace ASTrace
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

            Microsoft.Win32.RegistryKey software, microsoft, astrace;
            software = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
            microsoft = software.OpenSubKey("Microsoft", true);
            astrace = microsoft.CreateSubKey("ASTrace");
            astrace.SetValue("path", Environment.CurrentDirectory); 
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstaller1_BeforeUninstall(object sender, InstallEventArgs e)
        {
            try
            {
                Microsoft.Win32.RegistryKey software, microsoft;
                software = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
                microsoft = software.OpenSubKey("Microsoft",true);
                microsoft.DeleteSubKey("ASTrace"); 
            }
            catch { }

        }
    }
}