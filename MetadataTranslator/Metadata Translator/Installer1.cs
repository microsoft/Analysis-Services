using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Permissions;
using System.Windows;
using System.IO;

namespace Metadata_Translator
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);
            MessageBox.Show("Commit");
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string exeName = Context.Parameters["assemblypath"];
            string appDir = Path.GetDirectoryName(exeName);
            string pbiSharedDir = Environment.ExpandEnvironmentVariables(@"%CommonProgramFiles%\microsoft shared\Power BI Desktop\External Tools\");
            string pbiToolsJsonTemplate = $"{appDir}\\metadata-translator.pbitool.json";
            if (File.Exists(pbiToolsJsonTemplate))
            {
                string pbiToolsJson = File.ReadAllText(pbiToolsJsonTemplate).Replace("<METADATA_TRANSLATOR_PATH>", exeName.Replace("\\", "\\\\"));
                File.WriteAllText($"{pbiSharedDir}\\metadata-translator.pbitool.json", pbiToolsJson);
            }
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            
            string pbiSharedDir = Environment.ExpandEnvironmentVariables(@"%CommonProgramFiles%\microsoft shared\Power BI Desktop\External Tools\");
            string pbiToolFile = $"{pbiSharedDir}\\metadata-translator.pbitool.json";
            if (File.Exists(pbiToolFile))
            {
                File.Delete(pbiToolFile);
            }
        }
    }
}
