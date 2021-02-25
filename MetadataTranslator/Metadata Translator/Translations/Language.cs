using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata_Translator
{
    public class Language : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string LanguageTag { get; set; }
        public string TranslationId { get; set; }
        public string TranslationGroup { get; set; }
        public string DisplayName { get; set; }
        public string NativeName { get; set; }

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public bool IsModelDefault { get; set; }
        public bool IsNotModelDefault { get => !IsModelDefault; }
    }
}