using ConnectionLogger.Settings;
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
    public partial class ChartPage : BaseView
    {

        public ChartPage()
        {
            //Initialize UI
            InitializeComponent();
            SetViewModel<MainPageViewModel>();
            ((DateTimeAxis)this.chart.PrimaryAxis).LabelStyle.LabelFormat = "hh:mm:ss";
            chartPicker.Items.Clear();
            chartPicker.Items.Add("10 Minutes");
            chartPicker.Items.Add("30 Minutes");
            chartPicker.Items.Add("1 Hour");
            chartPicker.Items.Add("12 Hours");
            chartPicker.Items.Add("24 Hours");
            var vm= GetViewModel<MainPageViewModel>();
            chartPicker.SelectedIndex = vm.TimelineIndex;
            chartPicker.SelectedIndexChanged += chartPicker_SelectedIndexChanged;

            UpdateAxis();

            chart.BindingContextChanged += (s, e) => { UpdateAxis(); };
        }

        
        #region Navigation
        protected override void OnAppearing()
        {
            if (Settings.AppSettings.SpeedUnits)
                numAxis.Title = new ChartAxisTitle() { Text = "MB/s" };
            else
                numAxis.Title = new ChartAxisTitle() { Text = "mb/s" };
            numAxis.Minimum = 0;
            base.OnAppearing();
        }
        #endregion

        #region Controls Handlers
        void chartPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var vm = GetViewModel<MainPageViewModel>();
            vm.TimelineIndex = chartPicker.SelectedIndex;

            UpdateAxis();
        }
                 
        void UpdateAxis()
        {
           /* var vm = GetViewModel<MainPageViewModel>();
            var axis = ((DateTimeAxis)this.chart.PrimaryAxis);
            //if (vm.TimelineIndex < 3)
            //    axis.IntervalType = DateTimeIntervalType.Minutes;
            //else
            //    axis.IntervalType = DateTimeIntervalType.Hours;
            axis.Maximum = DateTime.Now;
            axis.Minimum = DateTime.Now.Add(AppSettings.TimelineTime);*/
        }

        #endregion
    }
}
