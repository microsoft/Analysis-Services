using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TranslationGroupPanel.xaml
    /// </summary>
    public partial class TranslationGroupPanel : UserControl
    {
        public TranslationGroupPanel()
        {
            InitializeComponent();
        }

        #region Dependency Properties
        public static readonly DependencyProperty LanguagesProperty =
            DependencyProperty.Register("Languages", typeof(ObservableCollection<Language>), typeof(TranslationGroupPanel));

        public ObservableCollection<Language> Languages
        {
            get { return (ObservableCollection<Language>)GetValue(LanguagesProperty); }
            set { SetValue(LanguagesProperty, value); }
        }

        public static readonly DependencyProperty TranslationGroupIdProperty =
            DependencyProperty.Register("TranslationGroupId", typeof(string), typeof(TranslationGroupPanel));

        public string TranslationGroupId
        {
            get { return (string)GetValue(TranslationGroupIdProperty); }
            set { SetValue(TranslationGroupIdProperty, value); }
        }
        #endregion

        private void Languages_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (e.Item is Language langItem && langItem.TranslationId == TranslationGroupId); 
        }
    }
}
