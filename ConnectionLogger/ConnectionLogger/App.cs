using ConnectionLogger.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ConnectionLogger
{
    public class App
    {
        public static INavigationActions ExternalNavigator { get; set; }

        public static Page GetMainPage()
        {
            RootPage = new NavigationPage(new MainPage());
            return RootPage;
        }

        public static Page RootPage { get; private set; }
    }
}
