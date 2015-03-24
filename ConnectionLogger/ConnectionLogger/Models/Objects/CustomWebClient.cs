#region Using
using ConnectionLogger.Settings;
using ModernHttpClient;
// Imported namespaces
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
#endregion
namespace ConnectionLogger
{
    /// <summary>Custom WebClient object that supports timing the estimated time of arrival (ETA) of the file download.</summary>
    public class CustomWebClient : HttpClient
    {
        #region Events
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        public event EventHandler<DownloadDataCompletedEventArgs> DownloadDataCompleted;
        public event EventHandler<DownloadStringCompletedEventArgs> DownloadStringCompleted;
        #endregion

        #region Members
        // Private variables
        private long p_AllBytesDownloaded;
        private double p_CurrentSpeed;
        private long p_LastBytesDownloaded;
        private Stopwatch p_Stopwatch;
                
        string _currentIP;

        CancellationTokenSource _cancelToken;
        #endregion

        #region Properties
        /// <summary>Gets the total number of bytes downloaded using an asynchronous request.</summary>
        public long AllBytesDownloaded
        {
            get { return p_AllBytesDownloaded; }
        }

        /// <summary>Gets the current speed of data downloaded using an asynchronous request.</summary>
        public double CurrentSpeed
        {
            get { return p_CurrentSpeed; }
        }

        /// <summary>Gets the most recent number of bytes downloaded using an asynchronous request.</summary>
        public long LastBytesDownloaded
        {
            get { return p_LastBytesDownloaded; }
        }

        /// <summary>Gets the Stopwatch object used to calculate the estimated time of arrival (ETA).</summary>
        public Stopwatch Stopwatch
        {
            get { return p_Stopwatch; }
        }

        string CurrentIP
        {
            get
            {
                return _currentIP;
            }
            set
            {
                if (_currentIP!=value)
                {
                    _currentIP = value;
                    AppSettings.LastIP = value;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>Initializes a new instance of the CustomWebClient class.</summary>
        public CustomWebClient() : base(
            new NativeMessageHandler() {  UseCookies = false}
            )
        {
            DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true, NoStore = true };
            
            p_Stopwatch = new Stopwatch();

        }
        #endregion

        #region Methods
        /// <summary>Occurs when an asynchronous data download operation completes.</summary>
        protected void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
                {
                    Debug.WriteLine(string.Format("Got data, speed={1}, {0}", DateTime.Now.ToString(),CurrentSpeed));
                    // Reset the Stopwatch
                    p_Stopwatch.Reset();
                    p_Stopwatch.Stop();

                    if (DownloadDataCompleted != null)
                        DownloadDataCompleted(this,e);
                });
        }

        /// <summary>Occurs when an asynchronous download operation successfully transfers some or all of the data.</summary>
        protected void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            try
                {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (p_Stopwatch.IsRunning)
                    {
                        Debug.WriteLine(string.Format("Bytes received {1}, {0}", DateTime.Now.ToString(), e.BytesReceived));
                        long bytesChange = 0;
                        if (p_LastBytesDownloaded.Equals(0))
                        {

                            bytesChange = e.BytesReceived;
                        }
                        else
                        {
                            bytesChange = e.BytesReceived - p_LastBytesDownloaded;


                        }
                        if (bytesChange > 0)
                            p_AllBytesDownloaded += bytesChange;
                        p_LastBytesDownloaded = e.BytesReceived;
                        p_CurrentSpeed = p_AllBytesDownloaded * 1000 / p_Stopwatch.Elapsed.TotalMilliseconds;

                        Debug.WriteLine(string.Format("Bytes received total {1}, {0}", DateTime.Now.ToString(), p_AllBytesDownloaded));

                        if (DownloadProgressChanged != null)
                            DownloadProgressChanged(this, e);
                    }
                });
                }
            catch(Exception ex)
            {
                Debug.WriteLine(string.Format("Onprogress Error: {1}, {0}", DateTime.Now.ToString(), ex.Message));
            }
        }

        /// <summary>Occurs when an asynchronous string download operation completes.</summary>
        protected void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
                {
                    // Start the Stopwatch //Anton??
                    p_Stopwatch.Reset();
                    p_Stopwatch.Stop();
           

                    if (DownloadStringCompleted != null)
                        DownloadStringCompleted(this, e);
                });
        }

        public void DownloadStringAsync(Uri uri )
        {
            Debug.WriteLine(string.Format("Request to get IP, {0}", DateTime.Now.ToString()));
            GetAsync(uri, DownloadType.String);
            
        }

        public void DownloadDataAsync(Uri uri, object userToken)
        {
            Debug.WriteLine(string.Format("Request to get Data, {0}", DateTime.Now.ToString()));
            CurrentIP = userToken.ToString();
            GetAsync(uri, DownloadType.Data);
        }

        public void Cancel()
        {
            p_Stopwatch.Reset();
            p_Stopwatch.Stop();
            _cancelToken.Cancel();
        }

        public void GetAsync(Uri uri, DownloadType type)
        {
            if (p_Stopwatch.IsRunning)
            {
                Debug.WriteLine(string.Format("Still running prev test, {0}", DateTime.Now.ToString()));
                return;
            }

            _cancelToken = new CancellationTokenSource();
            var token = _cancelToken.Token;
            
            //ugly but ensure there will be no caching as on WP it seems to ignore all headers and caches anyway
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("{0}?timestamp={1}", uri.AbsoluteUri,DateTime.Now.Ticks));

            int read = 0;
            int offset = 0;
            string result = string.Empty;

            byte[] responseBuffer = new byte[131072];

            var operation = SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);

            p_AllBytesDownloaded = 0;
            p_CurrentSpeed = 0;
            p_LastBytesDownloaded = 0;

            
            p_Stopwatch.Start();

            Task.Run<string>(async () =>
            {

                using (var responseStream = await operation.Result.Content.ReadAsStreamAsync())
                {
                    do
                    {
                        if (token.IsCancellationRequested)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                                {
                                    Stopwatch.Reset();
                                    Stopwatch.Stop();
                                });
                            return null;
                        }
                        read = await responseStream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                        if (type== DownloadType.String)
                            result += Encoding.UTF8.GetString(responseBuffer, 0, read);
                        offset += read;

                        if (type == DownloadType.Data)
                            OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(offset));
                    } while (read != 0);
                                        
                    switch(type)
                    {
                        case DownloadType.String:
                            var ea = new DownloadStringCompletedEventArgs(result);
                            CheckConnectionStatus(ea,operation);
                            OnDownloadStringCompleted(ea);
                            break;
                        case DownloadType.Data:
                            var e2 = new DownloadDataCompletedEventArgs(CurrentIP);
                            CheckConnectionStatus(e2,operation);
                            OnDownloadDataCompleted(e2);
                            break;
                    }
                    
                }

                return result;
            });
        }

        void CheckConnectionStatus(DownloadBaseEventArgs ea, Task<HttpResponseMessage> operation)
        {
            if (operation.Exception == null)
            {
                if (operation.Result.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine(string.Format("Error: {1}, {0}", DateTime.Now.ToString(), operation.Result.StatusCode));
                    ea.Error = new Exception(operation.Result.ReasonPhrase);
                }
            }
            else
            {
                Debug.WriteLine(string.Format("Error: {1}, {0}", DateTime.Now.ToString(), operation.Exception.Message));
                ea.Error = operation.Exception;
            }
        }
        #endregion
    }
}
