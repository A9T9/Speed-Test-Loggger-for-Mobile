using Microsoft.Phone.Tasks;
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
            var task = new Microsoft.Phone.Tasks.WebBrowserTask
            {
                Uri = new Uri(url)
            };
            task.Show();
            if (OnNavigateToUrl != null)
                OnNavigateToUrl(url);
        }
    }
}
