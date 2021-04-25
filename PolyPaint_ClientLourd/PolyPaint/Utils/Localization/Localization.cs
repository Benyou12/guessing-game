using PolyPaint.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PolyPaint.Utils.Localization
{
    public class LocalizationExtension : Binding
    {
        public LocalizationExtension(string name) : base("[" + name + "]")
        {
            //Have to be removed when we have a setting page
            string locale = Global.language;
            TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo(locale);
            this.Mode = BindingMode.OneWay;
            this.Source = TranslationSource.Instance;
        }
    }

    public class TranslationSource : INotifyPropertyChanged
    {

        private static readonly TranslationSource instance = new TranslationSource();

        public static TranslationSource Instance
        {
            get { return instance; }
        }

        private readonly ResourceManager resManager = 
            Properties.Resources.ResourceManager;

        private CultureInfo currentCulture = null;

        public string this[string key]
        {
            get { return this.resManager.GetString(key, this.currentCulture); }
        }

        public CultureInfo CurrentCulture
        {
            get { return this.currentCulture; }

            set
            {
                if (this.currentCulture != value)
                {
                    this.currentCulture = value;

                    var @event = this.PropertyChanged;

                    if (@event != null)

                    {
                        @event.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
