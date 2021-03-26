using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WF = System.Windows.Forms;

namespace Metadata_Translator
{
    /// <summary>
    /// Interaction logic for ImportExportPanel.xaml
    /// </summary>
    public partial class ImportExportPanel : UserControl
    {
        public ImportExportPanel()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if (mainWnd != null)
            {
                mainWnd.ImportExportToggle.IsChecked = false;
            }
        }

        private void OnImportButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if (mainWnd != null)
            {
                Button importButton = sender as Button;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                // Set filter options and filter index.
                openFileDialog1.Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;

                openFileDialog1.Multiselect = true;
                openFileDialog1.CheckFileExists = true;

                if (openFileDialog1.ShowDialog() == true)
                {
                    using (new Hourglass())
                    {

                        foreach (string filePath in openFileDialog1.FileNames)
                        {
                            string lcid = Path.GetFileNameWithoutExtension(filePath);
                            mainWnd.AddColumn(lcid);

                            try
                            {
                                mainWnd.DataModel?.ImportFromCsv(filePath, lcid, mainWnd.OverwriteTranslation);
                            }
                            catch { }
                        }
                    }
                }

                mainWnd.ImportExportToggle.IsChecked = false;
            }
        }

        private void OnExportButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if (mainWnd != null)
            {
                Button exportButton = sender as Button;
                WF.FolderBrowserDialog folderDlg = new WF.FolderBrowserDialog
                {
                    SelectedPath = mainWnd.LastUsedExportFolder,
                    Description = (string)exportButton.Tag,
                    ShowNewFolderButton = true,
                };

                WF.DialogResult result = folderDlg.ShowDialog();
                if (result == WF.DialogResult.OK)
                {
                    mainWnd.LastUsedExportFolder = folderDlg.SelectedPath;

                    using (new Hourglass())
                    {
                        mainWnd.DataModel?.ExportToCsv(folderDlg.SelectedPath);
                    }
                }

                mainWnd.ImportExportToggle.IsChecked = false;
            }
        }
    }
}
