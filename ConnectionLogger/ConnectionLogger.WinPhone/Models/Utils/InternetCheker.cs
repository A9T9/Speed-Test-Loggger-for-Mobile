
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;



namespace ConnectionLogger
{
    public class InternetCheker : IInternetCheker
    {
        /// <summary>
        /// Check for internet connection
        /// </summary>
        /// <returns></returns>
        public bool IsInternetAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
            
        }
    }
}
