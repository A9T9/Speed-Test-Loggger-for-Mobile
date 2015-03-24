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
    public partial class LogPage : BaseView
    {

        public LogPage()
        {
            //Initialize UI
            InitializeComponent();
            SetViewModel<MainPageViewModel>();
        }
                 

        #region Navigation
        
        #endregion

        #region Controls Handlers
        
        #endregion
    }
}
