using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Xamarin.Forms;
using ConnectionLogger.Settings;
using Syncfusion.SfChart.XForms.WinPhone;
using XFormsRadioButton.WinPhone.Controls;


namespace ConnectionLogger.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            new SfChartRenderer();
            new RadioButtonRenderer();
            //Initialize settings
            AppSettings.Init(new LocalSettings());

            Global.InetChecker = new InternetCheker();

            Forms.Init();
            ConnectionLogger.App.ExternalNavigator = new NavigationActions();
            Content = ConnectionLogger.App.GetMainPage().ConvertPageToUIElement(this);
        }
    }
}
