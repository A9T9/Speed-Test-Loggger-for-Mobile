using Android.Content;
using Android.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger
{
    public class InternetCheker:IInternetCheker
    {
        Context _androidContext;

        public InternetCheker(Context context)
        {
            _androidContext = context;
        }

        /// <summary>
        /// Check for internet connection
        /// </summary>
        /// <returns></returns>
        public bool IsInternetAvailable()
        {
            try
            {

                var connectivityManager = (ConnectivityManager)_androidContext.GetSystemService(Context.ConnectivityService);

                var activeConnection = connectivityManager.ActiveNetworkInfo;

                if ((activeConnection != null) && activeConnection.IsConnected)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                var t = ex.Message;
            }
            return false;
        }
    }
}
