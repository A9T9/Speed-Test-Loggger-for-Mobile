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
    public class MainPageViewModel:ParentViewModel
    {
        #region Constructor
        public MainPageViewModel()
        {
            Title = "Connection Logger";
            p_StartupTime = DateTime.Now;
            p_CurrentIP = AppSettings.LastIP;

            _stopCommand = new Command(() =>
                {
                    _continueTimer = false;

                    _stopCommand.ChangeCanExecute();
                    _startCommand.ChangeCanExecute();

                   webClient.Cancel();

                    StatusText2 = "Stopped";
                    StatusText = "";

                    // Create a new ListViewItem object
                    ListViewItem item = new ListViewItem(DateTime.Now,0);
                    item.SubItems.Add(Global.ConvertBytes(0) + "/s");
                    item.SubItems.Add(p_CurrentIP);
                    item.SubItems.Add("OK");
                    item.SubItems.Add("Logger Stopped");
                    item.ImageIndex = 1;

                    // Add the ListViewItem to the log
                    AddItem(item);
                },
                    CanStop
                );
            
            _startCommand = new Command(() =>
                {
                    
                    if (Children.Count == 0)
                    {
                        StatusText = "Started logging at: " + Global.ConvertTime(DateTime.Now);

                        // Create a new ListViewItem object
                        ListViewItem item = new ListViewItem(DateTime.Now,0);
                        item.SubItems.Add(Global.ConvertBytes(0) + "/s");
                        item.SubItems.Add(p_CurrentIP);
                        item.SubItems.Add("OK");
                        item.SubItems.Add("Logger Started");
                        item.ImageIndex = 1;

                        // Add the ListViewItem to the log
                        AddItem(item);
                    }
                    _continueTimer = true;

                    _stopCommand.ChangeCanExecute();
                    _startCommand.ChangeCanExecute();

                    Device.StartTimer(TimeSpan.FromMilliseconds(1000), ContinueTimer);

                    GetRemoteIP();
                },
                CanStart
                );
            //add web client events handlers
            webClient.DownloadDataCompleted += webClient_DownloadDataCompleted;
            webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;

            //load and start logging
            Start();

        }
        #endregion
        
        #region Members
        // Private variables
        private string p_CurrentIP;
        private StatusCode p_CurrentStatus= StatusCode.Online;
        private int p_ElapsedSeconds;
        
        private DateTime p_StartupTime;

        private string _statusText;
        private string _statusText2;

        private DateTime _minTime;
        private DateTime _maxTime;

        //Chart points for offline data
        ObservableCollection<ChartDataPoint> _offlineItems;
        
        //menu/buttons commands
        Command _startCommand;
        Command _stopCommand;
        Command _testCommand;
        Command _aboutCommand;
        Command _helpCommand;
        Command _settingsCommand;

        //webclient, using single shared instance for all request
        private CustomWebClient webClient = new CustomWebClient();

        //var to indicate whether we are running or not
        bool _continueTimer;
        #endregion

        #region Properties
        
        /// <summary>
        /// Command to start logging
        /// </summary>
        public Command StartCommand
        {
            get
            {
                return _startCommand;
            }
        }

        /// <summary>
        /// Command to stop logging
        /// </summary>
        public Command StopCommand
        {
            get
            {
                return _stopCommand;
            }
        }

        /// <summary>
        /// Command for single speed test
        /// </summary>
        public Command TestCommand
        {
            get
            {
                return _testCommand ?? (_testCommand = new Command(() =>
                {
                    GetRemoteIP();
                }
                    ));
            }
        }

        public Command HelpCommand
        {
            get
            {
                return _helpCommand ?? (_helpCommand = new Command(() =>
                {
                    App.ExternalNavigator.NavigateToUrl("http://google.com");
                }
                    ));
            }
        }

        public Command SettingsCommand
        {
            get
            {
                return _settingsCommand ?? (_settingsCommand = new Command(() =>
                {
                    CommonActions.NavigateToPage(AppPages.SettingsPage);
                }
                    ));
            }
        }

        public Command AboutCommand
        {
            get
            {
                return _aboutCommand ?? (_aboutCommand = new Command(() =>
                {
                    CommonActions.NavigateToPage(AppPages.AboutPage);
                }
                    ));
            }
        }

        /// <summary>
        /// First line of status text
        /// </summary>
        public string StatusText
        {
            get
            { return _statusText; }
            set
            { 
                if (_statusText!=value)
                {
                    _statusText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Second line of status, indicates when test's running or stopped
        /// </summary>
        public string StatusText2
        {
            get
            { return _statusText2; }
            set
            {
                if (_statusText2 != value)
                {
                    _statusText2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime MinTime
        {
            get
            { return _minTime; }
            set
            {
                _minTime = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("MaxTime");
            }
        }

        public DateTime MaxTime
        {
            get
            { return DateTime.Now; }
            
        }

        /// <summary>
        /// Chart points
        /// Calculated in real time based on log, maybe not the best for performance
        /// </summary>
        public ObservableCollection<ChartDataPoint> LogChartItems 
        { 
            get
            {
                //return value
                List<ChartDataPoint> data = new List<ChartDataPoint>();

                //minimum time visible on chart based on timeline settings
                MinTime = DateTime.Now.Add(AppSettings.TimelineTime);
                
                //log items in our timeline
                var items = Children.Cast<ItemViewModel>().Where(f=>f.Time>MinTime).OrderBy(f=>f.Time);

                //value for starting point
                double minData = (items.Any()) ? items.First().SpeedNum : 0;

                //adding firstpoint
                data.Add(new ChartDataPoint(MinTime, minData));
                
                DateTime lastTime = DateTime.MinValue;
                double lastSpeed = 0;
                double maxValue = 0;

                //add data point for every log item, looking for max value
                foreach (ItemViewModel vm in items)
                {
                    data.Add(new ChartDataPoint(vm.Time, vm.SpeedNum));
                    lastTime = vm.Time;
                    lastSpeed = vm.SpeedNum;

                    if (maxValue < lastSpeed)
                        maxValue = lastSpeed;
                }

                //add extra point for current time every 3 seconds
                if ((DateTime.Now-lastTime).TotalSeconds>3)
                {
                    data.Add(new ChartDataPoint(DateTime.Now, lastSpeed));
                }

                
                //offlineItems
                //looking for log items where status was changed
                //we use this data for "offline" time charting
                var changedItems = Children.Cast<ItemViewModel>().OrderBy(f => f.Time).Where(f => f.IsChange).ToList();

                //chart data for offline chart
                List<ChartDataPoint> offData = new List<ChartDataPoint>();

                

                if (changedItems.Any())
                {
                    //last change point before timeline
                    //chech charting range start point in offline zone
                    var lastChange = changedItems.LastOrDefault(f => f.Time <= MinTime);
                    if (lastChange != null && lastChange.Status == "Offline")
                    {
                        ListViewItem item = new ListViewItem(MinTime.AddSeconds(1), 0);
                        item.SubItems.Add(Global.ConvertBytes(0) + "/s");
                        item.SubItems.Add(p_CurrentIP);
                        item.SubItems.Add("Offline");
                        item.SubItems.Add("Offline");
                        item.ImageIndex = 1;
                        changedItems.Insert(0, new ItemViewModel(item));
                    } 

                    //check if currently offline
                    if (changedItems.Last().Status == "Offline")
                    {
                        ListViewItem item = new ListViewItem(DateTime.Now,0);
                        item.SubItems.Add(Global.ConvertBytes(0) + "/s");
                        item.SubItems.Add(p_CurrentIP);
                        item.SubItems.Add("Offline");
                        item.SubItems.Add("Offline");
                        item.ImageIndex = 1;
                        changedItems.Add(new ItemViewModel(item));
                    }
                }

                //add offline data point for every log item with "change" status
                foreach (var item in changedItems)
                {
                    //do not check items out of charting timeline
                    if (item.Time < MinTime)
                        continue;
                    //if offline add point with max value
                    var last = offData.LastOrDefault();
                    if (item.Status=="Offline")
                    {
                        if (last != null && last.YValue == 0)
                            offData.Add(new ChartDataPoint(item.Time.AddSeconds(-1), 0));
                        offData.Add(new ChartDataPoint(item.Time, maxValue));
                    }
                    else
                    {
                        if (last!=null && last.YValue!=0)
                            offData.Add(new ChartDataPoint(item.Time.AddSeconds(-1), maxValue));
                        offData.Add(new ChartDataPoint(item.Time, 0));
                    }
                }

                OfflineItems = new ObservableCollection<ChartDataPoint>(offData);

                return new ObservableCollection<ChartDataPoint>(data);
            }
        }

        public ObservableCollection<ChartDataPoint> OfflineItems
        {
            get
            {
                return _offlineItems;
            }
            private set
            {
                _offlineItems = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// selected timeline index in dropdown
        /// </summary>
        public int TimelineIndex
        {
            get
            { return AppSettings.SelectedTimeIndex; }
            set
            {
                AppSettings.SelectedTimeIndex = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("LogChartItems");
            }
        }
        
        //values binded to toggleswitches to show/hide corresponding log items
        public bool ShowStatusOk
        {
            get
            { 
                return AppSettings.ShowStatusOk;
            }
            set
            {
                if (AppSettings.ShowStatusOk != value)
                {
                    AppSettings.ShowStatusOk = value;
                    NotifyPropertyChanged();
                    RefreshChildren();
                }
            }
        }
        public bool ShowStatusChanged
        {
            get
            {
                return AppSettings.ShowStatusChanged; 
            }
            set
            {
                if (AppSettings.ShowStatusChanged != value)
                {
                    AppSettings.ShowStatusChanged = value;
                    NotifyPropertyChanged();
                    RefreshChildren();
                }
            }
        }
        public bool ShowStatusError
        {
            get
            { return AppSettings.ShowStatusError; }
            set
            {
                if (AppSettings.ShowStatusError != value)
                {
                    AppSettings.ShowStatusError = value;
                    NotifyPropertyChanged();
                    RefreshChildren();
                }
            }
        }
        #endregion

        #region Methods

        async void Start()
        {
            //load prev session
            await Global.LoadConnectionLog(AppSettings.LogFile + ".csv");
            RefreshChildren();
            //start logging
            _startCommand.Execute("");
        }

        /// <summary>
        /// Indicates to continue logging or not, fired by timer every second
        /// </summary>
        /// <returns>true - timer will continue, false timer will stop</returns>
        bool ContinueTimer()
        {
            if (!_continueTimer)
                return false;

            p_ElapsedSeconds++;

            // Create a new TimeSpan object
            TimeSpan ts = new TimeSpan(0, 0, 0, p_ElapsedSeconds);

            //id time to start new test
            if (ts.TotalSeconds > AppSettings.CheckConnectionTime.TotalSeconds)
            {
                GetRemoteIP();
            }
            else
            {
                //updating status
                var timeTo = TimeSpan.FromSeconds(AppSettings.CheckConnectionTime.TotalSeconds - ts.TotalSeconds);
                StatusText2 = string.Format("Running - Next test in {0} minutes",timeTo.ToString(@"mm\:ss"));
            }

            //Updating chart every 2 seconds
            if ((p_ElapsedSeconds & 1) == 0)
                NotifyPropertyChanged("LogChartItems");

            return _continueTimer;
        }

        /// <summary>
        /// Can we stop? we can if we are running
        /// </summary>
        bool CanStop()
        {
            return _continueTimer;
        }
        bool CanStart()
        {
            return !_continueTimer;
        }

        /// <summary>Retrieves the remote IPv4 address for this system from icanhazip.com.</summary>
        public void GetRemoteIP()
        {
            // Reset elapsed seconds
            p_ElapsedSeconds = 0;

            if (Global.IsOnline)
            {
                // Create a new WebClient object and download the string
                webClient.DownloadStringAsync(new Uri("http://icanhazip.com/"));
            }
            else
            {
                // Local variables
                StatusCode newStatus = StatusCode.Offline;
                int imageIndex = 2;
                string ip = "-";
                string status = "-";

                // Set status to Offline
                StatusText = "Unable to log connection: " + Global.ConvertTime(DateTime.Now);

                if (p_CurrentIP!=ip)
                {
                    status = "IP Changed";
                    imageIndex = 1;
                    p_CurrentIP = ip;
                }

                if (!p_CurrentStatus.Equals(newStatus))
                {
                    status = "Offline";
                    imageIndex = 1;
                    p_CurrentStatus = newStatus;
                }

                // Create a new ListViewItem object
                ListViewItem item = new ListViewItem(DateTime.Now,0);
                item.SubItems.Add(Global.ConvertBytes(0) + "/s");
                item.SubItems.Add(p_CurrentIP);
                item.SubItems.Add("Offline");
                item.SubItems.Add(status);
                item.ImageIndex = imageIndex;

                // Add the ListViewItem to the log
                AddItem(item);
                
            }
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
            // Local variables
            string ip = "-";
            string status = "-";
            StatusCode newStatus = StatusCode.Online;
            int imageIndex = 0;

            if (e.Error == null) // String downloaded successfully
            {
                // Remove line breaks from the IP address
                ip = e.Result.Replace("\r", "").Replace("\n", "");

                StatusText = "Logging connection at: " + Global.ConvertTime(DateTime.Now);

                // Begin downloading the data, passing the IP to the DownloadDataCompleted event
                if (AppSettings.AutoDownload)
                {
                    string url = string.Empty;
                    long maximumSpeed = 0;

                    foreach (ListViewItem item in Global.ConnectionLog)
                    {
                        long speed = item.SpeedByte;

                        if (speed > maximumSpeed) maximumSpeed = speed;
                    }

                
                    // Auto-select URL based on maximum download speed
                    if (maximumSpeed < 1024000)
                    {
                        // Download 0.6 MB file
                        url = "http://81.169.245.14/~0.6MBfile.zip";
                    }
                    else if (maximumSpeed < 1024000 * 2)
                    {
                        // Download 1.5 MB file
                        url = "http://81.169.245.14/~1.5MBfile.zip";
                    }
                    else if (maximumSpeed < 1024000 * 3)
                    {
                        // Download 3 MB file
                        url = "http://81.169.245.14/~3MBfile.zip";
                    }
                    else if (maximumSpeed < 1024000 * 5)
                    {
                        // Download 6 MB file
                        url = "http://81.169.245.14/~6MBfile.zip";
                    }
                    else
                    {
                        // Download 12 MB file
                        url = "http://81.169.245.14/~12MBfile.zip";
                    }

                    webClient.DownloadDataAsync(new Uri(url), ip);
                }
                else webClient.DownloadDataAsync(new Uri(AppSettings.DownloadURL), ip);
            }
            else // An error occured downloading the string
            {
                StatusText = "Unable to log connection: " + Global.ConvertTime(DateTime.Now);

                // Set status to Offline
                newStatus = StatusCode.Offline;
                imageIndex = 2;
                ip = "-";

                if (!p_CurrentIP.Equals(ip))
                {
                    status = "IP Changed";
                    imageIndex = 1;
                    p_CurrentIP = ip;
                }

                if (!p_CurrentStatus.Equals(newStatus))
                {
                    status = "Offline";
                    imageIndex = 2;
                    p_CurrentStatus = newStatus;
                }

                // Create a new ListViewItem object
                ListViewItem item = new ListViewItem(DateTime.Now,0);
                item.SubItems.Add(Global.ConvertBytes(0) + "/s");
                item.SubItems.Add(p_CurrentIP);
                item.SubItems.Add("Offline");
                item.SubItems.Add(status);
                item.ImageIndex = imageIndex;

                // Add the ListViewItem to the log
                AddItem(item);
                                
            }
        }

        void webClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            // Retrieve the IP address
            string ip = (string)e.UserState;
                   
            // Local variables
            string status = "-";
            StatusCode newStatus = StatusCode.Online;
            int imageIndex = 0;
            ListViewItem item = null;

            if (e.Error == null) // Data downloaded successfully
            {
                // Update the StatusBar
                StatusText = "Connection logged, size: " + Global.ConvertBytes(webClient.AllBytesDownloaded) + " speed: " + Global.ConvertBytes(webClient.CurrentSpeed) + "/s";

                if (p_CurrentIP!=ip)
                {
                    status = "IP Changed";
                    imageIndex = 1;
                    p_CurrentIP = ip;
                }

                if (!p_CurrentStatus.Equals(newStatus))
                {
                    status = "Online";
                    imageIndex = 1;
                    p_CurrentStatus = newStatus;
                }

                // Create a new ListViewItem object
                item = new ListViewItem(DateTime.Now,webClient.CurrentSpeed);
                item.SubItems.Add(Global.ConvertBytes(webClient.CurrentSpeed) + "/s");
                item.SubItems.Add(p_CurrentIP);
                item.SubItems.Add("OK");
                item.SubItems.Add(status);
                item.ImageIndex = imageIndex;
            }
            else // An error occured downloading the data
            {
                // Update the StatusBar
                StatusText = "Unable to log connection: " + Global.ConvertTime(DateTime.Now);

                // Set status to Offline
                newStatus = StatusCode.InvalidURL;
                imageIndex = 2;
                //ip = "-";

                if (!p_CurrentIP.Equals(ip))
                {
                    status = "IP Changed";
                    imageIndex = 1;
                    p_CurrentIP = ip;
                }

                if (!p_CurrentStatus.Equals(newStatus))
                {
                    status = "Invalid URL";
                    imageIndex = 2;
                    p_CurrentStatus = newStatus;
                }

                // Create a new ListViewItem object
                item = new ListViewItem(DateTime.Now,0);
                item.SubItems.Add(Global.ConvertBytes(0) + "/s");
                item.SubItems.Add(p_CurrentIP);
                item.SubItems.Add("Error");
                item.SubItems.Add(status);
                item.ImageIndex = imageIndex;
            }

            if (item != null)
            {
                // Add the ListViewItem to the log
                AddItem(item);
                // Save the connection log
                Global.SaveConnectionLog(AppSettings.LogFile + ".csv");
            }
                                  
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
                StatusText = "Calculating speed: " + Global.ConvertBytes(webClient.CurrentSpeed) + "/s...";
        }

        /// <summary>
        /// Add new item to update log list in UI
        /// </summary>
        /// <param name="item"></param>
        void AddItem(ListViewItem item)
        {
            Global.ConnectionLog.Add(item);
            if ((item.IsStatusOk && AppSettings.ShowStatusOk)
                || (item.IsStatusChanged && AppSettings.ShowStatusChanged)
                || (item.IsStatusError && AppSettings.ShowStatusError))
            {
                var vm = new ItemViewModel(item);
                AddChild(vm);

                NotifyPropertyChanged("LogChartItems");
            }

            
        }

        /// <summary>
        /// Refresh log after reloading
        /// </summary>
        public void RefreshChildren()
        {
            ClearChildren();
            foreach (var item in Global.ConnectionLog.Where(f=>(f.IsStatusOk && AppSettings.ShowStatusOk)
                || (f.IsStatusChanged && AppSettings.ShowStatusChanged)
                || (f.IsStatusError && AppSettings.ShowStatusError)))
            {
                item.Refresh();
                var vm = new ItemViewModel(item);
                AddChild(vm);
            }
            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("LogChartItems");

        }
        #endregion
    }
}
