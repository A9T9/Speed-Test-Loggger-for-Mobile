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
    public partial class SettingsPage:BaseView
    {
        SettingsViewModel _vm;

        public SettingsPage()
        {
            //Initialize UI
            InitializeComponent();
            //there are a lot of binding bugs in XForms yet, so using this workarounds
            _vm = ViewModelProvider.GetViewModel<SettingsViewModel>();
            BindingContext = _vm;
            urlRadioGroup.CheckedChanged += urlRadioGroup_CheckedChanged;
            checkPicker.Items.Clear();
            checkPicker.Items.Add("30 Seconds");
            checkPicker.Items.Add("1 Minute");
            checkPicker.Items.Add("5 Minutes");
            checkPicker.Items.Add("10 Minutes");
            checkPicker.Items.Add("30 Minutes");
            checkPicker.Items.Add("1 Hour");

            checkPicker.SelectedIndex = _vm.CheckConnectionTime;
            checkPicker.SelectedIndexChanged += checkPicker_SelectedIndexChanged;

            dateRadioGroup.CheckedChanged += dateRadioGroup_CheckedChanged;
            speedRadioGroup.CheckedChanged += speedRadioGroup_CheckedChanged;
        }

        
        #region Navigation
        protected override void OnAppearing()
        {
            int ui = _vm.SelectedIndexForUrl;
            if (ui==0)
                urlRadioGroup.SelectedIndex = 1;
            urlRadioGroup.SelectedIndex = ui;

            int di =_vm.SelectedIndexForDate;
            if (di==0)
                dateRadioGroup.SelectedIndex = 1;
            dateRadioGroup.SelectedIndex = di;

            int si = _vm.SelectedIndexForSpeed;
            if (si == 0)
                speedRadioGroup.SelectedIndex = 1;
            speedRadioGroup.SelectedIndex = si;

            base.OnAppearing();
        }
        #endregion

        #region Controls Handlers
        void checkPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            _vm.CheckConnectionTime = checkPicker.SelectedIndex;
        }

        void speedRadioGroup_CheckedChanged(object sender, int e)
        {
            _vm.SelectedIndexForSpeed = e;
        }

        void dateRadioGroup_CheckedChanged(object sender, int e)
        {
            _vm.SelectedIndexForDate = e;
        }

        void urlRadioGroup_CheckedChanged(object sender, int e)
        {
            _vm.SelectedIndexForUrl = e;
        }
               
        async void OpenLog(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new OpenLogPage());
        }
        
        #endregion
    }
}
