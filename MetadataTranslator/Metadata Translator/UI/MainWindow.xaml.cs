using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();

            SetDependencyProperty("SubscriptionKey");
            SetDependencyProperty("TranslatorEndpoint");
            SetDependencyProperty("TranslatorLocation");
            SetDependencyProperty("OverwriteTranslation");
            SetDependencyProperty("LastUsedExportFolder");

            if (string.IsNullOrEmpty(SubscriptionKey))
            {
                SettingsToggle.IsChecked = true;
            }
        }

        public MainWindow(StartupEventArgs e) : this()
        {
            if (e.Args.Length == 2)
            {
                ConnectToDataset(e.Args[0], e.Args[1]);
            }
        }

        public void AddColumn(string lcid)
        {
            Language language = DataModel.GetLanguageByLcid(lcid);
            if (language != null)
            {
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = language.DisplayName,
                    Binding = new Binding(lcid)
                });

                language.IsSelected = true;
            }
        }

        /// <summary>
        /// Connects to the dataset using a connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public void ConnectToDataset(string connectionString)
        {
            try
            {
                DataModel = DataModel.Connect(connectionString);
                InitTranslationUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Connects to the dataset via server and database name.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        public void ConnectToDataset(string server, string database)
        {
            try
            {
                DataModel = DataModel.Connect(server, database);
                InitTranslationUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
        }

        #region Helper functions
        /// <summary>
        /// Initializes the translation grid.
        /// </summary>
        private void InitTranslationUI()
        {
            PowerBIEngine = DataModel.ServerName;
            DatabaseName = DataModel.DatabaseName;

            SetDependencyProperty("Languages");
            InitializeDataGrid();
        }

        /// <summary>
        /// Set Dependency Properties that wrap apps ettings.
        /// </summary>
        /// <param name="parameterName"></param>
        private void SetDependencyProperty(string parameterName)
        {
            switch (parameterName)
            {
                case "SubscriptionKey":
                    SubscriptionKey = Properties.Settings.Default.SubscriptionKey;
                    break;
                case "TranslatorEndpoint":
                    TranslatorEndpoint = Properties.Settings.Default.TranslatorEndpoint;
                    break;
                case "TranslatorLocation":
                    TranslatorLocation = Properties.Settings.Default.TranslatorLocation;
                    break;
                case "OverwriteTranslation":
                    OverwriteTranslation = Properties.Settings.Default.OverwriteTranslation;
                    break;
                case "LastUsedExportFolder":
                    LastUsedExportFolder = Properties.Settings.Default.LastUsedExportFolder;
                    break;
                case "Languages":
                    Languages = new ObservableCollection<Language>();
                    foreach (var language in DataModel.SupportedLanguages)
                    {
                        Languages.Add(language);
                    }
                    break;
            }
        }

        /// <summary>
        /// Initializes the datagrid columns and sets the ItemsSource to the default collection.
        /// </summary>
        private void InitializeDataGrid()
        {
            List<string> cultures = DataModel.CultureNames;

            /// Create some setters and triggers for the 
            /// styles of the read-only columns.
            /// 
            Trigger iIsSelectedTrigger = new Trigger()
            {
                Property = DataGridTextColumn.IsReadOnlyProperty,
                Value = true
            };

            var foregroundSetter = new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.Black));
            iIsSelectedTrigger.Setters.Add(foregroundSetter);

            var backgroundSetter = new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightGray));

            /// The first column is for the metadata container object.
            /// 
            var objectColumnStyle = new Style(typeof(DataGridCell));
            objectColumnStyle.Setters.Add(backgroundSetter);
            objectColumnStyle.Setters.Add(foregroundSetter);
            objectColumnStyle.Triggers.Add(iIsSelectedTrigger);
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = FindResource("TabularObjectColumnHeader").ToString(),
                Binding = new Binding(DataModel.ContainerColumnHeader),
                IsReadOnly = true,
                CellStyle = objectColumnStyle
            });

            /// The second column is for the default culture of the data model,
            /// which is always the first language in the list of data model cultures.
            /// 
            var defaultLangColumnStyle = new Style(typeof(DataGridCell));
            defaultLangColumnStyle.Setters.Add(foregroundSetter);
            defaultLangColumnStyle.Triggers.Add(iIsSelectedTrigger);

            /// Add a tooltip to flag that the default culture is readonly.
            /// 
            var objectHeaderStyle = new Style(typeof(DataGridColumnHeader));
            objectHeaderStyle.Setters.Add(new Setter(ToolTipService.ToolTipProperty, 
                FindResource("DefaultCultureColumnHeaderToolTip").ToString()));

            Language defaultLang = DataModel.GetLanguageByLcid(cultures[0]);
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = $"{defaultLang.DisplayName}*",
                HeaderStyle = objectHeaderStyle,
                Binding = new Binding(cultures[0]),
                IsReadOnly = true,
                CellStyle = defaultLangColumnStyle
            });
            DataModel.SetLanguageFlags(cultures[0], true, true);


            /// Add the remaining languages that already exist in the data model
            /// and mark them as selected in the list of supported languages.
            /// 
            for (int i = 1; i< cultures.Count; i++)
            {
                AddColumn(cultures[i]);
            }

            /// And set Captions as the default content of the datagrid.
            dataGrid.ItemsSource = DataModel.Captions;
        }

        /// <summary>
        /// Get a handle to the main window object so that other user controls can
        /// access the public properties of the main window object directly.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static MainWindow GetMainWindow(DependencyObject child)
        {
            if (child == null) return null;

            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject is MainWindow parent)
            {
                return parent;
            }
            else
            {
                return GetMainWindow(parentObject);
            }
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// DataModel object
        /// </summary>
        public static readonly DependencyProperty DataModelProperty =
            DependencyProperty.Register("DataModel", typeof(DataModel), typeof(MainWindow));

        public DataModel DataModel
        {
            get { return (DataModel)GetValue(DataModelProperty); }
            set { SetValue(DataModelProperty, value); }
        }

        /// <summary>
        /// Supported languages collection
        /// </summary>
        public static readonly DependencyProperty LanguagesProperty =
            DependencyProperty.Register("Languages", typeof(ObservableCollection<Language>), typeof(MainWindow),
            new PropertyMetadata(null, new PropertyChangedCallback(OnLanguagesChanged)));

        public ObservableCollection<Language> Languages
        {
            get { return (ObservableCollection<Language>)GetValue(LanguagesProperty); }
            set { SetValue(LanguagesProperty, value); }
        }

        private static void OnLanguagesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as MainWindow).OnLanguagesChanged(e);
        }

        private void OnLanguagesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ObservableCollection<Language> newCollection)
            {
                newCollection.CollectionChanged += LanguagesCollection_Changed;

                if (newCollection.Count > 0)
                    AttachLanguagePropertyChangedEventHandler(newCollection);
            }
            else if (e.OldValue is ObservableCollection<Language> oldCollection)
            {
                oldCollection.CollectionChanged -= LanguagesCollection_Changed;

                if (oldCollection.Count > 0)
                    RemoveLanguagePropertyChangedEventHandler(oldCollection);
            }
        }

        private void LanguagesCollection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AttachLanguagePropertyChangedEventHandler(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveLanguagePropertyChangedEventHandler(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveLanguagePropertyChangedEventHandler(e.OldItems);
                    AttachLanguagePropertyChangedEventHandler(e.NewItems);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void AttachLanguagePropertyChangedEventHandler(System.Collections.IList items)
        {
            foreach (Language language in items)
            {
                language.PropertyChanged += LanguageProperty_Changed;
            }
        }

        private void RemoveLanguagePropertyChangedEventHandler(System.Collections.IList items)
        {
            foreach (Language language in items)
            {
                language.PropertyChanged -= LanguageProperty_Changed;
            }
        }

        /// <summary>
        /// Event handler for LanguageProperty_Changed event to add or remove a lanugage 
        /// from the datagrid headers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageProperty_Changed(object sender, PropertyChangedEventArgs e)
        {
            
            if(sender is Language language 
                && e.PropertyName == "IsSelected"
                && !language.LanguageTag.Equals(DataModel.DefaultCulture))
            {
                var existingColumn = dataGrid.Columns.Where(x => language.DisplayName.Equals(x.Header.ToString())).FirstOrDefault();
                if (language.IsSelected)
                {
                    if (existingColumn == null)
                    {
                        dataGrid.Columns.Add(new DataGridTextColumn
                        {
                            Header = language.DisplayName,
                            Binding = new Binding(language.LanguageTag)
                        });
                    }
                }
                else
                {
                    if (existingColumn != null)
                    {
                        dataGrid.Columns.Remove(existingColumn);
                    }
                }
            }
        }

        /// <summary>
        /// SubscriptionKey Property
        /// </summary>
        public static readonly DependencyProperty SubscriptionKeyProperty =
        DependencyProperty.Register("SubscriptionKey", typeof(string), typeof(MainWindow),
            new PropertyMetadata(new PropertyChangedCallback(OnSubscriptionKeyChanged)));

        public string SubscriptionKey
        {
            get { return (string)GetValue(SubscriptionKeyProperty); }
            set { SetValue(SubscriptionKeyProperty, value); }
        }

        private static void OnSubscriptionKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Properties.Settings.Default.SubscriptionKey = (string)e.NewValue;
        }

        /// <summary>
        /// TranslatorEndpoint Property
        /// </summary>
        public static readonly DependencyProperty TranslatorEndpointProperty =
            DependencyProperty.Register("TranslatorEndpoint", typeof(string), typeof(MainWindow),
                new PropertyMetadata(new PropertyChangedCallback(OnTranslatorEndpointChanged)));

        public string TranslatorEndpoint
        {
            get { return (string)GetValue(TranslatorEndpointProperty); }
            set { SetValue(TranslatorEndpointProperty, value); }
        }

        private static void OnTranslatorEndpointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Properties.Settings.Default.TranslatorEndpoint = (string)e.NewValue;
        }

        /// <summary>
        /// TranslatorLocation Property
        /// </summary>
        public static readonly DependencyProperty TranslatorLocationProperty =
            DependencyProperty.Register("TranslatorLocation", typeof(string), typeof(MainWindow),
                new PropertyMetadata(new PropertyChangedCallback(OnTranslatorLocationChanged)));

        public string TranslatorLocation
        {
            get { return (string)GetValue(TranslatorLocationProperty); }
            set { SetValue(TranslatorLocationProperty, value); }
        }

        private static void OnTranslatorLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Properties.Settings.Default.TranslatorLocation = (string)e.NewValue;
        }

        /// <summary>
        /// OverwriteTranslation Property
        /// </summary>
        public static readonly DependencyProperty OverwriteTranslationProperty =
            DependencyProperty.Register("OverwriteTranslation", typeof(bool), typeof(MainWindow),
                new PropertyMetadata(new PropertyChangedCallback(OnOverwriteTranslationChanged)));

        public bool OverwriteTranslation
        {
            get { return (bool)GetValue(OverwriteTranslationProperty); }
            set { SetValue(OverwriteTranslationProperty, value); }
        }

        private static void OnOverwriteTranslationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Properties.Settings.Default.OverwriteTranslation = (bool)e.NewValue;
        }

        /// <summary>
        ///  LastUsedExportFolder Property
        /// </summary>
        public static readonly DependencyProperty LastUsedExportFolderProperty =
            DependencyProperty.Register("LastUsedExportFolder", typeof(string), typeof(MainWindow),
                new PropertyMetadata(new PropertyChangedCallback(OnLastUsedExportFolderChanged)));

        public string LastUsedExportFolder
        {
            get { return (string)GetValue(LastUsedExportFolderProperty); }
            set { SetValue(LastUsedExportFolderProperty, value); }
        }

        private static void OnLastUsedExportFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Properties.Settings.Default.LastUsedExportFolder = (string)e.NewValue;
        }

        /// <summary>
        /// PowerBIEngine Property
        /// </summary>
        public static readonly DependencyProperty PowerBIEngineProperty =
            DependencyProperty.Register("PowerBIEngine", typeof(string), typeof(MainWindow));
        public string PowerBIEngine
        {
            set { SetValue(PowerBIEngineProperty, value); }
            get { return (string)GetValue(PowerBIEngineProperty); }
        }

        /// <summary>
        /// DatabaseName Property
        /// </summary>
        public static readonly DependencyProperty DatabaseNameProperty =
            DependencyProperty.Register("DatabaseName", typeof(string), typeof(MainWindow));
        public string DatabaseName
        {
            set { SetValue(DatabaseNameProperty, value); }
            get { return (string)GetValue(DatabaseNameProperty); }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for ToolBar_Loaded event to remove some of the 
        /// unnecessary standard UI elements of the toolbar.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if(sender is ToolBar toolBar)
            {
                if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
                {
                    overflowGrid.Visibility = Visibility.Collapsed;
                }

                if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
                {
                    mainPanelBorder.Margin = new Thickness();
                }
            }
        }

        /// <summary>
        /// Enabling the deselect of a selected toggle button on mouseclick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is RadioButton radio)
            {
                bool tagState = (radio.Tag == null);
                radio.Tag = (radio.Tag == null) ? new object() : null;
                radio.IsChecked = tagState;
            }
        }

        /// <summary>
        /// Unsets the tag that indicates if a toggle button was clicked to be deselected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnToggleButton_Uncheck(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio)
            {
                radio.Tag = null;
            }
        }

        /// <summary>
        /// Display the Captions in the datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCaptionRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataModel != null)
            {
                dataGrid.ItemsSource = DataModel.Captions;
            }
        }

        /// <summary>
        /// Display the Descriptions in the datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDescriptionRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataModel != null)
            {
                dataGrid.ItemsSource = DataModel.Descriptions;
            }
        }

        /// <summary>
        /// Display the DisplayFolder strings in the datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisplayFolderRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataModel != null)
            {
                dataGrid.ItemsSource = DataModel.DisplayFolders;
            }
        }

        /// <summary>
        /// Make the terms in the default culture more translatable by splitting the strings
        /// based on camel casing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrepareButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataModel == null) return;

            string defaultCulture = DataModel.DefaultCulture;
            foreach(ExpandoObject row in DataModel.GetAllDataRows())
            {
                ((IDictionary<String, Object>)row)[defaultCulture] = row.SeparateCamelCase(defaultCulture);
            }
        }

        /// <summary>
        /// Translate the strings tn the default culture into the 
        /// selected target languages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTranslateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataModel.HasTargetLanguages)
            {
                using (new Hourglass())
                {
                    var ts = new TranslatorService(Languages.Where(x => x.IsSelected == true)?.ToList(), DataModel.DefaultCulture, SubscriptionKey, TranslatorEndpoint, TranslatorLocation);
                    ts.Translate(DataModel.GetAllDataRows(), OverwriteTranslation);
                }
            }
            else
            {
                MessageBox.Show(FindResource("NothingToTranslate").ToString());
                LanguageToggle.IsChecked = true;
            }
        }

        /// <summary>
        /// Apply the current values in the Captions, Descriptions, and DisplayFolders collections to the data model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplyButton_Click(object sender, RoutedEventArgs e)
        {
            DataModel.Update();
        }

        /// <summary>
        /// Saves modified user settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainWindow_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
