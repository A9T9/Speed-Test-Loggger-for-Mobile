using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;
using ConnectionLogger.Settings;
using Android.Content;

namespace ConnectionLogger.Droid
{
    [Activity(Label = "ConnectionLogger",
        MainLauncher = true,
        Icon = "@drawable/icon",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity :FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Initialize settings
            AppSettings.Init(new LocalSettings());

            Global.InetChecker = new InternetCheker(this);

            Xamarin.Forms.Forms.Init(this, bundle);
            ConnectionLogger.App.ExternalNavigator = new NavigationActions();
            ConnectionLogger.App.ExternalNavigator.OnNavigateToUrl += ExternalNavigator_OnNavigateToUrl;
            SetPage(ConnectionLogger.App.GetMainPage());
        }

        void ExternalNavigator_OnNavigateToUrl(string url)
        {
            var uri = Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }
    }
}

