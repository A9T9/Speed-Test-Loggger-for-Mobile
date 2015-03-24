using ConnectionLogger.Models.Utils;
using ConnectionLogger.Settings;
using Newtonsoft.Json.Linq;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;



namespace ConnectionLogger.ViewModels
{
    public class SettingsViewModel:BaseViewModel
    {
        #region Constructor
        public SettingsViewModel()
        {
            Title = "Settings";
        }
        #endregion

        #region Events
        #endregion

        #region Members
        private Dictionary<int, string> _urlOptions = new Dictionary<int,string>{{0,"Automatic"},{1,"Custom:"}};

        private Dictionary<int, string> _dateOptions = new Dictionary<int, string> 
        { 
        { 0, "One World (ISO Format) - yyyy/mm/dd 24h clock" }, 
        { 1, "EU Format - dd/mm/yyyy 24h clock" },
        { 2, "US Format - mm/dd/yyyy 12h clock" }
        };

        private Dictionary<int, string> _speedOptions = new Dictionary<int, string> 
        { 
        { 0, "Bits per-second - kb/s, mb/s" }, 
        { 1, "Bytes per-second - KB/s, MB/s" }
        };

        private ObservableCollection<string> _checkItems = new ObservableCollection<string>
        {
            "2",
            "3",
            "4"
        };
        #endregion

        #region Properties

        public Dictionary<int, string> UrlOptions
        {
            get {
                return _urlOptions; 
            }
            set
            {
                _urlOptions = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<int, string> DateOptions
        {
            get
            {
                return _dateOptions;
            }
            set
            {
                _dateOptions = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<int, string> SpeedOptions
        {
            get
            {
                return _speedOptions;
            }
            set
            {
                _speedOptions = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> CheckItems
        {
            get
            { return _checkItems; }
        }

        public int SelectedIndexForUrl
        {
            get
            {
                return AppSettings.SelectedURLIndex;
            }
            set
            {
                if (AppSettings.SelectedURLIndex != value)
                {
                    AppSettings.SelectedURLIndex = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("IsUrlEnabled");
                }
            }
        }

        public int SelectedIndexForDate
        {
            get
            {
                return AppSettings.TimeFormat;
            }
            set
            {
                if (AppSettings.TimeFormat != value)
                {
                    AppSettings.TimeFormat = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int SelectedIndexForSpeed
        {
            get
            {
                return AppSettings.SelectedSpeedIndex;
            }
            set
            {
                if (AppSettings.SelectedSpeedIndex != value)
                {
                    AppSettings.SelectedSpeedIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int CheckConnectionTime
        {
            get
            {
                return AppSettings.SelectedConnectionIndex;
            }
            set
            {
                if (AppSettings.SelectedConnectionIndex != value)
                {
                    AppSettings.SelectedConnectionIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsUrlEnabled
        {
            get
            {
                return SelectedIndexForUrl == 1;
            }
        }

        public string DownloadURL
        {
            get
            { return AppSettings.DownloadURL; }
            set
            {
                AppSettings.DownloadURL = value;
                NotifyPropertyChanged();
            }
        }

        public string LogFile
        {
            get
            { return AppSettings.LogFile; }
            set
            {
                AppSettings.LogFile = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Methods

        
        #endregion
    }
}
