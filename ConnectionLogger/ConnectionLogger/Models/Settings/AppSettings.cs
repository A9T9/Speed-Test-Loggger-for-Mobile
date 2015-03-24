using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ConnectionLogger.Settings
{
    public class AppSettings
    {
        #region Constants
        
        const int _numOfRuns = 0;

		#endregion

        #region Events
        #endregion

        #region Fields
        static ISettingsContainer _localSettings;

        public static Color PageBackgroundColor = Color.Transparent;

        public static Random Rnd = new Random();

        #endregion

        #region Properties
        
        public static int NumOfRuns
        {
            get
            {
                if (_localSettings.GetValue("NumOfRuns") == null)
                {
                    _localSettings.SetValue("NumOfRuns", _numOfRuns);
                    return _numOfRuns;
                }
                return Convert.ToInt32(_localSettings.GetValue("NumOfRuns"));
            }
            set
            {
                _localSettings.SetValue("NumOfRuns", value);

            }
        }

        public static int TimeFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("TimeFormat")))
                {
                    return 0;
                }
                return int.Parse(_localSettings.GetValue("TimeFormat"));
            }
            set
            {
                _localSettings.SetValue("TimeFormat", value);
            }
        }

        public static int SelectedConnectionIndex
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("CheckConnectionTime")))
                {
                    return 0;
                }
                return int.Parse(_localSettings.GetValue("CheckConnectionTime"));
            }
            set
            {
                _localSettings.SetValue("CheckConnectionTime", value);
            }
        }

        public static int SelectedTimeIndex
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("SelectedTimeIndex")))
                {
                    return 0;
                }
                return int.Parse(_localSettings.GetValue("SelectedTimeIndex"));
            }
            set
            {
                _localSettings.SetValue("SelectedTimeIndex", value);
            }
        }

        public static TimeSpan CheckConnectionTime
        {
            get
            {
                switch (SelectedConnectionIndex)
                {
                    case 0: // 30 Seconds
                        return TimeSpan.FromSeconds(30);
                    case 1: // 1 Minute
                        return TimeSpan.FromMinutes(1);
                    case 2: // 5 Minutes
                        return TimeSpan.FromMinutes(5);
                    case 3: // 10 Minutes
                        return TimeSpan.FromMinutes(10);
                    case 4: // 30 Minutes
                        return TimeSpan.FromMinutes(30);
                    case 5: // 1 Hour
                        return TimeSpan.FromMinutes(60);
                }
                return TimeSpan.FromSeconds(-30);
            }
        }

        public static TimeSpan TimelineTime
        {
            get
            {
                switch (SelectedTimeIndex)
                {
                    case 0: // 10 Minutes
                        return TimeSpan.FromMinutes(-10);
                    case 1: // 30 Minutes
                        return TimeSpan.FromMinutes(-30);
                    case 2: // 1 Hour
                        return TimeSpan.FromMinutes(-60);
                    case 3: // 12 Hour
                        return TimeSpan.FromHours(-12);
                    case 4: // 24 Hour
                        return TimeSpan.FromHours(-24);
                }
                return TimeSpan.FromMinutes(-10);
            }
        }

        public static bool SpeedUnits
        {
            get
            {
                return SelectedSpeedIndex==1;
            }
        }

        public static string LogFile
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("LogFile")))
                {
                    return "log";
                }
                return _localSettings.GetValue("LogFile");
            }
            set
            {
                _localSettings.SetValue("LogFile", value);

            }
        }

        public static bool AutoDownload
        {
            get
            {
                return SelectedURLIndex == 0;
            }
            
        }

        public static string DownloadURL
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("DownloadURL")))
                {
                    return "http://anyserver/anyfile.zip";
                }
                return _localSettings.GetValue("DownloadURL");
            }
            set
            {
                _localSettings.SetValue("DownloadURL", value);

            }
        }

        public static string LastIP
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("LastIP")))
                {
                    return "";
                }
                return _localSettings.GetValue("LastIP");
            }
            set
            {
                _localSettings.SetValue("LastIP", value);

            }
        }

        public static int SelectedURLIndex
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("SelectedURLIndex")))
                {
                    return 0;
                }
                return int.Parse(_localSettings.GetValue("SelectedURLIndex"));
            }
            set
            {
                _localSettings.SetValue("SelectedURLIndex", value);

            }
        }

        public static int SelectedSpeedIndex
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("SelectedSpeedIndex")))
                {
                    return 0;
                }
                return int.Parse(_localSettings.GetValue("SelectedSpeedIndex"));
            }
            set
            {
                _localSettings.SetValue("SelectedSpeedIndex", value);
            }
        }

        public static bool ShowStatusOk
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("ShowStatusOk")))
                {
                    return true;
                }
                return bool.Parse(_localSettings.GetValue("ShowStatusOk"));
            }
            set
            {
                _localSettings.SetValue("ShowStatusOk", value);

            }
        }

        public static bool ShowStatusChanged
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("ShowStatusChanged")))
                {
                    return true;
                }
                return bool.Parse(_localSettings.GetValue("ShowStatusChanged"));
            }
            set
            {
                _localSettings.SetValue("ShowStatusChanged", value);

            }
        }

        public static bool ShowStatusError
        {
            get
            {
                if (string.IsNullOrEmpty(_localSettings.GetValue("ShowStatusError")))
                {
                    return true;
                }
                return bool.Parse(_localSettings.GetValue("ShowStatusError"));
            }
            set
            {
                _localSettings.SetValue("ShowStatusError", value);

            }
        }

        #endregion

        #region Methods
        public static void Init(ISettingsContainer settingsContainer)
        {
            _localSettings = settingsContainer;
        }
        #endregion
    }
}
