using ConnectionLogger.Models.Utils;
using ConnectionLogger.ViewModels;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace ConnectionLogger.Views
{
    public partial class MainPage:TabbedPage
    {
        

        public MainPage()
        {
            //Initialize UI
            InitializeComponent();
            BindingContext = ViewModelProvider.GetViewModel<MainPageViewModel>();
            this.ToolbarItems[0].Icon = ImageSource.FromFile("start.png") as FileImageSource;
            this.ToolbarItems[1].Icon = ImageSource.FromFile("stop.png") as FileImageSource;
            this.ToolbarItems[2].Icon = ImageSource.FromFile("testnow.png") as FileImageSource;

            _logPage = new LogPage();
            _chartPage = new ChartPage();
            
            Children.Add(_chartPage);
            Children.Add(_logPage);
        }

        #region Members
        LogPage _logPage;
        ChartPage _chartPage;
        #endregion

        #region Navigation
        protected override void OnAppearing()
        {
            CommonActions.OnNavigateToPage += CommonActions_OnNavigateToPage;
            var vm = ViewModelProvider.GetViewModel<MainPageViewModel>();
            vm.RefreshChildren();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            CommonActions.OnNavigateToPage -= CommonActions_OnNavigateToPage;
            base.OnDisappearing();
        }
        
        async void CommonActions_OnNavigateToPage(AppPages obj)
        {
            switch (obj)
            {
                case AppPages.SettingsPage:
                    await Navigation.PushAsync(new SettingsPage());
                    break;
                case AppPages.AboutPage:
                    await Navigation.PushAsync(new AboutPage());
                    break;
            }
            
        }

        
        #endregion

        #region Controls Handlers
        //async void OnQuizzesClicked(object sender, EventArgs args)
        //{
        //    await Navigation.PushAsync(new QuizzesPage());
        //}
        
        #endregion
    }
}
