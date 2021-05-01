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
    /// Interaction logic for SettingsPanel.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
        public SettingsPanel()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if(mainWnd != null)
            {
                mainWnd.SettingsToggle.IsChecked = false;
            }
        }

        private void Visibility_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if (mainWnd == null) return;

            if (e.NewValue is Boolean show && show == true)
            {
                AsServerInfo.Text = $"Data Source={mainWnd.PowerBIEngine};Initial Catalog={mainWnd.DatabaseName}";
                SubscriptionKey.Text = mainWnd.SubscriptionKey;
                TranslatorEndpoint.Text = mainWnd.TranslatorEndpoint;
                TranslatorLocation.Text = mainWnd.TranslatorLocation;
                OverwriteTranslation.IsChecked = mainWnd.OverwriteTranslation;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if (mainWnd != null)
            {
                mainWnd.SubscriptionKey = SubscriptionKey.Text;
                mainWnd.TranslatorEndpoint = TranslatorEndpoint.Text;
                mainWnd.TranslatorLocation = TranslatorLocation.Text;
                mainWnd.OverwriteTranslation = (bool) OverwriteTranslation.IsChecked;
                mainWnd.SettingsToggle.IsChecked = false;
            }
        }
    }
}
