using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Metadata_Translator
{
    /// <summary>
    /// Interaction logic for ConnectionStringInput.xaml
    /// </summary>
    public partial class ConnectionStringInput : UserControl
    {
        public ConnectionStringInput()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if (mainWnd != null)
            {
                mainWnd.ConnectToDataset(ConnectionString.Text);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
