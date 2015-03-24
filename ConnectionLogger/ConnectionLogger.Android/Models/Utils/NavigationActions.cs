
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConnectionLogger
{
    public class NavigationActions : INavigationActions
    {
        public event Action<string> OnNavigateToUrl;
        
        public void NavigateToUrl(string url)
        {
            if (OnNavigateToUrl != null)
                    OnNavigateToUrl(url);
        }
        
    }
}
