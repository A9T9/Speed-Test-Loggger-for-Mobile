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
    public partial class OpenLogPage : BaseView
    {

        public OpenLogPage()
        {
            //Initialize UI
            InitializeComponent();
            SetViewModel<OpenLogViewModel>();
        }
                 

        #region Navigation
        protected override void OnAppearing()
        {
            var vm = GetViewModel<OpenLogViewModel>();
            vm.RefreshLogs();
            base.OnAppearing();
        }
        #endregion

        #region Controls Handlers
        public async void ItemTapped(object sender, ItemTappedEventArgs args)
        {
            var log = args.Item as LogFileViewModel;
            await Global.LoadConnectionLog(log.Name);
            await Navigation.PopToRootAsync();
        }
        #endregion
    }
}
