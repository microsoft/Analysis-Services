using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Interaction logic for LanguagePanel.xaml
    /// </summary>
    public partial class LanguagePanel : UserControl
    {
        public LanguagePanel()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = MainWindow.GetMainWindow(this);
            if (mainWnd != null)
            {
                mainWnd.LanguageToggle.IsChecked = false;
            }
        }

        #region Dependency Properties
        public static readonly DependencyProperty LanguagesProperty =
            DependencyProperty.Register("Languages", typeof(ObservableCollection<Language>), typeof(LanguagePanel),
            new PropertyMetadata(null, new PropertyChangedCallback(OnLanguagesChanged)));

        public ObservableCollection<Language> Languages
        {
            get { return (ObservableCollection<Language>)GetValue(LanguagesProperty); }
            set { SetValue(LanguagesProperty, value); }
        }

        private static void OnLanguagesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is LanguagePanel langPanel)
            {
                langPanel.OnLanguagesChanged(e);
            }
        }

        private void OnLanguagesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ObservableCollection<Language> newItem)
            {
                newItem.CollectionChanged += LanguagesCollection_Changed;
            }
            else if (e.OldValue is ObservableCollection<Language> oldItem)
            {
                oldItem.CollectionChanged -= LanguagesCollection_Changed;
            }
        }

        private void LanguagesCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            TranslationGroups = Languages.Select(x => new TranslationGroup { Name = x.TranslationGroup, Tag = x.TranslationId }).Distinct().ToList();
        }


        public static readonly DependencyProperty TranslationGroupsProperty =
            DependencyProperty.Register("TranslationGroups", typeof(List<TranslationGroup>), typeof(LanguagePanel));

        public List<TranslationGroup> TranslationGroups
        {
            get { return (List<TranslationGroup>)GetValue(TranslationGroupsProperty); }
            set { SetValue(TranslationGroupsProperty, value); }
        }
        #endregion
    }
}
